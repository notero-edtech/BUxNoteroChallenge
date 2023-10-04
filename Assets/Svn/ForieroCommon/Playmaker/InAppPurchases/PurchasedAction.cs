using ForieroEngine.Purchasing;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory ("Foriero")]
	[Tooltip ("Purchased")]
	public class PurchasedAction: FsmStateAction
	{
		[RequiredField] public FsmString inAppId;
		[RequiredField] public FsmBool defaultValue;
		
		public FsmEvent trueEvent;
		public FsmEvent falseEvent;
		
		public override void Reset ()
		{
			inAppId = new FsmString (){ UseVariable = true };
			defaultValue = new FsmBool() { UseVariable = true };
			trueEvent = null;
			falseEvent = null;
		}

		public override void OnEnter ()
		{
			if (Store.Purchased (inAppId.Value, defaultValue.Value)) {
				if (trueEvent != null) Fsm.Event (trueEvent);	
			} else {
				if (falseEvent != null) Fsm.Event (falseEvent);
			}
			Finish ();
		}
	}
}