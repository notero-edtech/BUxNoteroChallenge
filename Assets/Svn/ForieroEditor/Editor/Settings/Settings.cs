using UnityEditor;
using UnityEngine;

namespace ForieroEditor.Settings
{
    public class Settings<T> : ScriptableObject where T : Settings<T>
    {
        public static T instance { get { return Instances.EditorInstance<T>(typeof(T).Name, "Assets/Editor/Settings/"); } }

#if UNITY_EDITOR
        public static void Select()
        {
            T i = instance;
            EditorGUIUtility.PingObject(i);
            Selection.objects = new UnityEngine.Object[1] { i };
        }
#endif
    }
}
