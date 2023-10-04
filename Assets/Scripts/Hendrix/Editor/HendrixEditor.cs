using UnityEditor;
using UnityEngine;

namespace Hendrix
{
    public class HendrixEditor : EditorWindow
    {
        private const float m_Width = 300;
        private const float m_Height = 150;
        private const int m_ServerPort = 26133;
        private const string m_DefaultAddress = "localhost";

        private static HendrixEditor m_EditorWindow;

        private bool m_IsInstructorMode;
        private bool m_IsStudentMode;
        private int m_StudentConnectionAmount;
        private Vector2 m_ScrollPos;

        #region Editor Window Config

        [MenuItem("Hendrix/Hendrix Editor")]
        private static void OpenWindow()
        {
            if(m_EditorWindow == null)
            {
                m_EditorWindow = GetWindow<HendrixEditor>();
                m_EditorWindow.minSize = new Vector2(m_Width, m_Height);
                m_EditorWindow.Show();
            }
            else
            {
                m_EditorWindow.Focus();
            }

            EditorPrefs.DeleteKey(HendrixEditorPrefs.m_AppModeKey);
        }

        private void OnGUI()
        {
            GUI.backgroundColor = new Color(2f, 1.6f, 0, 1);

            SelectMode();

            GUILayout.Space(20);
            //GUILayout.BeginVertical("box");
            //m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.ExpandWidth(true),
            //    GUILayout.ExpandHeight(true));
            //EditorGUILayout.EndScrollView();
            //GUILayout.EndVertical();

            if(!EditorPrefs.HasKey(HendrixEditorPrefs.m_AppModeKey) || GUI.changed)
            {
                SetMode();
            }
        }

        #endregion

        #region Select Application Mode

        private void SelectMode()
        {
            EditorGUILayout.LabelField("Run as", EditorStyles.boldLabel);
            m_IsInstructorMode = GUILayout.Toggle(!m_IsStudentMode, "instructor");
            m_IsStudentMode = GUILayout.Toggle(!m_IsInstructorMode, "student");
        }

        private void SetMode()
        {
            if(GUILayout.Toggle(m_IsStudentMode, ""))
            {
                EditorPrefs.SetString(HendrixEditorPrefs.m_AppModeKey, "mode=student");
            }
            else
            {
                EditorPrefs.SetString(HendrixEditorPrefs.m_AppModeKey, "mode=instructor");
            }
        }

        #endregion
    }
}