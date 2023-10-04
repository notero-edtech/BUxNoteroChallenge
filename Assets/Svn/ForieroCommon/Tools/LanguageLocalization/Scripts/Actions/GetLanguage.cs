using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Localization")]
	[Tooltip("Gets language code name.")]
	public class GetLanguage : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmString storeResult;
				
		public override void Reset()
		{
			storeResult = null;
		}

		public override void OnEnter()
		{
			storeResult.Value = Lang.selectedLanguage.ToString();
			Finish();
		}
	}
}