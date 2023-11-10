using Notero.RaindropGameplay.Core;
using Notero.RaindropGameplay.UI;
using Notero.Unity.MidiNoteInfo;
using UnityEngine;
using UnityEngine.UI;

namespace BU.MidiGameplay.UI
{
    public class BackgroundStaticExample : BaseBackgroundFeedbackManager
    {
        [SerializeField]
        private RawImage m_BackgroundImage;

        public override void SetBackgroundImage(Texture texture)
        {
            var aspectRatio = (float)texture.width / texture.height;
            var bgAspectRatioFitter = m_BackgroundImage.GetComponent<AspectRatioFitter>();
            bgAspectRatioFitter.aspectRatio = aspectRatio;
            m_BackgroundImage.texture = texture;
        }

        public override void OnBlankKeyPressed(int midiId, double time)
        {
        }

        public override void OnBlankKeyReleased(int midiId, double time)
        {
        }

        public override void OnBPMChange(float bpm)
        {
        }

        public override void OnGameplayEnd()
        {
        }

        public override void OnGameplayStart()
        {
        }

        public override void OnGameplayTimeUpdate(float time)
        {
        }

        public override void OnNoteInfoNoteEnded(MidiNoteInfo noteInfo, double time)
        {
        }

        public override void OnNoteInfoNoteStarted(MidiNoteInfo noteInfo, double time)
        {
        }

        public override void OnNoteInfoPressed(MidiNoteInfo noteInfo, double time)
        {
        }

        public override void OnNoteInfoReleased(MidiNoteInfo noteInfo, double time)
        {
        }

        public override void OnNoteTimingScore(MidiNoteInfo noteInfo, string result, string action)
        {
        }

        public override void OnScoreUpdate(SelfResultInfo resultInfo)
        {
        }

        public override void OnTimeSignatureChange(float numerator, float denominator)
        {
        }

        public override void SetBPM(float bpm)
        {
        }

        public override void SetGameDuration(float duration)
        {
        }

        public override void SetTimeSignature(float numerator, float denominator)
        {
        }
    }
}
