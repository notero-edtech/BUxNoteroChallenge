using Notero.RaindropGameplay.Core;
using Notero.Unity.MidiNoteInfo;
using UnityEngine;

namespace Notero.RaindropGameplay.UI
{
    public abstract class BaseBackgroundFeedbackManager : MonoBehaviour
    {
        #region Background
        public abstract void SetBackgroundImage(Texture texture);
        #endregion

        #region Score
        public abstract void OnNoteTimingScore(MidiNoteInfo noteInfo, string result, string action);
        public abstract void OnScoreUpdate(SelfResultInfo resultInfo);
        #endregion

        #region Time
        public abstract void SetGameDuration(float duration);
        public abstract void OnGameplayTimeUpdate(float time);
        #endregion

        #region Timing
        public abstract void SetBPM(float bpm);

        public abstract void SetTimeSignature(float numerator, float denominator);
        public abstract void OnBPMChange(float bpm);
        public abstract void OnTimeSignatureChange(float numerator, float denominator);
        #endregion

        #region Game State
        public abstract void OnGameplayStart();
        public abstract void OnGameplayEnd();
        #endregion

        #region Input
        public abstract void OnNoteInfoPressed(MidiNoteInfo noteInfo, double time);
        public abstract void OnNoteInfoReleased(MidiNoteInfo noteInfo, double time);
        public abstract void OnBlankKeyPressed(int midiId, double time);
        public abstract void OnBlankKeyReleased(int midiId, double time);
        #endregion

        #region Note on/off
        public abstract void OnNoteInfoNoteStarted(MidiNoteInfo noteInfo, double time);
        public abstract void OnNoteInfoNoteEnded(MidiNoteInfo noteInfo, double time);
        #endregion
    }
}
