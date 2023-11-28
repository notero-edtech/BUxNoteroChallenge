using BU.NineTails.Gameplay.Scoring;
using BU.NineTails.Gameplay;
using BU.NineTails.MidiGameplay.UI;
using BU.NineTails.Scripts.UI;
using BU.NineTails.Scripts.UI.VirtualPiano;
using Notero.MidiAdapter;
using Notero.MidiGameplay.Bot;
using Notero.MidiGameplay.Core;
using Notero.RaindropGameplay.Core;
using Notero.RaindropGameplay.Core.Scoring;
using Notero.Unity.AudioModule;
using Notero.Unity.MidiNoteInfo;
using Notero.Unity.UI.VirtualPiano;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace BU.NineTails.MidiGameplay.Gameplay
{
    public class DemoRaindropGameController : MonoBehaviour, IMidiGameController
    {
        [SerializeField]
        protected GameLogicConnector m_GameLogicController;

        [SerializeField]
        protected GameplayConfig m_GameplayConfig;

        [SerializeField]
        protected GameObject m_TimeProviderGO;
        protected ITimeProvider m_TimeProvider;

        [SerializeField]
        protected BaseVirtualPiano m_VirtualPiano;

        [SerializeField]
        protected DemoRaindropNoteController m_RaindropNoteController;

        [SerializeField]
        protected GameObject m_IGameplayUIControllable;
        protected IGameplayUIControllable m_GameplayUIController;

        [SerializeField]
        protected Texture m_CurrentBackgroundTexture;

        [SerializeField]
        protected int m_RaindropOctaveAmount = 4;

        [SerializeField]
        protected int m_RaindropMinimumKeyId = 24;

        [SerializeField]
        protected int m_RaindropMaximumKeyId = 47;

        [SerializeField]
        protected RectTransform m_ActionBar;

        [SerializeField]
        private HealthSliderController health;

        [SerializeField]
        private CharactersAnimationController[] characters;

        protected BaseScoringProcessor m_ScoringController;
        public GameplayModeController ModeController { get; protected set; }

        public double NoteStartTimeOffset { get; private set; }
        protected MidiFile m_CurrentMidiFile;
        protected IAdjustableAudioClip m_CurrentMusic;
        protected float m_CurrentMidiTimeOffset;
        protected float m_SongTimeInSecond;
        protected float m_CustomBPM;
        private int m_CurrentBarlineIndex = 0;

        public float SpawnPointPos => m_GameplayConfig.SpawnPointPos;
        public bool IsMusicLoaded => m_CurrentMusic != null && m_CurrentMusic.LoadState == AudioDataLoadState.Loaded;
        public IMidiGameLogic GameLogic { get; protected set; }
        public IBotControllable BotControllable => null;
        public bool IsMusicExists => m_CurrentMusic != null;
        public bool IsControllerReady => m_VirtualPiano != null && m_ScoringController != null;

        protected HashSet<int> m_MidiInputHashSet = new();

        public UnityEvent OnMusicLoadingCompleted { get; } = new();
        public UnityEvent OnGameEnded { get; } = new();
        public UnityEvent OnGameStarted { get; } = new();
        public UnityEvent<SelfResultInfo> OnGameResultSubmitted { get; } = new();

        private void Awake()
        {
            characters = FindObjectsOfType<CharactersAnimationController>();
            m_GameplayUIController = m_IGameplayUIControllable.GetComponent<IGameplayUIControllable>();
            m_TimeProvider = m_TimeProviderGO.GetComponent<ITimeProvider>();
            GameLogic = m_GameLogicController.GameLogicController;
        }

        private void Update()
        {
            GameLogic?.UpdateLogic();
        }

        public virtual void Initial()
        {
            m_GameplayUIController.SetCanvasScale();

            if(m_CurrentBackgroundTexture != null) m_GameplayUIController.SetBackgroundImage(m_CurrentBackgroundTexture);

            m_RaindropNoteController.Init(SpawnPointPos);
            m_GameplayUIController.SetupPianoFeedback(m_VirtualPiano, m_RaindropNoteController.LanePosList);

            StartCoroutine(ProcessMusicLoadingCompletedEvent());
        }

        public virtual void Setup(MidiFile midiFile, IAdjustableAudioClip music, float midiTimeOffset, float customBPM = 0)
        {
            m_CurrentMidiFile = midiFile;
            m_CurrentMusic = music;
            m_CurrentMidiTimeOffset = midiTimeOffset;
            var accompaniment = music.AudioClip;

            if(customBPM <= 0)
            {
                m_CustomBPM = midiFile.BPM;
            }

            m_ScoringController = new DemoScoringController(MidiNoteInfoHelper.GenerateMidiNoteInfoListByFile(midiFile, m_CustomBPM));

            m_SongTimeInSecond = accompaniment.length * (midiFile.BPM / m_CustomBPM);
            ModeController = new GameplayModeController();
            m_RaindropNoteController.Setup(m_RaindropOctaveAmount, m_RaindropMinimumKeyId, m_GameplayConfig.RaindropScrollSpeed);
            m_RaindropNoteController.BarlineSetup(m_CustomBPM, m_CurrentMidiFile.TopNumberTimeSignature, m_CurrentMidiFile.BottomNumberTimeSignature, m_SongTimeInSecond);
            m_ScoringController.SetGameplayConfig(m_GameplayConfig);
        }

        protected IEnumerator ProcessMusicLoadingCompletedEvent()
        {
            yield return new WaitUntil(() => IsMusicLoaded);
            OnMusicLoadingCompleted?.Invoke();
        }

        public void SetCountInCallback(Action callback)
        {
            if(m_GameplayUIController == null) return;
            m_GameplayUIController.SetCountInCallback(callback);
        }

        public void SetCountInActive(bool isActive)
        {
            if(m_GameplayUIController == null) return;
            m_GameplayUIController.SetCountInActive(isActive);
        }

        public void StartGameplay()
        {
            m_CurrentBarlineIndex = 0;
            m_MidiInputHashSet.Clear();

            if((ModeController.Mode == GameplayMode.Normal || ModeController.Mode == GameplayMode.LibraryNormal))
            {
                GameLogic.StartGameplay(true);
            }
            else
            {
                GameLogic.StartGameplay(false);
            }

            OnGameStarted?.Invoke();
        }

        public virtual void SetupGameLogicController()
        {
            GameLogic.SetMidiInput(MidiInputAdapter.Instance);
            GameLogic.SetGameDuration(m_SongTimeInSecond);
            GameLogic.SetTimeOffset(m_GameplayConfig.GoodTimeMs, m_GameplayConfig.PerfectTimeMs);
            GameLogic.SetTimeProvider(m_TimeProvider);
            GameLogic.SetMidi(m_CurrentMidiFile, m_CustomBPM);
            GameLogic.SetMusic(m_CurrentMusic);
            GameLogic.SetMidiTimeOffset(m_CurrentMidiTimeOffset);
            GameLogic.SetGameplayCanvas(m_GameplayUIController.GameplayCanvas);
            GameLogic.SetGameplayControllers(m_RaindropNoteController);
            NoteStartTimeOffset = CalculateNoteStartTimeOffset(m_RaindropNoteController.RaindropScrollSpeed);
            GameLogic.SetNoteStartTimeOffset(NoteStartTimeOffset);
            m_ScoringController.SetNoteStartTimeOffset(NoteStartTimeOffset);

            GameLogic.OnGameplayEnd.AddListener(OnGameEnd);
            SubscribeGameFeedback();
            SubscribeGameplayTimeUpdateEventHandlers();
        }

        private void OnGameEnd()
        {
            OnGameEnded?.Invoke();
            UnsubscribeGameFeedback();
            UnsubscribeGameplayTimeUpdateEventHandlers();
            var selfResultInfo = m_ScoringController.GetScoringInfo();
            OnGameResultSubmitted?.Invoke(selfResultInfo);
            m_MidiInputHashSet.Clear();

        }

        public void ResetController()
        {
            GameLogic?.Dispose();
        }

        /// <summary>
        /// Select raindrop gameplay mode[normal, holdon]
        /// </summary>
        /// <param name="mode"></param>
        public void SelectMode(GameplayMode mode) => ModeController.SelectMode(mode);

        public void SetupScoringController()
        {
            m_ScoringController.OnNoteTimingProcessed.AddListener(m_GameplayUIController.UpdateTextFeedback);
            m_ScoringController.OnNoteTimingProcessed.AddListener(m_GameplayUIController.UpdatePianoFeedback);
            m_ScoringController.OnScoreUpdated.AddListener(m_GameplayUIController.HandleScoreUpdate);
        }

        public void SubscribeGameFeedback()
        {
            GameLogic.OnNoteInfoPressed.AddListener(OnNotePressed);
            GameLogic.OnNoteInfoReleased.AddListener(OnNoteReleased);
            GameLogic.OnNoteInfoNoteStarted.AddListener(OnNoteStarted);
            GameLogic.OnNoteInfoNoteEnded.AddListener(OnNoteEnded);
            GameLogic.OnBlankKeyPressed.AddListener(OnBlankKeyPressed);
            GameLogic.OnBlankKeyReleased.AddListener(OnBlankKeyReleased);
        }

        public void UnsubscribeGameFeedback()
        {
            GameLogic.OnNoteInfoPressed.RemoveListener(OnNotePressed);
            GameLogic.OnNoteInfoReleased.RemoveListener(OnNoteReleased);
            GameLogic.OnNoteInfoNoteStarted.RemoveListener(OnNoteStarted);
            GameLogic.OnNoteInfoNoteEnded.RemoveListener(OnNoteEnded);
            GameLogic.OnBlankKeyPressed.RemoveListener(OnBlankKeyPressed);
            GameLogic.OnBlankKeyReleased.RemoveListener(OnBlankKeyReleased);
        }

        private void SubscribeGameplayTimeUpdateEventHandlers()
        {
            GameLogic.OnGameplayTimeUpdate.AddListener(m_GameplayUIController.HandleGameplayTimeUpdate);
            //GameLogic.OnGameplayTimeUpdate.AddListener(ActiveBarline);
            GameLogic.OnGameplayTimeUpdate.AddListener(m_RaindropNoteController.UpdateLogic);
        }

        private void UnsubscribeGameplayTimeUpdateEventHandlers()
        {
            GameLogic.OnGameplayTimeUpdate.RemoveAllListeners();
        }

        private void OnNoteStarted(MidiNoteInfo note, double time)
        {
            if(!IsPressing(note.MidiId))
            {
                Handside hand = HandIdentifier.GetHandsideByTrackIndex(note.TrackIndex);
                int customValue = MidiNoteMapper.MapMidiToCustom(note.MidiId);
                m_VirtualPiano.SetCueIn(note.MidiId, hand, false, customValue);
            }

            m_GameplayUIController.UpdateFeedbackOnNoteStart(note, time);
        }

        private void OnNoteEnded(MidiNoteInfo note, double time)
        {
            int midiId = note.MidiId;
            int customValue = MidiNoteMapper.MapMidiToCustom(note.MidiId);
            if (!IsPressing(note.MidiId)) m_VirtualPiano.SetDefault(note.MidiId, false, customValue);
            m_GameplayUIController.UpdateTextFeedbackOnNoteEnd(note, time);
            health.Opps_Healthbar();
            foreach (var character in characters)
            {
                character.CheckOppsAnimation(midiId);
            }
        }

        private void OnNotePressed(MidiNoteInfo note, double time)
        {
            var midiId = note.MidiId;
            int customValue = MidiNoteMapper.MapMidiToCustom(note.MidiId);
            Assert.IsFalse(m_MidiInputHashSet.Contains(midiId), $"Duplicate key press without release on note: {midiId}");
            m_MidiInputHashSet.Add(midiId);

            m_ScoringController.ProcessNotePressTiming(note, time);

            Handside hand = HandIdentifier.GetHandsideByTrackIndex(note.TrackIndex);
            m_VirtualPiano.SetCueIn(midiId, hand, true, customValue);
            m_RaindropNoteController.SetCorrect(note.RaindropNoteId);
        }

        private void OnNoteReleased(MidiNoteInfo note, double time)
        {
            m_ScoringController.ProcessNoteReleaseTiming(note, time);
            const bool isPressing = false;
            int midiId = note.MidiId;
            int customValue = MidiNoteMapper.MapMidiToCustom(note.MidiId);

            if (IsPressing(midiId))
            {
                m_MidiInputHashSet.Remove(midiId);
            }

            if(note.IsPressed)
            {
                var score = m_ScoringController.CalculateTimingScore(note, time, false);

                if (score.ToString() == "Perfect")
                {
                    health.Perfect_Healthbar();
                    foreach (var character in characters)
                    {
                        character.CheckPerfectAnimation(midiId);
                    }
                }
                else if(score.ToString() == "Good")
                {
                    health.Good_Healthbar();
                    foreach (var character in characters)
                    {
                        character.CheckGoodAnimation(midiId);
                    }
                }
                else if (score.ToString() == "Oops")
                {
                    health.Opps_Healthbar();
                    foreach (var character in characters)
                    {
                        character.CheckOppsAnimation(midiId);
                    }
                }

                if (m_RaindropNoteController.IsCueShowing(note.RaindropNoteId) && score != NoteTimingScore.Perfect)
                {
                    m_RaindropNoteController.SetMiss(note.RaindropNoteId);
                    if (isPressing == true) m_VirtualPiano.SetMissKey(midiId, isPressing, 7);
                    else m_VirtualPiano.SetMissKey(midiId, isPressing, customValue);
                    m_GameplayUIController.UpdatePianoFeedback(note, score.ToString(), "Release");
                    m_GameplayUIController.UpdateTextFeedback(note, score.ToString(), "Release");
                }
                else
                {
                    if (isPressing == true) m_VirtualPiano.SetDefault(midiId, isPressing, 7);
                    else m_VirtualPiano.SetDefault(midiId, isPressing, customValue);
                }
            }
            else
            {
                Handside hand = HandIdentifier.GetHandsideByTrackIndex(note.TrackIndex);
                if (isPressing == true) m_VirtualPiano.SetCueIn(midiId, hand, isPressing, customValue);
                else m_VirtualPiano.SetCueIn(midiId, hand, isPressing, customValue);
            }
        }

        private void OnBlankKeyPressed(int midiId, double time)
        {
            Assert.IsFalse(m_MidiInputHashSet.Contains(midiId), $"Duplicate key press without release on note: {midiId}");
            m_MidiInputHashSet.Add(midiId);
            int customValue = MidiNoteMapper.MapMidiToCustom(midiId);
            Debug.Log($"Note {midiId}, Custom Value: {customValue}");
            if (!GameLogic.IsPlaying)
            {
                m_VirtualPiano.SetDefault(midiId, true, 7);
                return;
            }

            m_ScoringController.ProcessBlankKeyPress(midiId, time);

            m_GameplayUIController.UpdateFeedbackBlankKeyPress(midiId, time);
            m_VirtualPiano.SetMissKey(midiId, true, 7);
        }

        private void OnBlankKeyReleased(int midiId, double time)
        {
            int customValue = MidiNoteMapper.MapMidiToCustom(midiId);
            Debug.Log($"Note {midiId}, Custom Value: {customValue}");
            if (IsPressing(midiId))
            {
                m_MidiInputHashSet.Remove(midiId);
            }

            if(!GameLogic.IsPlaying)
            {
                m_VirtualPiano.SetDefault(midiId, false, customValue);
                return;
            }

            m_ScoringController.ProcessBlankKeyRelease(midiId, time);

            m_GameplayUIController.UpdateFeedbackBlankKeyRelease(midiId, time);
            m_VirtualPiano.SetDefault(midiId, false, customValue);
            health.Opps_Healthbar();
            foreach (var character in characters)
            {
                character.CheckOppsAnimation(midiId);
            }
        }

        public void CreateVirtualPiano() => m_VirtualPiano.Create("Gameplay_Test");

        public void SetGameplayOverlayActive(bool isActive)
        {
            if(m_RaindropNoteController == null) return;

            //m_RaindropNoteController.SetBarOverlayActive(isActive);

            SetGameplayScene();
        }

        public virtual void SetCompetitivePanelActive(bool isActive)
        {
            if(m_GameplayUIController != null)
            {
                m_GameplayUIController.SetupTimerDisplay(m_SongTimeInSecond + (float)NoteStartTimeOffset / 1000f);
                m_GameplayUIController.SetupAccuracyMeterBar(m_GameplayConfig);
                m_GameplayUIController.SetAccuracyMeterBarActive(isActive);
                m_GameplayUIController.SetCompetitivePanelActive(isActive);
                m_GameplayUIController.SetScoreDisplayActive(isActive);
                m_GameplayUIController.SetTimerDisplayActive(isActive);
            }
        }

        /// <summary>
        /// Clear or reset related gameplay components
        /// </summary>
        public void ClearGameplayComponents()
        {
            if(m_VirtualPiano != null) m_VirtualPiano.DeleteAllPianoKeys();

            m_RaindropNoteController?.ResetCues();

            SetCompetitivePanelActive(false);
            SetGameplayOverlayActive(false);
            m_GameplayUIController.SetActionBarActive(false);
        }

        protected virtual double CalculateNoteStartTimeOffset(float noteSpeed)
        {
            var destination = m_ActionBar.anchoredPosition.x;
            //var origin = ((RectTransform)m_RaindropNoteController.RaindropNoteSpawner.transform).anchoredPosition.x;
            var origin = 1270;
            float distance = Mathf.Abs(destination - origin);
            Debug.Log("Destination = " + destination + " Origin = " + origin + " Distance = "+ distance);
            return distance / noteSpeed * 1000;
        }

        public class MidiNoteMapper
        {
            public static int MapMidiToCustom(int midiNote)
            {
                if(midiNote == 24 || midiNote == 36 || midiNote == 48 || midiNote == 60 || midiNote == 72)
                {
                    return 0;
                }
                else if (midiNote == 26 || midiNote == 38 || midiNote == 50 || midiNote == 62 || midiNote == 74)
                {
                    return 1;
                }
                else if (midiNote == 28 || midiNote == 40 || midiNote == 52 || midiNote == 64 || midiNote == 76)
                {
                    return 2;
                }
                else if (midiNote == 29 || midiNote == 41 || midiNote == 53 || midiNote == 65 || midiNote == 77)
                {
                    return 3;
                }
                else if (midiNote == 31 || midiNote == 43 || midiNote == 55 || midiNote == 67 || midiNote == 79)
                {
                    return 4;
                }
                else if (midiNote == 33 || midiNote == 45 || midiNote == 57 || midiNote == 69 || midiNote == 81)
                {
                    return 5;
                }
                else if (midiNote == 35 || midiNote == 47 || midiNote == 59 || midiNote == 71 || midiNote == 83)
                {
                    return 6;
                }
                else
                {
                    throw new ArgumentException("Invalid MIDI note value");
                }
            }
        }

        public void SetGameplayScene()
        {
            m_GameplayUIController.SetActionBarActive(true);
            m_GameplayUIController.SetBarlineActive(true);
        }

        /*private void ActiveBarline(float currentTime)
        {
            if(HasBarlineToActive(currentTime) && !m_RaindropNoteController.RaindropBarlineList[m_CurrentBarlineIndex].isActiveAndEnabled)
            {
                m_RaindropNoteController.RaindropBarlineList[m_CurrentBarlineIndex].gameObject.SetActive(true);
                m_CurrentBarlineIndex++;
            }
        }*/

        /*private bool HasBarlineToActive(double currentTime)
        {
            return m_CurrentBarlineIndex < m_RaindropNoteController.RaindropBarlineList.Count &&
                   currentTime >= m_RaindropNoteController.RaindropBarlineList[m_CurrentBarlineIndex].EndScreenTimeInSecond;
        }*/

        public void SetBackgroundImage(Texture texture)
        {
        }

        public void SetMetronome(bool active)
        {
        }

        public void StartCutSceneAnimation()
        {
        }

        public void StartGameplayWithMusic()
        {
            StartGameplay();
        }

        public void ShowGameplayControllerHUD()
        {
        }

        public void OpenStudentStatusList()
        {
        }

        public void SetPreResultDialogActive(bool isActive)
        {
        }

        public void CloseStudentStatusList()
        {
        }

        public void OpenEndGameConfirmation()
        {
        }

        public void SetGameplayState(int state)
        {
        }

        bool IsPressing(int midiId) => m_MidiInputHashSet.Contains(midiId);
    }
}