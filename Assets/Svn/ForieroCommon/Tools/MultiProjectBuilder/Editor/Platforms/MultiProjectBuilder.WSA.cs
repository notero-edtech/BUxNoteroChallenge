using System;
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.Extensions;
using UnityEditor;
using UnityEngine;

using Debug = UnityEngine.Debug;

namespace ForieroEditor.Tools.MultiProjectBuilder
{
    public partial class MultiProjectBuilder : ScriptableObject
    {
        [Serializable]
        public class PlatformVersionWSA
        {
            public UnityEngine.Object wsa;
            public UnityEngine.Object wsa_icon;
            public UnityEngine.Object wsa_wide_icon;

            public bool OnGUIBuild()
            {
                return GUI.changed;
            }

            public void Build(string buildPath, string projectName, string[] scenes)
            {

            }

            public bool OnGUIIcons()
            {
                wsa = EditorGUILayout.ObjectField("wsa", wsa, typeof(UnityEngine.Object), false);
                wsa_icon = EditorGUILayout.ObjectField("wsa_icon", wsa_icon, typeof(UnityEngine.Object), false);
                wsa_wide_icon = EditorGUILayout.ObjectField("wsa_wide_icon", wsa_wide_icon, typeof(UnityEngine.Object), false);
                return GUI.changed;
            }

            public Texture2D[] GetIcons()
            {
                Texture2D[] icons = new Texture2D[0];

                Array.Reverse(icons);

                return icons;
            }
        }
    }
}