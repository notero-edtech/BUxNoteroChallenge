// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Foriero")]
	[Tooltip("Sets the Position of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class CheckPosition : FsmStateAction
	{
		public enum EqualityEnum{
			GreaterEqual,
			LessEqual
		}
		
		[RequiredField]
		[Tooltip("The GameObject to position.")]
		public FsmOwnerDefault gameObject;
						
		public FsmFloat x;
		public EqualityEnum x_comparer;
		public FsmEvent x_reached;
		public FsmFloat y;
		public EqualityEnum y_comparer;
		public FsmEvent y_reached;
		public FsmFloat z;
		public EqualityEnum z_comparer;
		public FsmEvent z_reached;
				
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		[Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;
				
		GameObject go;
		
		public override void Reset()
		{
			gameObject = null;
			x = new FsmFloat { UseVariable = true };
			x_comparer = EqualityEnum.LessEqual;
			x_reached = null;
			y = new FsmFloat { UseVariable = true };
			y_comparer = EqualityEnum.LessEqual;
			y_reached = null;
			z = new FsmFloat { UseVariable = true };
			z_comparer = EqualityEnum.LessEqual;
			z_reached = null;
					
			everyFrame = false;
			lateUpdate = false;
		}

		public override void OnEnter()
		{
			go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				return;
			}
			
			if (!everyFrame && !lateUpdate)
			{
				DoCheckPosition();
				Finish();
			}		
		}

		public override void OnUpdate()
		{
			if (!lateUpdate)
			{
				DoCheckPosition();
			}
		}

		public override void OnLateUpdate()
		{
			if (lateUpdate)
			{
				DoCheckPosition();
			}

			if (!everyFrame)
			{
				Finish();
			}
		}

		void DoCheckPosition()
		{
			Vector3 v = go.transform.position;
			if(!x.IsNone){
				switch(x_comparer){
				case EqualityEnum.LessEqual:
					if(v.x <= x.Value) {
						if(x_reached != null) Fsm.Event(x_reached);	
					}
				break;
				case EqualityEnum.GreaterEqual:
					if(v.x >= x.Value) {
						if(x_reached != null) Fsm.Event(x_reached);	
					}
				break;
				}
			}
			
			if(!y.IsNone){
				switch(y_comparer){
				case EqualityEnum.LessEqual:
					if(v.y <= y.Value) {
						if(y_reached != null) Fsm.Event(y_reached);	
					}
				break;
				case EqualityEnum.GreaterEqual:
					if(v.y >= y.Value) {
						if(y_reached != null) Fsm.Event(y_reached);	
					}
				break;
				}
			}
			
			if(!z.IsNone){
				switch(z_comparer){
				case EqualityEnum.LessEqual:
					if(v.z <= z.Value) {
						if(z_reached != null) Fsm.Event(z_reached);	
					}
				break;
				case EqualityEnum.GreaterEqual:
					if(v.z >= z.Value) {
						if(z_reached != null) Fsm.Event(z_reached);	
					}
				break;
				}
			}
		}


	}
}