using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UISettings))]
public class UISettingsInspector : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
