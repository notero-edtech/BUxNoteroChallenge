using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using FontAwesome;

[CustomEditor (typeof(FontAwesomeText))]
public class FontAwesomeTextEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		FontAwesomeText o = (FontAwesomeText)target;

		o.icon = (FontAwesomeIconEnum)EditorGUILayout.EnumPopup ("Icon", o.icon);

		if (!o.text) {
			o.text = o.gameObject.GetComponent<Text> ();
		}

		if (o.text.text != o.icon.ToFontAwesomeString ()) {
			o.text.text = o.icon.ToFontAwesomeString ();
			EditorUtility.SetDirty (o.text);
		}
	}
}
