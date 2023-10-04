using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public static partial class Lang {

	//REMOVE THESE CHARS FROM SENTENCES//

	public static char[] specialCharactersToBeRemoved = new char[2] {
		'[', 	//	begin compound word //
		']',	//	end compound word //
	};

	public class Word{
		public int charIndex = -1;
		public string text = "";
	}

	public static List<Word> ParseWords(string aText, Lang.LanguageCode aLangCode){
		List<Word> words = new List<Word>();
		switch(aLangCode){
		default : 
			words = ParserDefault(aText);
		break;
		}
		return words;
	}

	private static List<Word> ParserDefault(string aText){
		List<Word> words = new List<Word>();

		List<char> word = new List<char>();
		int wordIndex = 0;
		
		char[] chars = aText.ToCharArray();

		bool compoundWord = false;

		int indexSub = 0;

		for(int i = 0; i<chars.Length; i++){
			char ch = chars[i];

			//removing compound characters from text//
			if(ch == '[') {
				compoundWord = true;
				indexSub++;
				continue;
			} else if(ch == ']') {
				compoundWord = false;
				indexSub++;
				continue;
			}

			if(compoundWord){
				word.Add (ch);
			} else {
				if ((char.IsWhiteSpace(ch) || char.IsPunctuation(ch)) && !ch.ToString().Equals("'")){
					if(word.Count > 0){
						Word w = new Word();
						w.charIndex = wordIndex;
						w.text = new string(word.ToArray());
						words.Add(w);
						word.Clear ();
					}
				} else {
					if (char.IsLetterOrDigit(ch) || ch.ToString().Equals("'")){
						word.Add (ch);
					}
				}
			}

			if(word.Count == 1) wordIndex = i - indexSub;
		}
		
		if(word.Count > 0){
			Word w = new Word();
			w.charIndex = wordIndex;
			w.text = new string(word.ToArray());
			words.Add(w);
		}
		word.Clear ();

		return words;
	}

	private static List<Word> ParserThai(string aText){
		List<Word> words = new List<Word>();

		return words;
	}

	private static List<Word> ParserChinese(string aText){
		List<Word> words = new List<Word>();

		return words;
	}

	private static List<Word> ParserJapanese(string aText){
		List<Word> words = new List<Word>();

		return words;
	}
}
