using ForieroEngine.SVG;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SVGAtlas))]
public class SVGAtlasEditor : Editor
{
	SVGAtlas atlas = null;

	protected virtual void OnEnable()
	{
		atlas = target as SVGAtlas;
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		EditorGUILayout.Space();
		DropAreaGUI();
	}

	public void DropAreaGUI()
	{
		Event evt = Event.current;
		Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
		GUI.Box(drop_area, "Add Svg");

		switch (evt.type)
		{
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (!drop_area.Contains(evt.mousePosition))
					return;

				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (evt.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();

					foreach (Object o in DragAndDrop.objectReferences)
					{
						var go = o as GameObject;
						var sr = go.GetComponent<SpriteRenderer>();
						if (sr) atlas.sprites.Add(sr.sprite);
						EditorUtility.SetDirty(atlas);
					}
				}
				break;
		}
	}
}