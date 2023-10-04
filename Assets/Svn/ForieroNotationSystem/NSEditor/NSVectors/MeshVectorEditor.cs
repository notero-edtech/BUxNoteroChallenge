/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
//c# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace ForieroEngine.Music.NotationSystem.Classes
{
    [CustomEditor(typeof(MeshVector))]
    [CanEditMultipleObjects]
    public class MeshVectorEditor : Editor
    {
        MeshVector o;

        SerializedProperty material;
        SerializedProperty color;
        SerializedProperty raycastTarget;
        SerializedProperty useRect;

        SerializedProperty vector;

        Dictionary<VectorEnum, SerializedProperty> vectors = new Dictionary<VectorEnum, SerializedProperty>();

        SerializedProperty sp = null;

        void OnEnable()
        {
            EditorApplication.update += Update;

            o = serializedObject.targetObject as MeshVector;

            vector = serializedObject.FindProperty("vectorEnum");
            useRect = serializedObject.FindProperty("useRect");

            vectors.Add(VectorEnum.LineTest, serializedObject.FindProperty("lineTest"));
            vectors.Add(VectorEnum.LineHorizontal, serializedObject.FindProperty("lineHorizontal"));
            vectors.Add(VectorEnum.LineVertical, serializedObject.FindProperty("lineVertical"));
            vectors.Add(VectorEnum.Beam, serializedObject.FindProperty("beam"));
            vectors.Add(VectorEnum.Slur1, serializedObject.FindProperty("slur1"));
            vectors.Add(VectorEnum.Slur2, serializedObject.FindProperty("slur2"));
            vectors.Add(VectorEnum.Tie, serializedObject.FindProperty("tigh"));
            vectors.Add(VectorEnum.Tuplet, serializedObject.FindProperty("tuplet"));
            vectors.Add(VectorEnum.Hairpin, serializedObject.FindProperty("hairpin"));
        }

        void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        void Update()
        {
            o.Commit();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        static void OnScriptsReloaded()
        {
            UIVector[] vectors = GameObject.FindObjectsOfType<UIVector>();

            foreach (UIVector vector in vectors)
            {
                vector.SetAllDirty();
                vector.Commit();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

/*            EditorGUILayout.BeginHorizontal();
            o.material = (Material)EditorGUILayout.ObjectField("Material", o.material, typeof(Material), false);
            EditorGUILayout.EndHorizontal();

            o.color = EditorGUILayout.ColorField("Color", o.color);
            o.raycastTarget = EditorGUILayout.Toggle("Raycast Target", o.raycastTarget);

            EditorGUILayout.PropertyField(useRect);
            EditorGUILayout.PropertyField(vector);

            VectorEnum v = (VectorEnum)System.Enum.Parse(typeof(VectorEnum), vector.enumNames[vector.enumValueIndex]);

            if (vectors.ContainsKey(v))
            {
                sp = vectors[v];
                if (sp != null)
                {
                    EditorGUILayout.PropertyField(sp, true);
                }
                else
                {
                    Debug.LogError("Vector property not found : " + v.ToString());
                }
            }

            if (EditorGUI.EndChangeCheck() || GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.BeginDisabledGroup(v == VectorEnum.Undefined);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Commit"))
            {
                o.Commit();
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Reset"))
            {
                o.ResetVector();
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndHorizontal();*/

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.RectField("Rect", o.rect);
            EditorGUILayout.Vector2Field("Pivot", o.pivot);

            EditorGUI.EndDisabledGroup();
        }
    }
}
