using Notero.MidiGameplay.Core;
using Notero.RaindropGameplay.Core;
using Notero.RaindropGameplay.UI;
using Notero.Unity.MidiNoteInfo;
using Notero.Unity.UI.VirtualPiano;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BU.MidiGameplay.UI
{
    public class GameplayUIConnector : MonoBehaviour, IGameplayUIControllable
    {
        [SerializeField]
        private Canvas m_GameplayCanvas;

        [SerializeField]
        private GameObject m_RaindropGameplayCanvas;

        [SerializeField]
        private CountInPanel m_CountInPanel;

        [SerializeField]
        private CompetitivePanel m_CompetitivePanel;

        [SerializeField]
        private GameObject m_BackgroundImage;

        [SerializeField]
        private TextFeedbackManager m_TextFeedback;

        [SerializeField]
        private PianoFeedbackManager m_PianoFeedback;

        [SerializeField]
        private GameObject m_VirtualPiano;

        [SerializeField]
        private GameObject m_Barline;

        [SerializeField]
        private GameObject m_ActionBar;

        [SerializeField]
        private GameObject m_OctaceBar;

        public Canvas GameplayCanvas => m_GameplayCanvas;

        public event Action MetronomeButtonClicked;

        public void HandleGameplayTimeUpdate(float time)
        {
            m_CompetitivePanel.OnGameplayTimeUpdate(time);
        }

        public void HandleScoreUpdate(SelfResultInfo info)
        {
            m_CompetitivePanel.OnScoreUpdated(info);
        }

        public void SetAccuracyMeterBarActive(bool isActive)
        {
            m_CompetitivePanel.SetAccuracyMeterActive(isActive);
        }

        public void SetActionBarActive(bool isActive)
        {
            m_ActionBar.SetActive(isActive);
        }

        public void SetBackgroundImage(Texture texture)
        {
            var aspectRatio = (float)texture.width / texture.height;
            var bgAspectRatioFitter = m_BackgroundImage.GetComponent<AspectRatioFitter>();
            var backgroundImage = m_BackgroundImage.GetComponent<RawImage>();

            bgAspectRatioFitter.aspectRatio = aspectRatio;
            backgroundImage.texture = texture;
        }

        public void SetBarlineActive(bool isActive)
        {
            m_Barline.SetActive(isActive);
            m_OctaceBar.SetActive(isActive);
        }

        public void SetCanvasScale()
        {
        }

        public void SetCompetitivePanelActive(bool isActive)
        {
            m_CompetitivePanel.SetActive(isActive);
        }

        public void SetCountInActive(bool isActive, Action onFinished = null)
        {
            m_CountInPanel.SetActive(isActive);

            if(isActive)
            {
                m_CountInPanel.OnCountInFinished += onFinished;
                m_CountInPanel.StartCount();
            }
        }

        public void SetCountInCallback(Action callback)
        {
            m_CountInPanel.OnCountInFinished += callback;
        }

        public void SetScoreDisplayActive(bool isActive)
        {
            m_CompetitivePanel.SetScoreActive(isActive);
        }

        public void SetTimerDisplayActive(bool isActive)
        {
            m_CompetitivePanel.SetTimerActive(isActive);
        }

        public void SetupAccuracyMeterBar(GameplayConfig config)
        {
            m_CompetitivePanel.SetupAccuracyMeterBar(config);
        }

        public void SetupTimerDisplay(float duration)
        {
            m_CompetitivePanel.SetupTimerDisplay(duration);
        }

        public void SetVirtualPianoActive(bool isActive)
        {
            m_VirtualPiano.SetActive(isActive);
        }

        public void StartCutSceneAnimation()
        {
        }

        public void UpdateTextFeedback(MidiNoteInfo noteInfo, string result, string actionState)
        {
            m_TextFeedback.UpdateFeedback(noteInfo, result, actionState);
        }

        public void UpdatePianoFeedback(MidiNoteInfo noteInfo, string result, string actionState)
        {
            m_PianoFeedback.UpdateFeedback(noteInfo, result, actionState);
        }

        public void SetupPianoFeedback(BaseVirtualPianoController virtualPianoController, List<float> lanePosX)
        {
            m_PianoFeedback.Init(virtualPianoController, lanePosX);
        }

        public void UpdateTextFeedbackOnNoteEnd(MidiNoteInfo noteInfo, double time)
        {
            m_TextFeedback.UpdateFeedbackOnNoteEnd(noteInfo);
            m_PianoFeedback.UpdateFeedbackOnNoteEnd(noteInfo);
        }

        public void UpdateFeedbackOnNoteStart(MidiNoteInfo noteInfo, double time)
        {

        }

        public void UpdateFeedbackBlankKeyPress(int midiId, double time)
        {
            m_PianoFeedback.UpdateFeedbackBlankKeyPress(midiId, time);
        }

        public void UpdateFeedbackBlankKeyRelease(int midiId, double time)
        {
            m_TextFeedback.UpdateFeedbackBlankKeyRelease(midiId, time);
            m_PianoFeedback.UpdateFeedbackBlankKeyRelease(midiId, time);
        }
    }
}