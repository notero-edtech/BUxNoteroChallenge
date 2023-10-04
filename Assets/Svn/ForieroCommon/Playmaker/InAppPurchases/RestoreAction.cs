using ForieroEngine.Purchasing;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory ("Foriero")]
	[Tooltip ("Restore INAPP Product")]
	public class RestoreAction: FsmStateAction
	{
		[RequiredField] public FsmString inAppId;
		public FsmEvent restoredEvent;
		
		public override void Reset ()
		{
			inAppId = new FsmString (){ UseVariable = true };
			restoredEvent = null;
		}

		public override void OnEnter ()
		{
			Store.RestoreProduct(inAppId.Value, Restored);
		}

		void Restored(string id, string receipt)
		{
			if (id == inAppId.Value && restoredEvent != null)
			{
				Fsm.Event(restoredEvent);
				Finish();
			}
		}
	}
}