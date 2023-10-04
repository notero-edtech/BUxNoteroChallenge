using ForieroEngine.Purchasing;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory ("Foriero")]
	[Tooltip ("Purchase INAPP Product")]
	public class PurchaseAction: FsmStateAction
	{
		[RequiredField] public FsmString inAppId;
		public FsmEvent purchasedEvent;
		public FsmEvent failedEvent;
		
		public override void Reset ()
		{
			inAppId = new FsmString (){ UseVariable = true };
			purchasedEvent = null;
			failedEvent = null;
		}

		public override void OnEnter ()
		{
			Store.PurchaseProduct(inAppId.Value, Purchased, Failed);
		}

		void Purchased(string id, string receipt)
		{
			if (id == inAppId.Value && purchasedEvent != null)
			{
				Fsm.Event(purchasedEvent);
				Finish();
			} 
		}

		void Failed(string id, string reason)
		{
			if (id == inAppId.Value && failedEvent != null)
			{
				Fsm.Event(failedEvent);
				Finish();
			}
		}
	}
}