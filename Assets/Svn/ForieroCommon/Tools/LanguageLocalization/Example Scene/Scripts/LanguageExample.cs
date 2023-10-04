using UnityEngine;
using System.Collections;

public class LanguageExample : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	string text = "EMPTY";
	
	void OnGUI(){
		GUILayout.Label("Selected Language : " + Lang.selectedLanguage.ToString());
		if(GUILayout.Button("EN", GUILayout.Width(50))){
			Lang.selectedLanguage = Lang.LanguageCode.EN;
		}
		
		if(GUILayout.Button("DE", GUILayout.Width(50))){
			Lang.selectedLanguage = Lang.LanguageCode.DE;
		}
		
		
				
		GUI.BeginGroup(new Rect(Screen.width/2f - 200f, Screen.height/2 - 200f, 400, 200));
			GUILayout.Label("Localized Text : " + text);
			if(GUILayout.Button("001", GUILayout.Width(50))){
				text = Lang.GetText("Localization", "001", "EMPTY OR NOT FOUND - SELECT LANGUAGE");
			}
			if(GUILayout.Button("002", GUILayout.Width(50))){
				text = Lang.GetText("Localization", "002", "EMPTY OR NOT FOUND - SELECT LANGUAGE");
			}
			if(GUILayout.Button("003", GUILayout.Width(50))){
				text = Lang.GetText("Localization", "003", "EMPTY OR NOT FOUND - SELECT LANGUAGE");
			}
		GUI.EndGroup();
		
		GUI.Box(new Rect(Screen.width - 300, 0, 300, 130), 
			"\n" +
			"Open Menu->Foriero->Language Tool." + "\n" +
			"You should see there our example document." + "\n" + 
			"Click Download from GoogleDocs." + "\n" +
			"Click Update Camera." + "\n" +
			"You are ready to go :-)."
			);
	}
}