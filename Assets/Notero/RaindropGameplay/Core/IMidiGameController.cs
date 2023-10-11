using Notero.MidiGameplay.Bot;
using Notero.MidiGameplay.Core;
using Notero.Unity.AudioModule;
using Notero.Unity.MidiNoteInfo;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Hendrix.Gameplay.Core
{
    public interface IMidiGameController
    {
        IMidiGameLogic GameLogic { get; }
        IBotControllable BotControllable { get; }
        bool IsMusicExists { get; }
        bool IsMusicLoaded { get; }
        bool IsControllerReady { get; }

        UnityEvent OnGameEnded { get; }
        UnityEvent OnGameStarted { get; }
        UnityEvent OnMusicLoadingCompleted { get; }
        UnityEvent<SelfResultInfo> OnGameResultSubmitted { get; }
        GameplayModeController ModeController { get; }

        double NoteStartTimeOffset { get; }

        #region Loading
        void Setup(MidiFile midiFile, IAdjustableAudioClip music, float midiTimeOffset = 0, float customBPM = 0);
        void Initial();
        void SetMetronome(bool active);
        void SelectMode(GameplayMode mode);
        void SetBackgroundImage(Texture texture);
        #endregion

        #region Cooldown
        void CreateVirtualPiano();
        void StartCutSceneAnimation();
        #endregion

        #region CountIn
        void SetCountInCallback(Action callback);
        void SetCompetitivePanelActive(bool isActive);
        void SetCountInActive(bool isActive);
        void SetGameplayOverlayActive(bool isActive);
        void SetupGameLogicController();
        void SetupScoringController();
        #endregion

        #region Start
        void StartGameplayWithMusic();
        void ShowGameplayControllerHUD();
        #endregion

        #region Play
        void ResetController();
        #endregion

        #region End
        void OpenStudentStatusList();
        void ClearGameplayComponents();
        #endregion

        #region Pre-result
        void SetPreResultDialogActive(bool isActive);
        #endregion

        #region Result
        void CloseStudentStatusList();
        #endregion

        #region Gameplay App State
        void OpenEndGameConfirmation();
        void SetGameplayState(int state);
        #endregion
    }
}