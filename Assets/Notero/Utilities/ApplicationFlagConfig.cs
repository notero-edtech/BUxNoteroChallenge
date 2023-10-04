using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Notero.Utilities
{
    public static class ApplicationFlagConfig
    {
        public static Mode ApplicationMode = default;
        public static bool IsInstructorMode => ApplicationMode == Mode.instructor;

        public static bool IsStudentMode => ApplicationMode == Mode.student;

        // format key=value
        private static Regex m_CmdRegEx = new Regex(@"(?<name>.+?)=(?<val>.+)");
        private static Dictionary<FlagKey, string> m_CommandlineArgs = default;

        private enum FlagKey
        {
            mode
        }

        public enum Mode
        {
            instructor,
            student
        }

        public static void InitializeCommandlineArgs()
        {
            if(m_CommandlineArgs == null)
            {
                string[] args = GetArgumentsStringArray();
                m_CommandlineArgs = new Dictionary<FlagKey, string>();

                foreach(string s in args)
                {
                    Match m = m_CmdRegEx.Match(s);

                    if(m.Success && Enum.Parse(typeof(FlagKey), m.Groups[1].Value) is FlagKey key)
                    {
                        string value = m.Groups[2].Value;

                        m_CommandlineArgs.Add(key, value);
                        CacheConfig(key, value);
                    }
                }
            }
        }

        private static string[] GetArgumentsStringArray()
        {
#if UNITY_ANDROID
            return new string[]{ "mode=student" };
#elif UNITY_EDITOR
            return new string[] { EditorPrefs.GetString("mode") };
#else
            return Environment.GetCommandLineArgs();
#endif
        }

        private static void CacheConfig(FlagKey key, string value)
        {
            switch(key)
            {
                case FlagKey.mode:
                    CacheApplicationMode(value);
                    break;
            }
        }

        private static void CacheApplicationMode(string modeStr)
        {
            if(Enum.Parse(typeof(Mode), modeStr) is Mode mode)
            {
                ApplicationMode = mode;
            }
        }
    }
}