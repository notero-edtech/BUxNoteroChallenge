using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ForieroEditor
{
    public static class InspectorExtensions
    {
        public static void DropObjects<T>(this List<T> list, string label = "Drop files here!", float height = 25f) where T : UnityEngine.Object
        {
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, height, GUILayout.ExpandWidth(true));
            EditorGUI.HelpBox(drop_area, label, MessageType.None);

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

                        foreach (Object dragged_object in DragAndDrop.objectReferences)
                        {
                            if (dragged_object is T) list.Add(dragged_object as T);
                        }
                    }
                    break;
            }
        }
    }
}