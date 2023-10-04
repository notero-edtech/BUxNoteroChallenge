using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Game State")]
	[Tooltip("")]
	public class GetGameStateAction: FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(GameState))]
		public FsmEnum gameState;
		
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(RunningState))]
		public FsmEnum runningState;
		
		public override void Reset()
		{
			runningState = new FsmEnum() {UseVariable = true};
			gameState = new FsmEnum() {UseVariable = true};
		}

		public override void OnEnter()
		{
			if(!gameState.IsNone) gameState.Value = Game.GameState;
			if(!runningState.IsNone) runningState.Value = Game.RunningState;
			Finish();
		}
	}
}