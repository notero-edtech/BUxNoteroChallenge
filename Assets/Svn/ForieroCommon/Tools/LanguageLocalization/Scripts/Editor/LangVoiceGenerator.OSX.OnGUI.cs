using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

using ForieroEditor.Extensions;

public partial class LangVoiceGenerator : EditorWindow
{
    static partial class OSX
    {
        public static bool OnGUI()
        {
            bitRateIndex = EditorGUILayout.Popup("Bitrate", bitRateIndex, bitRateNames);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Gender", (indexer.voice != null ? indexer.voice.voiceGender.ToString() : VoiceGender.Undefined.ToString()));
            EditorGUI.EndDisabledGroup();

            return indexer.voiceLanguageIndex >= 0 && indexer.voiceNameIndex >= 0 && bitRateIndex >= 0;
        }
    }
}