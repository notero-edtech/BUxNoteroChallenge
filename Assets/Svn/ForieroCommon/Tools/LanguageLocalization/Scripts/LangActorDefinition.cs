using UnityEngine;
using System;
using System.Collections.Generic;

public class LangActorDefinition : ScriptableObject {
	
	[Serializable]
	public class ActorDefinition{
		public string actor = "";
		public List<string> dictionaries = new List<string>();
		public List<ActorLangDefinition> languages = new List<ActorLangDefinition>();
	}

	[Serializable]
	public class ActorLangDefinition{
		public bool generate = false;
		public Lang.LanguageCode langCode;
        public VoiceService voiceService;
        public Voice voice;
	}

	public List<ActorDefinition> actors = new List<ActorDefinition>();

}
