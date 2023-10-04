using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ForieroEditor.Tools.MultiProjectBuilder
{
    public partial class MultiProjectBuilder : ScriptableObject
    {
        [Serializable]
        public class PlatformVersionStandalone
        {
            public enum Platform
            {
                Linux = 10,
                OSX = 20,
                Windows = 30
            }
            
            public enum MacOSArchitecture
            {
                x64,
                ARM64,
                x64ARM64,
            }

            public Platform platform = Platform.OSX;

            public MacOSArchitecture architecture = MacOSArchitecture.x64ARM64;
            public bool macAppStoreValidation = false;
            
            public Object standaloneDirectory;

            public bool OnGUIBuild()
            {
                macAppStoreValidation = EditorGUILayout.Toggle("MacAppStore - Validation", macAppStoreValidation);
                platform = (Platform) EditorGUILayout.EnumPopup("Platform", platform);
                if (platform == Platform.OSX) architecture = (MacOSArchitecture) EditorGUILayout.EnumPopup("Architecture", architecture);
                return false;
            }

            public void Build(string buildPath, string projectName, string[] scenes)
            {
                string path = Path.Combine(buildPath, architecture.ToString());
                Debug.Log("Build Path : " + path);
                string fileName = projectName + GetExtension();
                Debug.Log("File Name : " + fileName);
                path = Path.Combine(path, fileName);
                Debug.Log("Full Path : " + path);
                BuildPipeline.BuildPlayer(scenes, path, GetBuildTarget(), BuildOptions.None);
            }

            public BuildTarget GetBuildTarget()
            {
                BuildTarget result = BuildTarget.Android;
                switch (platform)
                {
                    case Platform.Linux:
                        result = BuildTarget.StandaloneLinux64;
                        break;
                    case Platform.Windows:
                        result = BuildTarget.StandaloneWindows64;
                        break;
                    case Platform.OSX:
                        result = BuildTarget.StandaloneOSX;
                        break;
                }

                return result;
            }

            string GetExtension()
            {
                string result = "";
                switch (platform)
                {
                    case Platform.Linux:
                        result = "";
                        break;
                    case Platform.Windows:
                        result = ".exe";
                        break;
                    case Platform.OSX:
                        result = ".app";
                        break;
                }

                return result;
            }

            public bool OnGUIIcons()
            {
                standaloneDirectory = EditorGUILayout.ObjectField("standaloneDirectory", standaloneDirectory,
                    typeof(Object), false);
                return false;
            }

            public Texture2D[] GetIcons()
            {
                Texture2D[] icons = new Texture2D[0];

                return icons;
            }
        }
    }
}