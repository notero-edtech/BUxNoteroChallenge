using UnityEditor;

namespace RoleEditor
{
#if UNITY_EDITOR
    public static class RoleEditorPrefs
    {
        public const string m_AppModeKey = "mode";
        public const string m_UseEditorContentValueKey = "isUseEditorContentValue";
        public const string m_CurrentLessonKey = "currentLesson";
        public const string m_CurrentLessonNodeKey = "currentLessonNode";
        public const string m_CurrentSequenceKey = "currentSequence";
        public const string m_CurrentTheoryPageKey = "currentPage";
        public const string m_CurrentQuizKey = "currentQuiz";

        public static bool IsUseEditorContentValue()
        {
            return EditorPrefs.GetBool(m_UseEditorContentValueKey);
        }
    }

#endif
}
