using System;
using UnityEngine;
using System.Linq;
using UnityEditor;

[CustomPropertyDrawer(typeof(PopupIntList))]
public class PopupIntListDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var popupIntListAttribute = attribute as PopupIntList;

        var intInList = attribute as PopupIntList;
        var list = intInList.List;
        string[] stringList = list.Select(i => i.ToString()).ToArray();
        if (property.propertyType == SerializedPropertyType.Integer)
        {
            EditorGUI.LabelField(position, label);
            position.x += position.width / 3f;
            position.width -= position.width / 3f;
            int index = Mathf.Max(0, Array.IndexOf(list, property.intValue));
            if (popupIntListAttribute.showLabel)
            {
                index = EditorGUI.Popup(position, property.displayName, index, stringList);
            } else {
                index = EditorGUI.Popup(position, index, stringList);
            }

            property.intValue = list[index];
        }
        else
        {
            base.OnGUI(position, property, label);
        }
    }
}