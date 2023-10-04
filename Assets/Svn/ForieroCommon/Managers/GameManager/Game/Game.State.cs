using System;
using System.Collections.Generic;

public static partial class Game
{
    public static RunningState RunningState { get => State.runningState; set => State.runningState = value; }
    public static GameState GameState { get => State.gameState; set => State.gameState = value; }
    public static bool IsPlaying => RunningState is RunningState.Playing;
    public static bool IsPaused => RunningState is RunningState.Paused;

    public static void Pause() => RunningState = RunningState.Paused;
    public static void Pause(GameState g) { GameState = g; RunningState = RunningState.Paused; }
    public static void UnPause() => RunningState = RunningState.Playing;
    public static void UnPause(GameState g) { GameState = g; RunningState = RunningState.Playing; }
    
    public static class State
    {
        private static RunningState _runningState = RunningState.Playing;

        internal static RunningState runningState
        {
            get => _runningState;
            set
            {
                if(_runningState == RunningState.Paused && value == RunningState.Playing) OnUnPause?.Invoke();
                OnRunningStateChange?.Invoke(_runningState, value);
                if (_runningState != value)
                {
                    _runningState = value;
                    OnRunningStateChanged?.Invoke(value);
                    switch (value)
                    {
                        case RunningState.Playing: OnPlay?.Invoke(); break;
                        case RunningState.Paused: OnPause?.Invoke(); break;
                        case RunningState.Unknown: break;
                    }
                }
            }
        }
       
        public static Action<RunningState, RunningState> OnRunningStateChange;
        public static Action<RunningState> OnRunningStateChanged;
        
        public static Action OnPlay;
        public static Action OnPause;
        public static Action OnUnPause;
        
        private static GameState _gameState = GameState.Game;

        internal static GameState gameState
        {
            get => _gameState;
            set
            {
                OnGameStateChange?.Invoke(_gameState, value);
                if (_gameState != value)
                {
                    _gameState = value;
                    OnGameStateChanged?.Invoke(value);
                    switch (value)
                    {
                        case GameState.Game: OnGame?.Invoke(); break;
                        case GameState.Inventory: OnInventory?.Invoke(); break;
                        case GameState.Menu: OnMenu?.Invoke(); break;
                        case GameState.Tutorial: OnTutorial?.Invoke(); break;
                        case GameState.Cutscene: OnCutscene?.Invoke(); break;
                        case GameState.Unknown: break;
                    }
                }
            }
        }
        
        public static Action<GameState, GameState> OnGameStateChange;
        public static Action<GameState> OnGameStateChanged;
        
        public static Action OnGame;
        public static Action OnMenu;
        public static Action OnInventory;
        public static Action OnTutorial;
        public static Action OnCutscene;
    }
}

public enum RunningState
{
    Playing,
    Paused,
    Unknown = int.MaxValue
}

public enum GameState
{
    Game,
    Inventory,
    Menu,
    Tutorial,
    Cutscene,
    Unknown = int.MaxValue
}