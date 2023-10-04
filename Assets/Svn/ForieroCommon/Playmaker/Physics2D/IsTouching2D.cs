// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Adds a 2d torque (rotational force) to a Game Object.")]
    public class IsTouching2D : ComponentAction<Collider2D>
	{
		[RequiredField]
		[CheckForComponent(typeof(Collider2D))]
		[Tooltip("The GameObject to add torque to.")]
		public FsmOwnerDefault gameObject;
		
		[RequiredField]
		[UIHint(UIHint.TagMenu)]
		[Tooltip("Filter by Tag.")]
		public FsmString colliderTag;
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmBool storeValue;

		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
		
		private GameObject _owner;
		private ContactFilter2D _filter;
		private Collider2D[] _collider2Ds;

        public override void OnPreprocess()
        {
            Fsm.HandleFixedUpdate = true;
        }

		public override void Reset()
		{
			gameObject = null;
			colliderTag = null;
			storeValue = new FsmBool() {UseVariable = true};
			everyFrame = false;
		}

		public override void OnEnter()
		{
			_owner = Fsm.GetOwnerDefaultTarget(gameObject);
			UpdateCache(_owner);
			_filter = new ContactFilter2D();
			_collider2Ds = new Collider2D[0];

			if (!everyFrame)
			{
				CheckTouching();
				Finish();
			}
		}
		
		public override void OnFixedUpdate()
		{
			CheckTouching();
		}
		
		void CheckTouching()
		{
			storeValue.Value = false;
			cachedComponent.OverlapCollider(_filter, _collider2Ds);
			foreach (var c in _collider2Ds)
			{
				if (TagMatches(colliderTag, c))
				{
					storeValue.Value = true;
					return;
				}
			}
		}
	}
}