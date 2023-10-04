using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class EditorHelper{
	public static Enum PopUp(string label, Enum e){
		var namesOrdered = Enum.GetNames(e.GetType()).OrderBy(x => x).ToArray();
		var selectedName = Enum.GetName(e.GetType(), e);
		var selectedIndex = Array.FindIndex<string>(namesOrdered, s => s == selectedName);
		
		var newIndex = EditorGUILayout.Popup(label, selectedIndex, namesOrdered);
		
		var result =  (Enum)Enum.Parse(e.GetType(), namesOrdered[newIndex]);
		
		return result;
	}
}