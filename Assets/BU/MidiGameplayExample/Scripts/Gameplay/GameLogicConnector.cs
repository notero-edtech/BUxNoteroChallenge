using Notero.MidiAdapter;
using Notero.MidiGameplay.Core;
using Notero.Unity.AudioModule;
using Notero.Unity.MidiNoteInfo;
using UnityEngine;
using UnityEngine.Events;

namespace BU.MidiGameplay.Gameplay
{
    public class GameLogicConnector : MonoBehaviour, IMidiGameLogic
    {
        public UnityEvent<float> OnGameplayTimeUpdate => GameLogicController.OnGameplayTimeUpdate;

        public UnityEvent OnGameplayEnd => GameLogicController.OnGameplayEnd;

        public UnityEvent<MidiNoteInfo, double> OnNoteInfoPressed => GameLogicController.OnNoteInfoPressed;

        public UnityEvent<MidiNoteInfo, double> OnNoteInfoReleased => GameLogicController.OnNoteInfoReleased;

        public UnityEvent<MidiNoteInfo, double> OnNoteInfoNoteEnded => GameLogicController.OnNoteInfoNoteEnded;

        public UnityEvent<MidiNoteInfo, double> OnNoteInfoNoteStarted => GameLogicController.OnNoteInfoNoteStarted;

        public UnityEvent<int, double> OnBlankKeyPressed => GameLogicController.OnBlankKeyPressed;

        public UnityEvent<int, double> OnBlankKeyReleased => GameLogicController.OnBlankKeyReleased;

        public IMidiGameLogic GameLogicController { get; private set; } = new GameLogicController();

        public UnityEvent OnGameplayStart => GameLogicController.OnGameplayStart;

        public bool IsPlaying => GameLogicController.IsPlaying;

        public void Dispose()
        {
            GameLogicController.Dispose();
        }

        public void End()
        {
            GameLogicController.End();
        }

        public void SetGameDuration(float songTimeInSeconds)
        {
            GameLogicController.SetGameDuration(songTimeInSeconds);
        }

        public void SetGameplayCanvas(Canvas canvas)
        {
            GameLogicController.SetGameplayCanvas(canvas);
        }

        public void SetGameplayControllers(IMusicNotationControllable raindropNoteController)
        {
            GameLogicController.SetGameplayControllers(raindropNoteController);
        }

        public void SetMidi(MidiFile midiFile, float bpm = 0)
        {
            GameLogicController.SetMidi(midiFile, bpm);
        }

        public void SetMidiInput(IMidiKeyCallable midiKeyCallable)
        {
            GameLogicController.SetMidiInput(midiKeyCallable);
        }

        public void SetMidiTimeOffset(float offset)
        {
            GameLogicController.SetMidiTimeOffset(offset);
        }

        public void SetMusic(IAdjustableAudioClip music)
        {
            GameLogicController.SetMusic(music);
        }

        public void SetNoteStartTimeOffset(double offset)
        {
            GameLogicController.SetNoteStartTimeOffset(offset);
        }

        public void SetTimeOffset(int errorsTimeMs, int perfectTimeMs)
        {
            GameLogicController.SetTimeOffset(errorsTimeMs, perfectTimeMs);
        }

        public void SetTimeProvider(ITimeProvider timeProvider)
        {
            GameLogicController.SetTimeProvider(timeProvider);
        }

        public void StartGameplay(bool withMusic)
        {
            GameLogicController.StartGameplay(withMusic);
        }

        public void UpdateGUI()
        {
            GameLogicController.UpdateGUI();
        }

        public void UpdateLogic()
        {
            GameLogicController.UpdateLogic();
        }
    }
}
