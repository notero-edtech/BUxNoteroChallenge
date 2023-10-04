/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CanEditMultipleObjects, CustomEditor (typeof(NSEmptyRaycastTarget), false)]
public class NSEmptyRaycastTargetInspector : GraphicEditor
{
	public override void OnInspectorGUI ()
	{
		base.serializedObject.Update ();
		EditorGUILayout.PropertyField (base.m_Script, new GUILayoutOption[0]);

		base.RaycastControlsGUI ();
		base.serializedObject.ApplyModifiedProperties ();
	}
}
