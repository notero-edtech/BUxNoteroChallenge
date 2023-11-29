using BU.Chainpunch.Gameplay.Scoring;
using Notero.MidiAdapter;
using Notero.MidiGameplay.Bot;
using Notero.MidiGameplay.Core;
using Notero.RaindropGameplay.Core;
using Notero.RaindropGameplay.Core.Scoring;
using Notero.RaindropGameplay.UI;
using Notero.Unity.AudioModule;
using Notero.Unity.MidiNoteInfo;
using Notero.Unity.UI.VirtualPiano;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace BU.Chainpunch.MidiGameplay.Gameplay
{
    public class DemoRaindropGameController : MonoBehaviour, IMidiGameController, IBackgroundFeedbackChangable
    {
        [SerializeField]
        protected GameLogicConnector m_GameLogicController;

        [SerializeField]
        protected GameplayConfig m_GameplayConfig;

        [SerializeField] private VisualEffectController _visualEffectController; 

        [SerializeField]
        protected GameObject m_TimeProviderGO;
        protected ITimeProvider m_TimeProvider;

        [SerializeField]
        protected BaseVirtualPianoController m_VirtualPianoController;

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
        protected RectTransform m_ActionBar;

        public BaseScoringProcessor m_ScoringController;
        
        protected BaseBackgroundFeedbackManager m_BackgroundFeedbackManager;
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
        public bool IsControllerReady => m_VirtualPianoController != null && m_ScoringController != null;

        protected HashSet<int> m_MidiInputHashSet = new();

        public UnityEvent OnMusicLoadingCompleted { get; } = new();
        public UnityEvent OnGameEnded { get; } = new();
        public UnityEvent OnGameStarted { get; } = new();
        public UnityEvent<SelfResultInfo> OnGameResultSubmitted { get; } = new();

        private void Awake()
        {
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

            if(m_CurrentBackgroundTexture != null) m_BackgroundFeedbackManager.SetBackgroundImage(m_CurrentBackgroundTexture);

            m_RaindropNoteController.Init(SpawnPointPos);
            m_GameplayUIController.SetupPianoFeedback(m_VirtualPianoController, m_RaindropNoteController.LanePosList);

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
            _visualEffectController.m_ScoringController = m_ScoringController;
            _visualEffectController.Setup();
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
            GameLogic.OnGameplayTimeUpdate.AddListener(ActiveBarline);
            GameLogic.OnGameplayTimeUpdate.AddListener(m_RaindropNoteController.UpdateLogic);
        }

        private void UnsubscribeGameplayTimeUpdateEventHandlers()
        {
            GameLogic.OnGameplayTimeUpdate.RemoveAllListeners();
        }

        private void OnNoteStarted(MidiNoteInfo note, double time)
        {
            m_BackgroundFeedbackManager?.OnNoteInfoNoteStarted(note, time);

            if(!IsPressing(note.MidiId))
            {
                Handside hand = HandIdentifier.GetHandsideByTrackIndex(note.TrackIndex);
                m_VirtualPianoController.SetCueIn(note.MidiId, hand, false);
            }

            m_GameplayUIController.UpdateFeedbackOnNoteStart(note, time);
        }

        private void OnNoteEnded(MidiNoteInfo note, double time)
        {
            m_BackgroundFeedbackManager?.OnNoteInfoNoteEnded(note, time);
            
            if(!IsPressing(note.MidiId)) m_VirtualPianoController.SetDefault(note.MidiId, false);

            m_GameplayUIController.UpdateTextFeedbackOnNoteEnd(note, time);
        }

        private void OnNotePressed(MidiNoteInfo note, double time)
        {
            var midiId = note.MidiId;

            Assert.IsFalse(m_MidiInputHashSet.Contains(midiId), $"Duplicate key press without release on note: {midiId}");
            m_MidiInputHashSet.Add(midiId);

            m_ScoringController.ProcessNotePressTiming(note, time);
            m_BackgroundFeedbackManager?.OnNoteInfoPressed(note, time);
            Handside hand = HandIdentifier.GetHandsideByTrackIndex(note.TrackIndex);
            m_VirtualPianoController.SetCueIn(midiId, hand, true);
            m_RaindropNoteController.SetCorrect(note.RaindropNoteId);

            var score = m_ScoringController.CalculateTimingScore(note, time, true);
            m_BackgroundFeedbackManager?.OnNoteTimingScore(note, score.ToString(), "Press");
        }

        private void OnNoteReleased(MidiNoteInfo note, double time)
        {
            m_ScoringController.ProcessNoteReleaseTiming(note, time);
            m_BackgroundFeedbackManager?.OnNoteInfoReleased(note, time);

            const bool isPressing = false;
            int midiId = note.MidiId;

            if(IsPressing(midiId))
            {
                m_MidiInputHashSet.Remove(midiId);
            }

            if(note.IsPressed)
            {
                var score = m_ScoringController.CalculateTimingScore(note, time, false);
                m_BackgroundFeedbackManager?.OnNoteTimingScore(note, score.ToString(), "Release");

                if(m_RaindropNoteController.IsCueShowing(note.RaindropNoteId) && score != NoteTimingScore.Perfect)
                {
                    m_RaindropNoteController.SetMiss(note.RaindropNoteId);
                    m_VirtualPianoController.SetMissKey(midiId, isPressing);
                    m_GameplayUIController.UpdatePianoFeedback(note, score.ToString(), "Release");
                    m_GameplayUIController.UpdateTextFeedback(note, score.ToString(), "Release");
                }
                else
                {
                    m_VirtualPianoController.SetDefault(midiId, isPressing);
                }
            }
            else
            {
                Handside hand = HandIdentifier.GetHandsideByTrackIndex(note.TrackIndex);
                m_VirtualPianoController.SetCueIn(midiId, hand, isPressing);

            }
        }

        private void OnBlankKeyPressed(int midiId, double time)
        {
            Assert.IsFalse(m_MidiInputHashSet.Contains(midiId), $"Duplicate key press without release on note: {midiId}");
            m_MidiInputHashSet.Add(midiId);

            if(!GameLogic.IsPlaying)
            {
                m_VirtualPianoController.SetDefault(midiId, true);
                return;
            }

            m_ScoringController.ProcessBlankKeyPress(midiId, time);
            m_BackgroundFeedbackManager?.OnBlankKeyPressed(midiId, time);
            m_GameplayUIController.UpdateFeedbackBlankKeyPress(midiId, time);
            m_VirtualPianoController.SetMissKey(midiId, true);
        }

        private void OnBlankKeyReleased(int midiId, double time)
        {
            if(IsPressing(midiId))
            {
                m_MidiInputHashSet.Remove(midiId);
            }

            if(!GameLogic.IsPlaying)
            {
                m_VirtualPianoController.SetDefault(midiId, false);
                return;
            }

            m_ScoringController.ProcessBlankKeyRelease(midiId, time);

            m_GameplayUIController.UpdateFeedbackBlankKeyRelease(midiId, time);
            m_VirtualPianoController.SetDefault(midiId, false);
        }

        public void CreateVirtualPiano() => m_VirtualPianoController.Create("Gameplay_Test");

        public void SetGameplayOverlayActive(bool isActive)
        {
            if(m_RaindropNoteController == null) return;

            m_RaindropNoteController.SetBarOverlayActive(isActive);

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
            if(m_VirtualPianoController != null) m_VirtualPianoController.DeleteAllPianoKeys();

            m_RaindropNoteController?.ResetCues();

            SetCompetitivePanelActive(false);
            SetGameplayOverlayActive(false);
            m_GameplayUIController.SetActionBarActive(false);
        }

        protected virtual double CalculateNoteStartTimeOffset(float noteSpeed)
        {
            var destination = m_ActionBar.anchoredPosition.y;
            var origin = ((RectTransform)m_RaindropNoteController.RaindropNoteSpawner.transform).anchoredPosition.y;
            float distance = Mathf.Abs(destination - origin);
            return distance / noteSpeed * 1000;
        }

        public void SetGameplayScene()
        {
            m_GameplayUIController.SetActionBarActive(true);
            m_GameplayUIController.SetBarlineActive(true);
        }

        private void ActiveBarline(float currentTime)
        {
            if(HasBarlineToActive(currentTime) && !m_RaindropNoteController.RaindropBarlineList[m_CurrentBarlineIndex].isActiveAndEnabled)
            {
                m_RaindropNoteController.RaindropBarlineList[m_CurrentBarlineIndex].gameObject.SetActive(true);
                m_CurrentBarlineIndex++;
            }
        }

        private bool HasBarlineToActive(double currentTime)
        {
            return m_CurrentBarlineIndex < m_RaindropNoteController.RaindropBarlineList.Count &&
                   currentTime >= m_RaindropNoteController.RaindropBarlineList[m_CurrentBarlineIndex].EndScreenTimeInSecond;
        }

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

        public void SetBackgroundFeedback(BaseBackgroundFeedbackManager manager)
        {
            m_BackgroundFeedbackManager = manager;
        }
    }
}