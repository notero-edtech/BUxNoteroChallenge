using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Localization")]
	[Tooltip("Get Language Text.")]
	public class DictionaryGetText : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmString variable;
		
		[RequiredField]
		[Tooltip("Alias name you have defined int Dictionary Init Action.")]
		public FsmString dictionary;
		
		[Tooltip("Record id.")]
		[RequiredField]
		public FsmString id;
		
		[Tooltip("Default value in the case the id or dictionary has not been found.")]
		public FsmString defaultValue;
		
		public override void Reset()
		{
			dictionary = new FsmString{ UseVariable = true };
			id = new FsmString{ UseVariable = false };
			defaultValue = new FsmString{ UseVariable = true };
		}

		public override void OnEnter()
		{
			variable.Value = Lang.GetText(dictionary.Value,
	                                      id.Value, 
										  defaultValue.IsNone ? "" : defaultValue.Value
	                                     );
			Finish();
			
		}
	}
}