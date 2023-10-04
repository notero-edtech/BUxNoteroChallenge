using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if FORIERO_INAPP
using UnityEngine.Purchasing;
#endif

namespace ForieroEditor.Tools.MultiProjectBuilder
{
    public partial class MultiProjectBuilder : ScriptableObject
    {
        [Serializable]
        public class PlatformVersionAndroid
        {
            #if FORIERO_INAPP
            public AndroidStore store = AndroidStore.GooglePlay;
            #endif
            public UnityEngine.Object android26Directory;
            public UnityEngine.Object android25Directory;
            public UnityEngine.Object androidLegacyDirectory;
            
            int[] sizes = new int[] { 432, 324, 216, 162, 108, 81 };
            int[] legacySizes = new int[] { 192, 144, 96, 72, 48, 36 };

            public bool OnGUIBuild()
            {
#if FORIERO_INAPP
                store = (AndroidStore)EditorGUILayout.EnumPopup("Android Target", store);
#endif
                return false;
            }

            public void Build(string buildPath, string projectName, string[] scenes)
            {

            }

            public bool OnGUIIcons()
            {
                android26Directory = EditorGUILayout.ObjectField("android26Directory", android26Directory, typeof(UnityEngine.Object), false);
                android25Directory = EditorGUILayout.ObjectField("android25Directory", android25Directory, typeof(UnityEngine.Object), false);
                androidLegacyDirectory = EditorGUILayout.ObjectField("androidLegacyDirectory", androidLegacyDirectory, typeof(UnityEngine.Object), false);

                return false;
            }

            public Texture2D[] GetIcons()
            {
                List<Texture2D> icons = new List<Texture2D>();
               
                return icons.ToArray();
            }
        }
    }
}