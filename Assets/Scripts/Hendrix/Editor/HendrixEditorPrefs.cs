using UnityEditor;

namespace Hendrix
{
    public static class HendrixEditorPrefs
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

        public static int GetLessonContentValue(string key)
        {
            int result = EditorPrefs.GetInt(key);

            if (result < 0)
            {
                result = 0;
            }

            return result;
        }
    }
}
