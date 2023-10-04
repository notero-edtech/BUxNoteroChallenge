using System;
using UnityEditor;
using UnityEngine;

namespace ForieroEngine.Settings
{
    public interface ISettingsProvider { }
    [AttributeUsage(AttributeTargets.Class)] public class SettingsManager : Attribute {  }
    public abstract class Settings<T> : ScriptableObject where T : Settings<T>, ISettingsProvider
    {
        private static T _instance = null;
        public static T instance => _instance ? _instance : _instance = Instances.Instance<T>(typeof(T).Name, "Settings");
        public static T Instance() => instance;
        
#if UNITY_EDITOR
        public static void Select()
        {
            var i = instance;
            EditorGUIUtility.PingObject(i);
            Selection.objects = new UnityEngine.Object[1] { i };
        }

        public virtual void Apply()
        {
            if (this == instance) return;
            EditorUtility.CopySerialized(this, instance);
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
