using System;
using UnityEditor;
using UnityEngine;
using ForieroEditor.SceneManager;

[CustomEditor(typeof(SceneSettings))]
public class SceneSettingsInspector : Editor
{
    private SerializedProperty _debug;
    private SerializedProperty _vignette;
    private SerializedProperty _logoUIPrefab;
    private SerializedProperty _loadingUIPrefab;
    private SerializedProperty _sceneLoadingUIPrefab;
    private void OnEnable()
    {
        _debug = serializedObject.FindProperty("debug");
        _vignette = serializedObject.FindProperty("vignette");
        _logoUIPrefab = serializedObject.FindProperty("logoUIPrefab");
        _loadingUIPrefab = serializedObject.FindProperty("loadingUIPrefab");
        _sceneLoadingUIPrefab = serializedObject.FindProperty("sceneLoadingUIPrefab");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_debug);
        EditorGUILayout.PropertyField(_vignette);
        EditorGUILayout.PropertyField(_logoUIPrefab);
        EditorGUILayout.PropertyField(_loadingUIPrefab);
        EditorGUILayout.PropertyField(_sceneLoadingUIPrefab);
        serializedObject.ApplyModifiedProperties();
        
        if (GUILayout.Button("Open")) { SceneGraph.InitSceneGraphExternal(target as SceneSettings); }
    }
}
