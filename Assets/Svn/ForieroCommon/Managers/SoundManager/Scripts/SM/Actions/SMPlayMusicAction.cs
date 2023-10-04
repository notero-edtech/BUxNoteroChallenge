using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("Sound Manager")]
	[Tooltip ("Play Music")]
	public class SMPlayMusicAction: FsmStateAction
	{		
		public FsmString groupId;

		public FsmString musicId;
		public FsmFloat volume;

        public FsmBool force;

		public override void Reset ()
		{
			musicId = new FsmString{ UseVariable = true };
			groupId = new FsmString{ UseVariable = true };
			volume = new FsmFloat{ UseVariable = true };
            force = new FsmBool { UseVariable = true };
		}

		public override void OnEnter ()
		{
			if (!groupId.IsNone || !musicId.IsNone) {
				SM.PlayMusic (groupId.IsNone ? null : groupId.Value, groupId.IsNone ? null : musicId.Value, force.IsNone ? true : force.Value);
			}
			Finish ();
		}
	}
}