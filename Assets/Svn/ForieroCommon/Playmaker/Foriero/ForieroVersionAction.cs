using UnityEngine;
using HutongGames.PlayMaker;
using ForieroEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("Foriero")]
	[Tooltip ("App Version")]
	public class ForieroVersionAction : FsmStateAction
	{
		
		public FsmEvent none;
		public FsmEvent free;
		public FsmEvent lite;
		public FsmEvent pro;

		public override void Reset ()
		{
			none = null;
			free = null;
			lite = null;
			pro = null;
		}

		public override void OnEnter ()
		{
			switch (Foriero.projectVersion) {
			case Foriero.ProjectVersion.NONE: if (none != null) Fsm.Event (none); break;
			case Foriero.ProjectVersion.FREE: if (free != null) Fsm.Event (free); break;
			case Foriero.ProjectVersion.LITE: if (lite != null) Fsm.Event (lite); break;
			case Foriero.ProjectVersion.PRO: if (pro != null) Fsm.Event (pro); break;
			default : Finish (); break;
			}
		}
	}
}