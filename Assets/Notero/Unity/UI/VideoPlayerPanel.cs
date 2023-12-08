using Hendrix.Generic.UI.Elements;
using Hendrix.TheorySequence.UI;
using Notero.Unity.AudioModule;
using Notero.Unity.AudioModule.Core;
using Notero.Utilities;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Video;

namespace Notero.Unity.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class VideoPlayerPanel : MonoBehaviour, IPointerClickHandler
    {
        private const double PlayDelayTime = 0.200; // play delay time in seconds

        [SerializeField]
        private RectTransform m_VideoPlayerRectTransform;

        [SerializeField]
        private VideoPlayer m_VideoPlayer;

        [SerializeField]
        private TMP_Text m_VideoTime;

        [SerializeField]
        private SmoothProgressBar m_VideoProgress;

        [Header("Play/Pause Button")]
        [SerializeField]
        private UnityEngine.UI.Button m_PlayPauseButton;

        [SerializeField]
        private SpriteToggle m_PlayPauseButtonSpriteToggle;

        public event Action<Texture> UpdateClipSizeHandler;
        public UnityEvent<VideoControlCommand, double, long> OnVideoControlChangedEvent = new UnityEvent<VideoControlCommand, double, long>();

        public IReferenceTime ReferenceTime
        {
            set
            {
                m_ReferenceTime = value;
            }
        }

        public bool isMediaPlaying => m_VideoPlayer.isPlaying;

        private readonly Vector2 m_OnPanelOffsetMin = new Vector2(16, 16);
        private readonly Vector2 m_OnPanelOffsetMax = new Vector2(-16, 88);
        private readonly Vector2 m_FullScreenOffsetMin = new Vector2(32, 24); // left, bottom
        private readonly Vector2 m_FullScreenOffsetMax = new Vector2(-296, 96); // -right, -top
        // Video Player Height = Vector2 Top Value - Bottom Value | e.g. Top[96] Bottom[24] = Height 72, Top[88] Bottom [16] = Height 72 too.

        private const float m_StarterMaxProgressValue = 0;
        private const int m_FirstFrame = 0;
        private const float m_EndProgressGapSeconds = 1f;

        private AudioSpeaker m_AudioSpeaker;

        private CanvasGroup m_CanvasGroup;
        private IReferenceTime m_ReferenceTime;
        private Command m_CurrentCommand;

        public void SetActiveVideoUI(bool isActive)
        {
            m_PlayPauseButton.gameObject.SetActive(isActive);
            m_VideoProgress.gameObject.SetActive(isActive);
            m_VideoTime.gameObject.SetActive(isActive);
        }

        public void SetActive(bool isActive, string videoPath = "", bool autoPlayer = false)
        {
            gameObject.SetActive(isActive);

            if(isActive)
            {
                m_VideoPlayer.url = videoPath;
                m_VideoPlayer.skipOnDrop = true;
                m_VideoPlayer.timeReference = VideoTimeReference.ExternalTime;
                StartCoroutine(PrepareSliderValue());
                m_PlayPauseButton.onClick.RemoveAllListeners();
                m_PlayPauseButton.onClick.AddListener(OnPlayPauseButtonClick);
                ResetPlayer();
            }

            if(autoPlayer)
            {
                Play(executionTime: m_ReferenceTime.Time + PlayDelayTime, isPlayImmediately: false);
            }
        }

        private IEnumerator PrepareSliderValue()
        {
            if(!m_VideoPlayer.isPrepared)
            {
                m_VideoPlayer.Prepare();
            }

            yield return new WaitUntil(() => m_VideoPlayer.isPrepared);
            m_VideoProgress.maxValue = (float)m_VideoPlayer.length;
            m_VideoPlayer.Play();
            m_VideoPlayer.Pause();
        }

        public void SetVisualActive(bool isActive)
        {
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(isActive);
            }
        }

        public void SetToFullScreenPos(bool isFullScreen)
        {
            m_VideoPlayerRectTransform.offsetMin = isFullScreen ? m_FullScreenOffsetMin : m_OnPanelOffsetMin;
            m_VideoPlayerRectTransform.offsetMax = isFullScreen ? m_FullScreenOffsetMax : m_OnPanelOffsetMax;
        }

        public void ResetPlayer()
        {
            m_VideoProgress.value = 0;
            m_VideoPlayer.time = 0;
            m_VideoPlayer.frame = m_FirstFrame;
            SetVideoTimeText(0);
            SetToPlayButton();
        }

        public void Play()
        {
            Play(targetFrame: m_VideoPlayer.frame, executionTime: m_ReferenceTime.Time + PlayDelayTime, isPlayImmediately: false);
        }

        public void Play(double executionTime = 0, long targetFrame = 0, bool isPlayImmediately = true)
        {
            // Sometime this function has been called when the component did not enable yet, so we keep the command to be executed later
            if(!isActiveAndEnabled)
            {
                m_CurrentCommand = new Command(VideoControlCommand.Play, executionTime, targetFrame, isPlayImmediately);
                return;
            }

            executionTime = Max(0, executionTime);
            targetFrame = Max(0, targetFrame);

            OnVideoControlChangedEvent?.Invoke(VideoControlCommand.Play, executionTime, targetFrame);
            StartCoroutine(StartPlaying(executionTime, targetFrame, isPlayImmediately));
        }

        public void Pause()
        {
            Pause(targetFrame: m_VideoPlayer.frame);
        }

        public void Pause(long targetFrame = 0)
        {
            targetFrame = Max(0, targetFrame);

            m_VideoPlayer.Pause();
            m_VideoPlayer.frame = targetFrame;
            OnVideoControlChangedEvent?.Invoke(VideoControlCommand.Pause, m_ReferenceTime.Time, targetFrame);
            SetToPlayButton();
        }

        public void SetVideoFrame(long frame)
        {
            m_VideoPlayer.frame = frame;
        }

        public bool IsVideoFinished() => m_VideoPlayer.texture != null;

        public Texture GetVideoTexture() => m_VideoPlayer.texture;

        private void Awake()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();

            m_AudioSpeaker = AudioPlayer.Instance.GetAudioSpeaker();
            var mixerGroup = AudioPlayer.Instance.GetAudioMixer(AudioChannel.Master);
            var audioSource = m_AudioSpeaker.AudioSource;
            audioSource.outputAudioMixerGroup = mixerGroup;

            m_VideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            m_VideoPlayer.SetTargetAudioSource(0, audioSource);
        }

        private void OnDestroy()
        {
            AudioPlayer.Instance.ReturnAudioSpeaker(m_AudioSpeaker);
        }

        private void OnEnable()
        {
            if(m_ReferenceTime == null)
                ReferenceTime = new UnityEngineTime();

            m_VideoPlayer.Prepare();

            if(m_CurrentCommand != null)
            {
                Execute(m_CurrentCommand);
                m_CurrentCommand = null;
            }
        }

        private void Update()
        {
            m_VideoPlayer.externalReferenceTime = m_ReferenceTime.Time;

            double countdownTime = m_VideoPlayer.length - m_VideoPlayer.time;

            if(m_VideoPlayer.isPlaying)
            {
                if(m_VideoProgress.maxValue <= m_StarterMaxProgressValue)
                {
                    m_VideoProgress.maxValue = (float)m_VideoPlayer.length - m_EndProgressGapSeconds;
                    UpdateClipSizeHandler?.Invoke(m_VideoPlayer.texture);
                    m_VideoPlayer.frame = 0;
                }

                SetVideoTimeText(countdownTime);
                m_VideoProgress.value = (float)m_VideoPlayer.time;
            }

            //when video end
            if(m_VideoPlayer.frame > m_FirstFrame && Mathf.Floor((float)countdownTime) <= 0)
            {
                ResetPlayer();
                m_VideoPlayer.Pause();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 result;
            Vector2 clickPosition = eventData.position;
            RectTransform videoProgressRectTransform = m_VideoProgress.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(videoProgressRectTransform, clickPosition, null, out result);

            // calculate slider value by (difference between video progress bar rectTransform - click position / slider length) * video length 
            float sliderValue = (MathF.Abs(videoProgressRectTransform.rect.x - result.x) / videoProgressRectTransform.rect.width) * (float)m_VideoPlayer.length;
            m_VideoProgress.value = sliderValue;
            m_VideoPlayer.time = sliderValue;

            //set time text
            var videoTimeText = Mathf.Abs((float)m_VideoPlayer.length - sliderValue);
            SetVideoTimeText(videoTimeText);
        }

        private void OnPlayPauseButtonClick()
        {
            if(m_VideoPlayer.isPlaying)
            {
                Pause(targetFrame: m_VideoPlayer.frame);
            }
            else
            {
                Play(targetFrame: m_VideoPlayer.frame, executionTime: m_ReferenceTime.Time + PlayDelayTime, isPlayImmediately: false);
            }
        }

        private void SetToPlayButton() => m_PlayPauseButtonSpriteToggle.SetActive(false);

        private void SetToPauseButton() => m_PlayPauseButtonSpriteToggle.SetActive(true);

        private void SetVideoTimeText(double seconds) => m_VideoTime.text = DataFormatValidator.SecondsToTimeFormat(seconds);

        private IEnumerator StartPlaying(double executeTime, long targetFrame, bool isPlayImmediately)
        {
            m_CanvasGroup.interactable = false;
            if(!m_VideoPlayer.isPrepared)
            {
                m_VideoPlayer.Prepare();
            }

            yield return new WaitUntil(() => m_VideoPlayer.isPrepared);
            long frame = (long)((m_ReferenceTime.Time - executeTime) * m_VideoPlayer.frameRate) + targetFrame;
            if(frame <= 0 || !isPlayImmediately)
            {
                yield return new WaitUntil(() => m_ReferenceTime.Time >= executeTime);
                frame = Max(0, (long)((m_ReferenceTime.Time - executeTime) * m_VideoPlayer.frameRate) + targetFrame);
            }

            m_VideoPlayer.frame = frame;
            m_VideoPlayer.Play();

            yield return new WaitUntil(() => m_ReferenceTime.Time >= executeTime);
            m_CanvasGroup.interactable = true;
            SetToPauseButton();
        }

        private long Max(long a, long b)
        {
            return a > b ? a : b;
        }

        private double Max(double a, double b)
        {
            return a > b ? a : b;
        }

        private void Execute(Command command)
        {
            switch(command.Type)
            {
                case VideoControlCommand.Play:
                    Play(command.ExecutionTime, command.TargetFrame, command.IsExecuteImmediately);
                    break;
                default:
                    Pause(command.TargetFrame);
                    break;
            }
        }

        private class Command
        {
            public VideoControlCommand Type;
            public double ExecutionTime;
            public long TargetFrame;
            public bool IsExecuteImmediately;

            public Command(VideoControlCommand type, double executionTime = 0, long targetFrame = 0, bool isExecuteImmediately = true)
            {
                Type = type;
                ExecutionTime = executionTime;
                TargetFrame = targetFrame;
                IsExecuteImmediately = isExecuteImmediately;
            }
        }
    }
}

namespace Hendrix.TheorySequence.UI
{
    public interface IReferenceTime
    {
        public double Time { get; }
    }

    public class UnityEngineTime : IReferenceTime
    {
        public double Time => UnityEngine.Time.realtimeSinceStartupAsDouble;
    }
}