using Notero.RaindropGameplay.Core;
using Notero.RaindropGameplay.UI;
using Notero.Unity.MidiNoteInfo;
using UnityEngine;
using UnityEngine.UI;

namespace BU.Chainpunch.MidiGameplay.UI
{
    public class BackgroundTextFeedbackExample : BaseBackgroundFeedbackManager
    {
        [SerializeField]
        private RawImage m_BackgroundImage;

        [SerializeField]
        TextScrollFeedbackExample m_TextFeedbackUI;

        [SerializeField]
        TextScrollFeedbackExample m_TextScoreUI;

        [SerializeField]
        TextScrollFeedbackExample m_TextEventUI;

        private float m_BPM;
        private float m_TotalDuration;
        private float m_TimeSigNum;
        private float m_TimeSigDen;

        public override void OnGameplayStart()
        {
        }

        public override void OnGameplayEnd()
        {
        }

        public override void OnBlankKeyPressed(int midiId, double time)
        {
            var text = Instantiate(m_TextFeedbackUI, m_TextFeedbackUI.transform.position, Quaternion.identity, transform);
            text.gameObject.SetActive(true);
            text.SetText($"Blank pressing: note {midiId}");
        }

        public override void OnBlankKeyReleased(int midiId, double time)
        {
            var text = Instantiate(m_TextFeedbackUI, m_TextFeedbackUI.transform.position, Quaternion.identity, transform);
            text.gameObject.SetActive(true);
            text.SetText($"Blank released: note {midiId}");
        }

        public override void OnNoteInfoPressed(MidiNoteInfo noteInfo, double time)
        {
            var text = Instantiate(m_TextFeedbackUI, m_TextFeedbackUI.transform.position, Quaternion.identity, transform);
            text.gameObject.SetActive(true);
            text.SetText($"Note pressing: note {noteInfo.MidiId}");
        }

        public override void OnNoteInfoReleased(MidiNoteInfo noteInfo, double time)
        {
            var text = Instantiate(m_TextFeedbackUI, m_TextFeedbackUI.transform.position, Quaternion.identity, transform);
            text.gameObject.SetActive(true);
            text.SetText($"Note released: note {noteInfo.MidiId}");
        }

        public override void OnNoteInfoNoteStarted(MidiNoteInfo noteInfo, double time)
        {
            var text = Instantiate(m_TextEventUI, m_TextEventUI.transform.position, Quaternion.identity, transform);
            text.gameObject.SetActive(true);
            text.SetText($"Note starts: {noteInfo.MidiId}");
        }

        public override void OnNoteInfoNoteEnded(MidiNoteInfo noteInfo, double time)
        {
            var text = Instantiate(m_TextEventUI, m_TextEventUI.transform.position, Quaternion.identity, transform);
            text.gameObject.SetActive(true);
            text.SetText($"Note ends: {noteInfo.MidiId}");
        }

        public override void OnNoteTimingScore(MidiNoteInfo noteInfo, string result, string action)
        {
            var text = Instantiate(m_TextScoreUI, m_TextScoreUI.transform.position, Quaternion.identity, transform);
            text.gameObject.SetActive(true);
            text.SetText($"Note {noteInfo.MidiId} {action}: {result}");
        }

        public override void OnScoreUpdate(SelfResultInfo resultInfo)
        {
        }

        public override void SetBPM(float bpm)
        {
            m_BPM = bpm;
        }

        public override void OnBPMChange(float bpm)
        {
        }


        public override void SetGameDuration(float duration)
        {
            m_TotalDuration = duration;
        }

        public override void OnGameplayTimeUpdate(float time)
        {
        }

        public override void SetTimeSignature(float numerator, float denominator)
        {
            m_TimeSigNum = numerator;
            m_TimeSigDen = denominator;
        }

        public override void OnTimeSignatureChange(float numerator, float denominator)
        {
            SetTimeSignature(numerator, denominator);
        }

        public override void SetBackgroundImage(Texture texture)
        {
            var aspectRatio = (float)texture.width / texture.height;
            var bgAspectRatioFitter = m_BackgroundImage.GetComponent<AspectRatioFitter>();
            bgAspectRatioFitter.aspectRatio = aspectRatio;
            m_BackgroundImage.texture = texture;
        }
    }
}
