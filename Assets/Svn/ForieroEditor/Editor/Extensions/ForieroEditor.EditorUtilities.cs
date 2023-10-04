using System.Threading;
using UnityEditor;
using UnityEngine;

namespace ForieroEditor
{
    [InitializeOnLoad]
    public static class EditorUtilities
    {
        static EditorUtilities() {
            MainThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public static readonly int MainThreadId = Thread.CurrentThread.ManagedThreadId;
        public static bool IsMainThread => Thread.CurrentThread.ManagedThreadId == MainThreadId;

        public static bool DisplayDialog(string title, string message, string ok, string cancel = null)
        {
            var result = false;
            var done = false;

            if (IsMainThread)
            {
                if (string.IsNullOrEmpty(cancel)) result = EditorUtility.DisplayDialog(title, message, ok);
                else result = EditorUtility.DisplayDialog(title, message, ok, cancel);
            } else
            {
                EditorDispatcher.Dispatch(() =>
                {
                    if (string.IsNullOrEmpty(cancel)) result = EditorUtility.DisplayDialog(title, message, ok);
                    else result = EditorUtility.DisplayDialog(title, message, ok, cancel);
                    done = true;
                });

                while (!done) { }
            }
                       
            return result;
        }

        public static int DisplayDialogComplex(string title, string message, string ok, string cancel, string alt)
        {
            var result = 1;
            var done = false;

            if (IsMainThread)
            {
                result = EditorUtility.DisplayDialogComplex(title, message, ok, cancel, alt);
                done = true;
            } else
            {
                EditorDispatcher.Dispatch(() =>
                {
                    result = EditorUtility.DisplayDialogComplex(title, message, ok, cancel, alt);
                    done = true;
                });

                while (!done) { }
            }
            
            return result;
        }

        public static class AssetDatabase{
            public static void Refresh()
            {                
                var done = false;
                EditorDispatcher.Dispatch(() => { UnityEditor.AssetDatabase.Refresh(); done = true; });
                if (!IsMainThread) while (!done) { }                
            }
        }
    }
}
