using Notero.MidiAdapter;
using Notero.Unity.AudioModule;
using Notero.Unity.MidiNoteInfo;
using UnityEngine;
using UnityEngine.Events;

namespace Notero.MidiGameplay.Core
{
    public interface IMidiGameLogic
    {
        public UnityEvent<float> OnGameplayTimeUpdate { get; }
        public UnityEvent OnGameplayStart { get; }
        public UnityEvent OnGameplayEnd { get; }
        public UnityEvent<MidiNoteInfo, double> OnNoteInfoPressed { get; } // send info, action time.
        public UnityEvent<MidiNoteInfo, double> OnNoteInfoReleased { get; }
        public UnityEvent<MidiNoteInfo, double> OnNoteInfoNoteEnded { get; }
        public UnityEvent<MidiNoteInfo, double> OnNoteInfoNoteStarted { get; }
        public UnityEvent<int, double> OnBlankKeyPressed { get; }
        public UnityEvent<int, double> OnBlankKeyReleased { get; }

        #region State
        public bool IsPlaying { get; }
        #endregion

        #region Game Flow Methods
        public void UpdateLogic();
        public void UpdateGUI();
        public void StartGameplay(bool withMusic, float speedMultiplier = 1);
        public void End();
        public void Dispose();
        #endregion

        #region Attaching Components
        public void SetGameplayCanvas(Canvas canvas);
        public void SetGameplayControllers(IMusicNotationControllable raindropNoteController);
        public void SetMidiInput(IMidiKeyCallable midiKeyCallable);
        public void SetTimeProvider(ITimeProvider timeProvider);
        #endregion

        #region Setting Game Parameter
        public void SetTimeOffset(int errorsTimeMs, int perfectTimeMs);
        public void SetGameDuration(float songTimeInSeconds);
        public void SetMidi(MidiFile midiFile, float bpm = default);
        public void SetMusic(IAdjustableAudioClip music);
        public void SetMidiTimeOffset(float offset);
        public void SetNoteStartTimeOffset(double offset);
        #endregion
    }
}