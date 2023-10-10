using UnityEditor;
using UnityEngine;

namespace RoleEditor
{
#if UNITY_EDITOR
    public class RoleEditor : EditorWindow
    {
        private const float m_Width = 300;
        private const float m_Height = 150;

        private static RoleEditor m_EditorWindow;

        private bool m_IsInstructorMode;
        private bool m_IsStudentMode;

        #region Editor Window Config

        [MenuItem("Role Manager/Role Editor")]
        private static void OpenWindow()
        {
            if(m_EditorWindow == null)
            {
                m_EditorWindow = GetWindow<RoleEditor>();
                m_EditorWindow.minSize = new Vector2(m_Width, m_Height);
                m_EditorWindow.Show();
            }
            else
            {
                m_EditorWindow.Focus();
            }

            EditorPrefs.DeleteKey(RoleEditorPrefs.m_AppModeKey);
        }

        private void OnGUI()
        {
            GUI.backgroundColor = new Color(2f, 1.6f, 0, 1);

            SelectMode();

            GUILayout.Space(20);

            if(!EditorPrefs.HasKey(RoleEditorPrefs.m_AppModeKey) || GUI.changed)
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
                EditorPrefs.SetString(RoleEditorPrefs.m_AppModeKey, "mode=student");
            }
            else
            {
                EditorPrefs.SetString(RoleEditorPrefs.m_AppModeKey, "mode=instructor");
            }
        }

        #endregion
    }
#endif
}