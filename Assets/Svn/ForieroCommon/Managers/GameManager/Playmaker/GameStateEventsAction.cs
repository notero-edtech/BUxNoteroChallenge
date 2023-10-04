namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Game State")]
	[Tooltip("")]
	public class GameStateEventsAction: FsmStateAction
	{
		[ActionSection("Running State")] 
		[ObjectType(typeof(RunningState))]
		public FsmEnum runningState;
		public FsmEvent onRunningState;
		
		[ActionSection("Game State")]
		[ObjectType(typeof(GameState))]
		public FsmEnum gameState;
		public FsmEvent onGameState;
		
		public override void Reset()
		{
			runningState = null;
			onRunningState = null;

			gameState = null;
			onGameState = null;
		}

		public override void OnEnter()
		{
			Game.State.OnGameStateChanged += OnGameStateChanged;
			Game.State.OnRunningStateChanged += OnRunningStateChanged;
		}

		private void OnRunningStateChanged(RunningState s)
		{
			if (!runningState.IsNone && (RunningState)runningState.Value == s && onRunningState != null)
			{
				Fsm.Event(onRunningState);
				Finish();;
			}
		}

		private void OnGameStateChanged(GameState s)
		{
			if (!gameState.IsNone && (GameState)gameState.Value == s && onGameState != null)
			{
				Fsm.Event(onGameState);
				Finish();;
			}
		}
		
		public override void OnExit(){
			Game.State.OnGameStateChanged -= OnGameStateChanged;
			Game.State.OnRunningStateChanged -= OnRunningStateChanged;
		}
	}
}