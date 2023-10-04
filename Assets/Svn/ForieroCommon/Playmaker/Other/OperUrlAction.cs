using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Foriero")]
	[Tooltip("OpenUrl")]
	public class OpenUrlAction: FsmStateAction
	{
		[RequiredField]
		public FsmString url;
				
		public override void Reset()
		{
			url = new FsmString(){UseVariable = true};
		}

		public override void OnEnter()
		{
			Debug.Log(url.Value);
			Application.OpenURL(url.Value);
			Finish();
		}
	}
}