using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Localization")]
	[Tooltip("Sets Language.")]
	public class SetLanguage : FsmStateAction
	{
		[RequiredField]
		public Lang.LanguageCode langCode;

		
		public override void Reset()
		{
			langCode = Lang.LanguageCode.Unassigned;
		}

		public override void OnEnter()
		{
			Lang.selectedLanguage = langCode;
			Finish();
		}
	}
}