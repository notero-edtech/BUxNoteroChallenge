using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Game State")]
	[Tooltip("")]
	public class SetGameStateAction: FsmStateAction
	{
		[ObjectType(typeof(GameState))] public FsmEnum gameState;
		[ObjectType(typeof(RunningState))] public FsmEnum runningState;
		
		public override void Reset()
		{
			gameState = new FsmEnum() { UseVariable = true};;
			runningState = new FsmEnum() { UseVariable = true};;
		}

		public override void OnEnter()
		{
			if(!gameState.IsNone) Game.GameState = (GameState) gameState.Value;
			if(!runningState.IsNone) Game.RunningState = (RunningState) runningState.Value;
			Finish();
		}
	}
}