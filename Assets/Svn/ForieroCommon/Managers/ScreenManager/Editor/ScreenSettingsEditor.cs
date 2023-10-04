using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Audio;

[CustomEditor(typeof(ScreenSettings))]
public class ScreenSettingsEditor : Editor
{
    //SerializedProperty m_Script;

    SerializedProperty debug;

    SerializedProperty editor;
    SerializedProperty desktop;
    SerializedProperty mobile;
    SerializedProperty xbox;
    SerializedProperty ps3;
    SerializedProperty ps4;
    SerializedProperty ps5;
    SerializedProperty nintendoswitch;
    

    //ScreenSettings screenSettings;

    protected virtual void OnEnable()
    {
        //screenSettings = target as ScreenSettings;

        //m_Script = serializedObject.FindProperty("m_Script");

        editor = serializedObject.FindProperty("editor");
        desktop = serializedObject.FindProperty("desktop");
        mobile = serializedObject.FindProperty("mobile");
        xbox = serializedObject.FindProperty("xbox");
        ps3 = serializedObject.FindProperty("ps3");
        ps4 = serializedObject.FindProperty("ps4");
        ps5 = serializedObject.FindProperty("ps5");
        nintendoswitch = serializedObject.FindProperty("nintendoswitch");
    }

    Color backgroundColor;

    enum Tabs
    {
        Editor, Desktop, Mobile, XBox, PS3, PS4, PS5, Switch
    }

    Tabs tab = Tabs.Desktop;

    public override void OnInspectorGUI()
    {
        backgroundColor = GUI.backgroundColor;

        EditorGUILayout.BeginHorizontal();

        foreach (string t in System.Enum.GetNames(typeof(Tabs)))
        {
            if (tab.ToString() == t)
            {
                GUI.backgroundColor = Color.green;
            }

            if (GUILayout.Button(t, EditorStyles.toolbarButton))
            {
                tab = (Tabs)System.Enum.Parse(typeof(Tabs), t);
            }

            GUI.backgroundColor = backgroundColor;
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.BeginChangeCheck();
        {
            switch (tab)
            {
                case Tabs.Editor: DrawScreenItem(editor); break;
                case Tabs.Desktop: DrawScreenItem(desktop); break;
                case Tabs.Mobile: DrawScreenMobileItem(mobile); break;
                case Tabs.XBox: DrawScreenItem(xbox); break;
                case Tabs.PS3: DrawScreenItem(ps3); break;
                case Tabs.PS4: DrawScreenItem(ps4); break;
                case Tabs.PS5: DrawScreenItem(ps5); break;
                case Tabs.Switch: DrawScreenItem(nintendoswitch); break;
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawScreenItem(SerializedProperty sp)
    {
        EditorGUILayout.PropertyField(sp.FindPropertyRelative("vSyncCount"));
        EditorGUILayout.PropertyField(sp.FindPropertyRelative("frameRate"));
        EditorGUILayout.PropertyField(sp.FindPropertyRelative("antialiasing"));
        EditorGUILayout.PropertyField(sp.FindPropertyRelative("sleepTimeout"));
    }

    private void DrawScreenMobileItem(SerializedProperty sp)
    {
        DrawScreenItem(sp);
        EditorGUILayout.PropertyField(sp.FindPropertyRelative("renderFrameRateInterval"));
    }
}
