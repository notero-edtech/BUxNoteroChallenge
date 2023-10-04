using UnityEngine;
using System;
using System.Collections.Generic;

public class LangFontDefinition : ScriptableObject {

	[Serializable]
	public class FontDefinition{
		public Lang.LanguageCode langCode;
		public Font font;
		public Material fontMaterial;
		public int fontSize;
		public FontStyle fontStyle;
		public float scaleFactor = 1f;
	}

	public FontDefinition defaultFont;

	public List<FontDefinition> fonts;

	public Font GetFont(Lang.LanguageCode langCode){
		FontDefinition fd = GetFontDefinition(langCode);
		return fd.font;
	}

	public Material GetFontMaterial(Lang.LanguageCode langCode){
		FontDefinition fd = GetFontDefinition(langCode);
		return fd.fontMaterial;
	}

	public int GetFontSize(Lang.LanguageCode langCode){
		FontDefinition fd = GetFontDefinition(langCode);
		return fd.fontSize;
	}

	public FontStyle GetFontStyle(Lang.LanguageCode langCode){
		FontDefinition fd = GetFontDefinition(langCode);
		return fd.fontStyle;
	}

	public float GetScaleFactor(Lang.LanguageCode langCode){
		FontDefinition fd = GetFontDefinition(langCode);
		return fd.scaleFactor;
	}

	public FontDefinition GetFontDefinition(Lang.LanguageCode langCode){
		foreach(FontDefinition fd in fonts){
			if(langCode == fd.langCode){
				return fd;
			}
		}
		return defaultFont;
	}
}
