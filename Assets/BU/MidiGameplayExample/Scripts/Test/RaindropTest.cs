using Hendrix.Gameplay.Core;
using Notero.Unity.AudioModule;
using Notero.Unity.MidiNoteInfo;
using Notero.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BU.MidiGameplay.Gameplay
{
    public class RaindropTest : MonoBehaviour
    {
        [SerializeField]
        GameObject m_GameControllerPrefab;

        [Header("UI Components")]
        [SerializeField]
        private Button m_StartGameplayStateButton;

        [SerializeField]
        private Transform m_StartGameplayPanel;

        [SerializeField]
        private float m_ActionDelaySec;

        [Header("MIDI Config")]
        [SerializeField]
        private TextAsset m_MidiFile;

        [SerializeField]
        private AudioClip m_BackingTrack;

        [SerializeField]
        private float m_MidiTimeOffset;

        [Header("Mode")]
        [SerializeField]
        private GameplayMode m_GameMode;

        private IMidiGameController m_RaindropGameController;
        private MidiFile m_CurrentMidiFile;
        private bool m_IsBackingTrackAvailable => m_BackingTrack != null;
        GameObject m_game;

        public void Awake()
        {
            AudioPlayer.Instance.MasterVolume = 1F;
            AudioPlayer.Instance.BGMVolume = 1F;
            AudioPlayer.Instance.SFXVolume = 1F;

            m_StartGameplayStateButton.onClick.AddListener(StartTest);

            ApplicationFlagConfig.InitializeCommandlineArgs();
        }

        private void OnDestroy()
        {
            m_StartGameplayStateButton.onClick.RemoveListener(StartTest);
        }

        public void StartTest()
        {
            m_game = Instantiate(m_GameControllerPrefab);
            m_RaindropGameController = m_game.GetComponentInChildren<IMidiGameController>();

            OnLoadingGameplayState(m_RaindropGameController);
            OnCoolDownGameplayState(m_RaindropGameController);

            m_RaindropGameController.SetCountInCallback(OnCountInFinished); //Simulate state change.

            OnCountInGameplayState(m_RaindropGameController);

            m_StartGameplayPanel.gameObject.SetActive(false);
        }

        void OnCountInFinished()
        {
            m_RaindropGameController.SetCountInActive(false);

            OnStartGameplayState(m_RaindropGameController);
            OnPlayGameplayState(m_RaindropGameController);
        }

        private void OnGameplayEnd()
        {
            OnEndGameplayState(m_RaindropGameController);

            m_StartGameplayPanel.gameObject.SetActive(true);

            Destroy(m_game);
            m_game = null;
        }

        private void OnGameResultSubmit(SelfResultInfo resultInfo)
        {
            Debug.Log(resultInfo.ToString());
            m_RaindropGameController.OnGameResultSubmitted.RemoveListener(OnGameResultSubmit);
        }

        private IEnumerator WaitForRaindropSpawn()
        {
            float waitTime = m_IsBackingTrackAvailable ? m_MidiTimeOffset : (float)m_RaindropGameController.NoteStartTimeOffset / 1000;
            yield return new WaitForSeconds(waitTime + m_ActionDelaySec);
        }

        private void OnLoadingGameplayState(IMidiGameController gameController)
        {
            m_CurrentMidiFile = new MidiFile(m_MidiFile.bytes);

            gameController.Setup(m_CurrentMidiFile, new BackingTrack(m_BackingTrack), m_MidiTimeOffset);
            gameController.Initial();
            gameController.SelectMode(m_GameMode);
        }

        private void OnCoolDownGameplayState(IMidiGameController gameController)
        {
            gameController.CreateVirtualPiano();
            gameController.StartCutSceneAnimation();
        }

        private void OnCountInGameplayState(IMidiGameController gameController)
        {
            gameController.SetupGameLogicController();
            gameController.SetupScoringController();
            gameController.SetGameplayOverlayActive(true);
            gameController.SetCompetitivePanelActive(true);
            gameController.SetCountInActive(true);

        }

        private void OnStartGameplayState(IMidiGameController gameController)
        {
            gameController.StartGameplayWithMusic();
        }

        private void OnPlayGameplayState(IMidiGameController gameController)
        {
            gameController.OnGameEnded.AddListener(OnGameplayEnd);
            gameController.OnGameResultSubmitted.AddListener(OnGameResultSubmit);
            StartCoroutine(WaitForRaindropSpawn());
        }

        private void OnEndGameplayState(IMidiGameController gameController)
        {
            gameController.ResetController();
            gameController.ClearGameplayComponents();
            gameController.OnGameEnded.RemoveListener(OnGameplayEnd);
        }
    }

    public class BackingTrack : IAdjustableAudioClip
    {
        public AudioClip AudioClip
        {
            get => m_AudioClip;
            set => m_AudioClip = value;
        }

        public float Volume
        {
            get => Mathf.Clamp01(m_Volume);
            set => m_Volume = Mathf.Clamp01(value);
        }

        public bool IsLoop
        {
            get => m_IsLoop;
            set => m_IsLoop = value;
        }

        public AudioChannel Channel
        {
            get => m_Channel;
            set => m_Channel = value;
        }

        public AudioDataLoadState LoadState => m_AudioClip == null ? AudioDataLoadState.Unloaded : m_AudioClip.loadState;

        private AudioClip m_AudioClip;
        private float m_Volume = 1F;
        private bool m_IsLoop;
        private AudioChannel m_Channel = AudioChannel.Music;

        public BackingTrack(AudioClip audioClip)
        {
            m_AudioClip = audioClip;
        }
    }
}
