// (c) Copyright HutongGames. All rights reserved.

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("UI - Interfaces")]
	[Tooltip("Control IUIPanelEvents interface.")]
	public class IUIPanelEventsAction : FsmStateAction
	{
		public enum Command
		{
			Show,
			Hide
		}
		
		[RequiredField] 
		[ObjectType(typeof(IUIPanelEvents))]
		public FsmOwnerDefault gameObject;
		
		public Command command;

		public override void Reset()
		{
			gameObject = null;
			command = Command.Show;
		}

		public override void OnEnter()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			var e = go.GetComponent<IUIPanelEvents>();
			switch (command)
			{
				case Command.Show: e?.OnShow(); break;
				case Command.Hide: e?.OnHide(); break;
			}
			Finish();
		}
	}
}