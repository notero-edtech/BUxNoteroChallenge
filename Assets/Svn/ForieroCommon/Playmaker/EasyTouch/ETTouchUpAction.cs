using UnityEngine;
using HutongGames.PlayMaker;
using HedgehogTeam.EasyTouch;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("EasyTouch")]
	[Tooltip ("TouchUp")]
	public class ETTouchUpAction: FsmStateAction
	{
		[RequiredField]
		[UIHint (UIHint.Variable)]
		public FsmOwnerDefault gameObject;
				
		public FsmEvent onTouchUp;
		public FsmEvent onTouchDown;
		
		[UIHint (UIHint.Variable)]
		public FsmGameObject pickObject;
		
		GameObject go;

		public override void Reset ()
		{
			gameObject = null;
			onTouchUp = null;
			onTouchDown = null;
			pickObject = new FsmGameObject (){ UseVariable = true };
		}

		public override void OnEnter ()
		{
			go = Fsm.GetOwnerDefaultTarget (gameObject);
			if (go == null) {
				Finish ();
				return;
			}
			EasyTouch.On_TouchUp += TouchUp;
			EasyTouch.On_TouchDown += TouchDown;
		}

		public override void OnExit ()
		{
			EasyTouch.On_TouchUp -= TouchUp;
			EasyTouch.On_TouchDown -= TouchDown;
		}

		void TouchUp (Gesture gesture)
		{
			if (!pickObject.IsNone)
				pickObject.Value = gesture.pickedObject;
			if (onTouchUp != null)
				Fsm.Event (onTouchUp);
			
		}

		void TouchDown (Gesture gesture)
		{
			if (!pickObject.IsNone)
				pickObject.Value = gesture.pickedObject;
			if (onTouchDown != null)
				Fsm.Event (onTouchDown);
		}
		
		
		
	}
}