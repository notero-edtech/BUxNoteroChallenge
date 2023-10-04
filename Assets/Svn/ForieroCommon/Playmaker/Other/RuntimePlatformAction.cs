using UnityEngine;
using HutongGames.PlayMaker;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("Foriero")]
	[Tooltip ("")]
	public class RuntimePlatformAction: FsmStateAction
	{
		public static RuntimePlatform testPlatform = RuntimePlatform.IPhonePlayer;
		public FsmEvent ios;
		public FsmEvent android;
		public FsmEvent osx;
		public FsmEvent win;
		public FsmEvent winstore;

		public override void Reset ()
		{
			ios = null;
			android = null;
			osx = null;
			win = null;
			winstore = null;
		}

		public override void OnEnter ()
		{
			
			RuntimePlatform validatedPlatform;
			#if UNITY_EDITOR 
			validatedPlatform = testPlatform;
			#else 
			validatedPlatform = Application.platform;
			#endif
			switch (validatedPlatform) {
			case RuntimePlatform.Android:
				CallEvent (android);
				break;
			case RuntimePlatform.IPhonePlayer:
				CallEvent (ios);
				break;
			}
			Finish ();
		}

		void CallEvent (FsmEvent e)
		{
			if (e != null)
				Fsm.Event (e);
		}
	}
}