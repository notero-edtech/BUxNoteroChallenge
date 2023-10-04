using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Foriero")]
	[Tooltip("Language Action")]
	public class ForieroLanguageAction : FsmStateAction
	{
		[CompoundArray("Languages","Language","Event")]
		public Lang.LanguageCode[] langs;
		public FsmEvent[] events;
		
						
		public override void Reset()
		{
			langs = null;
			events = null;
		}

		public override void OnEnter()
		{
			if(langs != null){
				for(int i = 0; i< langs.Length; i++){
					if(langs[i] == Lang.selectedLanguage){
						if(events[i] != null) {
							Fsm.Event(events[i]);
							break;
						}
					}
				}
				
			} 
			Finish();
		}
	}
}