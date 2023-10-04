using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

using ForieroEditor.Extensions;
using Newtonsoft.Json;

public partial class LangVoiceGenerator : EditorWindow
{
    static partial class GOOGLE
    {
        public static bool OnGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("BitRate", (indexer.voice != null ? indexer.voice.bitRate.ToString() : "20050"));
            EditorGUILayout.TextField("Gender", (indexer.voice != null ? indexer.voice.voiceGender.ToString() : VoiceGender.Undefined.ToString()));
            EditorGUI.EndDisabledGroup();

            jsonPath = EditorGUILayout.TextField("Google API Json", jsonPath);
            sdkPath = EditorGUILayout.TextField("Google API SDK", sdkPath);
            return indexer.voiceLanguageIndex >= 0 && indexer.voiceNameIndex >= 0;
        }
    }
}