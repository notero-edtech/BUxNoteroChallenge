using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CanEditMultipleObjects, CustomEditor (typeof(EmptyRaycastTarget), false)]
public class EmptyRaycastTargetInspector : GraphicEditor
{
	public override void OnInspectorGUI ()
	{
		base.serializedObject.Update ();
		EditorGUILayout.PropertyField (base.m_Script, new GUILayoutOption[0]);

		base.RaycastControlsGUI ();
		base.serializedObject.ApplyModifiedProperties ();
	}
}