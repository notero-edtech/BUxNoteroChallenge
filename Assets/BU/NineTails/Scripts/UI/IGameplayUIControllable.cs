using BU.NineTails.Scripts.UI.VirtualPiano;
using Notero.MidiGameplay.Core;
using Notero.RaindropGameplay.Core;
using Notero.Unity.MidiNoteInfo;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BU.NineTails.MidiGameplay.UI
{
    public interface IGameplayUIControllable
    {
        public Canvas GameplayCanvas { get; }

        //Gameplay Runtime
        public void HandleGameplayTimeUpdate(float time);
        public void HandleScoreUpdate(SelfResultInfo info);

        //Feedback
        public void UpdateTextFeedback(MidiNoteInfo noteInfo, string result, string actionState);
        public void UpdatePianoFeedback(MidiNoteInfo noteInfo, string result, string actionState);
        public void UpdateTextFeedbackOnNoteEnd(MidiNoteInfo noteInfo, double time);
        public void UpdateFeedbackOnNoteStart(MidiNoteInfo noteInfo, double time);
        public void UpdateFeedbackBlankKeyPress(int midiId, double time);
        public void UpdateFeedbackBlankKeyRelease(int midiId, double time);
        public void SetupPianoFeedback(BaseVirtualPiano virtualPianoController, List<float> lanePosX);

        //State Flow
        public void SetCanvasScale();
        public void StartCutSceneAnimation();
        public void SetBackgroundImage(Texture texture);
        public void SetCountInActive(bool isActive, Action onFinished = null);
        public void SetCountInCallback(Action callback);

        //Player HUD
        public void SetupTimerDisplay(float GameplayDuration);
        public void SetupAccuracyMeterBar(GameplayConfig config);
        public void SetCompetitivePanelActive(bool isActive);
        public void SetTimerDisplayActive(bool isActive);
        public void SetScoreDisplayActive(bool isActive);
        public void SetAccuracyMeterBarActive(bool isActive);

        //Player UI
        public void SetVirtualPianoActive(bool isActive);
        public void SetActionBarActive(bool isActive);
        public void SetBarlineActive(bool isActive);


    }
}