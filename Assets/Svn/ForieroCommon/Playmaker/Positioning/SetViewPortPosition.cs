// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Foriero")]
	[Tooltip("Sets the Position of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class SetViewPortPosition : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to position.")]
		public FsmOwnerDefault gameObject;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Use a stored Vector3 position, and/or set individual axis below.")]
		public FsmVector3 vector;
		
		public FsmFloat x;
		public FsmFloat y;
		public FsmFloat z;

		[Tooltip("Use local or world space.")]
		public Space space;
		
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		[Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;	

		public override void Reset()
		{
			gameObject = null;
			vector = null;
			// default axis to variable dropdown with None selected.
			x = new FsmFloat { UseVariable = true };
			y = new FsmFloat { UseVariable = true };
			z = new FsmFloat { UseVariable = true };
			space = Space.Self;
			everyFrame = false;
			lateUpdate = false;
		}

		public override void OnEnter()
		{
			if (!everyFrame && !lateUpdate)
			{
				DoSetPosition();
				Finish();
			}		
		}

		public override void OnUpdate()
		{
			if (!lateUpdate)
			{
				DoSetPosition();
			}
		}

		public override void OnLateUpdate()
		{
			if (lateUpdate)
			{
				DoSetPosition();
			}

			if (!everyFrame)
			{
				Finish();
			}
		}

		void DoSetPosition()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				return;
			}
			
			Vector3 v;
			if(vector.IsNone){
				v = new Vector3(x.IsNone ? 0 : x.Value, y.IsNone ? 0 : y.Value, z.IsNone ? 0 : z.Value);
			} else {
				v = vector.Value;
			}
			v = Camera.main.ViewportToWorldPoint(new Vector3(v.x, v.y, Vector3.Distance(Camera.main.transform.position, new Vector3(0, 0, v.z))));
			go.transform.position = v;
		}


	}
}