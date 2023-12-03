using Notero.MidiAdapter;
using Notero.MidiGameplay.Bot;
using Notero.Unity.AudioModule;
using Notero.Unity.AudioModule.Core;
using Notero.Unity.MidiNoteInfo;
using Notero.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Notero.MidiGameplay.Core
{
    public class GameLogicController : IBotControllable, IMidiGameLogic
    {
        public UnityEvent<float> OnGameplayTimeUpdate { get; } = new();
        public UnityEvent OnGameplayStart { get; } = new();
        public UnityEvent OnGameplayEnd { get; } = new();
        public UnityEvent<MidiNoteInfo, double> OnNoteInfoPressed { get; } = new(); // send midi info and action time.
        public UnityEvent<MidiNoteInfo, double> OnNoteInfoReleased { get; } = new();
        public UnityEvent<MidiNoteInfo, double> OnNoteInfoNoteStarted { get; } = new();
        public UnityEvent<MidiNoteInfo, double> OnNoteInfoNoteEnded { get; } = new();
        public UnityEvent<int, double> OnBlankKeyPressed { get; } = new();
        public UnityEvent<int, double> OnBlankKeyReleased { get; } = new();

        #region Bot Controllable
        public UnityEvent<double> UpdateTime { get; } = new();
        public double GetCurrentTime() => (double)m_CurrentTime - NoteStartTimeOffset; //for bot only.
        #endregion

        public bool IsEnd => m_IsEnd;

        protected float m_SongTimeInSecond;

        #region Time Relates
        public ITimeProvider TimeProvider { get; private set; }

        /// <summary>
        /// Time that note will collide to indicator after spawning in milliseconds
        /// </summary>
        public double NoteStartTimeOffset { get; private set; }
        protected virtual float m_CurrentTime => TimeProvider.CurrentTime;
        #endregion

        public List<MidiNoteInfo> MidiNoteInfos
        {
            get => m_MidiNoteInfoList;
        }

        protected List<MidiNoteInfo> m_MidiNoteInfoList;
        protected Dictionary<int, List<MidiNoteInfo>> m_SpawnedNoteIdDict;
        protected Dictionary<int, List<MidiNoteInfo>> m_UnScoredNoteIdDict;

        protected IMidiKeyCallable m_KeyCallable;
        protected IMusicNotationControllable m_RaindropNoteController;

        protected float m_MidiTimeOffset;
        protected double m_EndTime;
        protected int m_ErrorsTimeMs;
        protected int m_PerfectTimeMs;

        protected float m_MusicEndTime;
        protected int m_CurrentNoteIndex;
        protected int m_CurrentBarlineIndex;
        public bool IsPlaying { get; private set; }
        protected bool m_IsEnd = true;

        protected Canvas m_GameplayCanvas;
        protected MidiFile m_CurrentMidiFile;

        protected AudioSpeaker m_CurrentAudioSpeaker;
        protected IAdjustableAudioClip m_Music;

        protected int m_OctaveInputAmount;
        protected int m_MinimumKeyId;
        protected int m_MaximumKeyId;
        private Coroutine m_StartGameplayCoroutine;

        public GameLogicController()
        {
            m_SpawnedNoteIdDict ??= new Dictionary<int, List<MidiNoteInfo>>();
            m_UnScoredNoteIdDict ??= new Dictionary<int, List<MidiNoteInfo>>();
        }

        public void SetTimeProvider(ITimeProvider timeProvider)
        {
            TimeProvider = timeProvider;
            TimeProvider.Reset();
        }

        public void SetMidiInput(IMidiKeyCallable keyCallable)
        {
            if(m_KeyCallable != null)
            {
                m_KeyCallable.NoteOnEvent -= OnKeyPressed;
                m_KeyCallable.NoteOffEvent -= OnKeyReleased;
            }

            m_KeyCallable = keyCallable;

            m_KeyCallable.NoteOnEvent += OnKeyPressed;
            m_KeyCallable.NoteOffEvent += OnKeyReleased;
        }

        public virtual void UpdateLogic()
        {
            ProcessLogic();
        }

        //TODO: remove after finish implement hold on
        public virtual void UpdateGUI() { }

        public void Dispose()
        {
            CoroutineHelper.Instance.Stop(m_StartGameplayCoroutine);
            m_CurrentAudioSpeaker?.Stop();
            MidiInputAdapter.Instance.SetAllLedOff();

            if(m_KeyCallable != null)
            {
                m_KeyCallable.NoteOnEvent -= OnKeyPressed;
                m_KeyCallable.NoteOffEvent -= OnKeyReleased;
            }
        }

        public void SetGameDuration(float songTimeInSeconds)
        {
            m_SongTimeInSecond = songTimeInSeconds;
        }

        public void SetMidi(MidiFile midiFile, float bpm = default)
        {
            m_CurrentMidiFile = midiFile;
            m_MidiNoteInfoList = MidiNoteInfoHelper.GenerateMidiNoteInfoListByFile(midiFile, bpm);
        }

        public void SetMusic(IAdjustableAudioClip music)
        {
            m_Music = music;
        }

        public void SetMidiTimeOffset(float value)
        {
            m_MidiTimeOffset = value;
        }

        /// <summary>
        /// Set time offset
        /// </summary>
        /// <param name="errorsTimeMs">Room for errors in milliseconds</param>
        /// <param name="perfectTimeMs">Room for errors of perfect case in milliseconds</param>
        public virtual void SetTimeOffset(int errorsTimeMs, int perfectTimeMs)
        {
            m_ErrorsTimeMs = errorsTimeMs;
            m_PerfectTimeMs = perfectTimeMs;
        }

        public virtual void SetGameplayCanvas(Canvas canvas)
        {
            m_GameplayCanvas = canvas;
        }

        public virtual void SetGameplayControllers(IMusicNotationControllable raindropNoteController)
        {
            m_RaindropNoteController = raindropNoteController;

            m_OctaveInputAmount = m_RaindropNoteController.OctaveInputAmount;
            m_MinimumKeyId = m_RaindropNoteController.MinimumKeyId;
            m_MaximumKeyId = m_MinimumKeyId + (m_OctaveInputAmount * 12) - 1;
        }

        public void StartGameplay(bool withBGMusic, float speedMultiplier = 1)
        {
            m_IsEnd = false;

            if(withBGMusic && m_Music != null)
            {
                m_StartGameplayCoroutine = CoroutineHelper.Instance.Play(StartGameplayWithMusicProcess());
            }
            else
            {
                m_StartGameplayCoroutine = CoroutineHelper.Instance.Play(StartGameplayProcess());
            }
        }

        public void Play()
        {
            IsPlaying = true;
            OnGameplayStart?.Invoke();
            ProcessLogic();
        }

        public void Pause() => IsPlaying = false;

        public void End()
        {
            if(!m_IsEnd)
            {
                Pause();
                m_IsEnd = true;
            }
        }

        public void PressKey(int midiId)
        {
            // @todo: Refactor later to avoid precompilation
#if !BOT_PLAY
            if(!Debug.isDebugBuild) return;
#endif

            if(!IsInVirtualPianoBounds(midiId)) return;

            int octaveIndex = midiId / 12;
            int noteNumber = midiId % 12;
            OnKeyPressed(midiId, octaveIndex, noteNumber);
        }

        public void ReleaseKey(int midiId)
        {
            // @todo: Refactor later to avoid precompilation
#if !BOT_PLAY
            if(!Debug.isDebugBuild) return;
#endif

            if(!IsInVirtualPianoBounds(midiId)) return;

            int octaveIndex = midiId / 12;
            int noteNumber = midiId % 12;
            OnKeyReleased(midiId, octaveIndex, noteNumber);
        }

        protected virtual void StartReleaseNotes()
        {
            m_EndTime = (m_SongTimeInSecond * 1000) + NoteStartTimeOffset;
            m_SpawnedNoteIdDict.Clear();
            m_UnScoredNoteIdDict.Clear();
            TimeProvider.SetTimeStart();
            m_CurrentNoteIndex = 0;
            m_CurrentBarlineIndex = 0;
            Play();
        }

        protected virtual IEnumerator StartGameplayWithMusicProcess()
        {
            if(m_Music.AudioClip == null)
            {
                Debug.Log("Wait until the music is ready to play.");
                yield return new WaitUntil(() => m_Music.AudioClip != null);
                Debug.Log("Music is ready to play.");
            }

            double noteStartTimeOffset = NoteStartTimeOffset / 1000F;
            double timeToReleaseNotes = m_MidiTimeOffset - noteStartTimeOffset;

            if(timeToReleaseNotes >= 0)
            {
                m_CurrentAudioSpeaker = AudioPlayer.Instance.Play(m_Music);
                m_MusicEndTime = TimeProvider.CurrentRefTime + m_Music.AudioClip.length;
                yield return new WaitForSeconds((float)timeToReleaseNotes);
                StartReleaseNotes();
            }
            else
            {
                double timeToPlayMusic = -timeToReleaseNotes;
                StartReleaseNotes();
                yield return new WaitForSeconds((float)timeToPlayMusic);
                m_CurrentAudioSpeaker = AudioPlayer.Instance.Play(m_Music);
                m_MusicEndTime = TimeProvider.CurrentRefTime + m_Music.AudioClip.length;
            }

            bool isMusicEnded()
            {
                return TimeProvider.CurrentRefTime > m_MusicEndTime;
            }

            yield return new WaitUntil(() => m_IsEnd && isMusicEnded());
            OnGameplayEnd?.Invoke();
        }

        protected virtual IEnumerator StartGameplayProcess()
        {
            StartReleaseNotes();
            m_MusicEndTime = TimeProvider.CurrentRefTime + m_SongTimeInSecond;

            bool isMusicEnded()
            {
                return TimeProvider.CurrentRefTime > m_MusicEndTime;
            }

            yield return new WaitUntil(() => m_IsEnd && isMusicEnded());
            OnGameplayEnd?.Invoke();
        }

        public void SetNoteStartTimeOffset(double timeOffset)
        {
            NoteStartTimeOffset = timeOffset;
        }

        protected virtual void ProcessLogic()
        {
            if(IsPlaying)
            {
                TimeProvider.UpdateTime();
                OnGameplayTimeUpdate?.Invoke(m_CurrentTime / 1000f);
                SpawnNote(m_CurrentTime);
                VerifyNote(m_CurrentTime);
                GameEndCheck(m_CurrentTime);
                UpdateTime?.Invoke(GetCurrentTime());
            }
        }

        protected void SpawnNote(float currentTime)
        {
            while(HasNoteToSpawn(currentTime))
            {
                MidiNoteInfo midiNoteInfo = m_MidiNoteInfoList[m_CurrentNoteIndex];

                if(IsInVirtualPianoBounds(midiNoteInfo.MidiId))
                {
                    midiNoteInfo.SetRaindropNoteId(m_CurrentNoteIndex);
                    AddToSpawnedNoteIdDict(midiNoteInfo);
                    m_RaindropNoteController.CreateActionCue(midiNoteInfo);
                }

                m_CurrentNoteIndex++;
            }
        }

        private bool IsInVirtualPianoBounds(int midiId)
        {
            return m_MinimumKeyId <= midiId && midiId <= m_MaximumKeyId;
        }

        protected void VerifyNote(float currentTime)
        {
            foreach(List<MidiNoteInfo> noteList in m_SpawnedNoteIdDict.Values)
            {
                foreach(MidiNoteInfo note in noteList.ToList())
                {
                    if(IsNoteStart(currentTime, note.NoteOnTime) && !note.IsStart)
                    {
                        OnNoteStart(note);
                    }
                    else if(IsNoteEnd(currentTime, note.NoteOffTime))
                    {
                        OnNoteEnd(note);
                    }
                }
            }
        }

        protected void GameEndCheck(float currentTime)
        {
            if(!m_IsEnd && currentTime > m_EndTime)
            {
                End();
            }
        }

        private bool HasNoteToSpawn(double currentTime)
        {
            return m_CurrentNoteIndex < m_MidiNoteInfoList.Count &&
                   currentTime >= m_MidiNoteInfoList[m_CurrentNoteIndex].NoteOnTime;
        }

        private bool IsNoteStart(double currentTime, double noteOnTime)
        {
            return currentTime >= noteOnTime + NoteStartTimeOffset;
        }

        private bool IsNoteEnd(double currentTime, double noteOffTime) => currentTime >= noteOffTime + NoteStartTimeOffset;

        private void AddToSpawnedNoteIdDict(MidiNoteInfo note) => AddToNoteIdDict(m_SpawnedNoteIdDict, note);

        protected void AddToUnScoredNoteIdDict(MidiNoteInfo note) => AddToNoteIdDict(m_UnScoredNoteIdDict, note);

        private void AddToNoteIdDict(Dictionary<int, List<MidiNoteInfo>> dict, MidiNoteInfo note)
        {
            int midiId = note.MidiId;

            if(!dict.ContainsKey(midiId))
            {
                dict[midiId] = new List<MidiNoteInfo>();
            }

            dict[midiId].Add(note);
        }

        protected bool RemoveFromSpawnedNoteIdDict(MidiNoteInfo noteInfo)
        {
            if(m_SpawnedNoteIdDict.TryGetValue(noteInfo.MidiId, out List<MidiNoteInfo> list))
            {
                return list.Remove(noteInfo);
            }

            return false;
        }

        protected bool TryGetSpawnedNote(int midiId, out MidiNoteInfo noteInfo, bool isReleased = false)
        {
            noteInfo = null;

            if(isReleased && RemoveUnScoredNote(midiId, out MidiNoteInfo unScoredNoteInfo))
            {
                noteInfo = unScoredNoteInfo;
            }
            else if(m_SpawnedNoteIdDict.TryGetValue(midiId, out List<MidiNoteInfo> list))
            {
                noteInfo = list.FirstOrDefault(note => !note.IsPlayed);
            }

            return noteInfo != null;
        }

        private bool RemoveUnScoredNote(int midiId, out MidiNoteInfo noteInfo)
        {
            noteInfo = null;

            if(m_UnScoredNoteIdDict.TryGetValue(midiId, out List<MidiNoteInfo> list))
            {
                noteInfo = list.FirstOrDefault(note => !note.IsPlayed);
                if(noteInfo != null) list.Remove(noteInfo);
            }

            return noteInfo != null;
        }

        protected virtual void OnKeyPressed(int midiId, int octaveIndex, int noteNumber)
        {
            double pressTimeMilliseconds = m_CurrentTime;

            if(TryGetSpawnedNote(midiId, out MidiNoteInfo note) && IsInNoteRange(note, pressTimeMilliseconds) && !note.IsPlayed)
            {
                note.SetIsPressed(true);

                OnNoteInfoPressed?.Invoke(note, pressTimeMilliseconds);
                return;
            }
            else
            {
                OnBlankKeyPressed?.Invoke(midiId, pressTimeMilliseconds);
            }
        }

        protected virtual void OnKeyReleased(int midiId, int octaveIndex, int noteNumber)
        {
            double releaseTimeMilliseconds = m_CurrentTime;

            if(TryGetSpawnedNote(midiId, out MidiNoteInfo note, true) && IsInNoteRange(note, releaseTimeMilliseconds))
            {
                if(note.IsPressed)
                {
                    note.SetIsPlayed(true);
                }

                OnNoteInfoReleased?.Invoke(note, releaseTimeMilliseconds);
                return;
            }
            else
            {
                OnBlankKeyReleased?.Invoke(midiId, releaseTimeMilliseconds);
            }
        }

        protected virtual void OnNoteStart(MidiNoteInfo noteInfo)
        {
            noteInfo.SetIsStart(true);

            OnNoteInfoNoteStarted?.Invoke(noteInfo, m_CurrentTime);

            MidiInputAdapter.Instance.SetLedOn(noteInfo.MidiId);
            Debug.Log($"{nameof(OnNoteStart)} note: {noteInfo.MidiId}");
        }

        protected virtual void OnNoteEnd(MidiNoteInfo noteInfo)
        {
            if(RemoveFromSpawnedNoteIdDict(noteInfo))
            {
                m_RaindropNoteController.RemoveActionCue(noteInfo.RaindropNoteId);

                if(noteInfo.IsPressed)
                {
                    AddToUnScoredNoteIdDict(noteInfo);
                }
            }

            OnNoteInfoNoteEnded?.Invoke(noteInfo, m_CurrentTime);

            MidiInputAdapter.Instance.SetLedOff(noteInfo.MidiId);
        }

        protected virtual bool IsInNoteRange(MidiNoteInfo noteInfo, double actionTime)
        {
            if(noteInfo == null)
                return false;

            int marginTime = Mathf.Max(m_ErrorsTimeMs, m_PerfectTimeMs);

            double noteStartTime = noteInfo.NoteOnTime + NoteStartTimeOffset - marginTime;
            double noteEndTime = noteInfo.NoteOffTime + NoteStartTimeOffset + marginTime;

            return (noteStartTime <= actionTime) && (actionTime <= noteEndTime);
        }
        public IEnumerator WaitEndcount(float Sec)
        {
            Debug.Log("ASD");
            yield return new WaitForSecondsRealtime(Sec);
        }
    }
}