using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

using ForieroEditor.Extensions;

//Get List Of Voices : say -v '?'
//Command to say someting : say -v "voice name" -o "~/Desktop/hi.wav" --data-format=LEF32@22100 "hello"

public partial class LangVoiceGenerator : EditorWindow
{
    static partial class IBM
    {
        public static bool OnGUI()
        {
            //if (indexer.voiceLanguageIndex != (indexer.voiceLanguageIndexTmp = EditorGUILayout.Popup("Voice Region", indexer.voiceLanguageIndexTmp, indexer.voiceLanguages)))
            //{
            //    indexer.voiceLanguageIndex = indexer.voiceLanguageIndexTmp;
            //    indexer.voiceIndex = indexer.voiceIndexTmp = -1;
            //    indexer.voiceNames = (from v in voices
            //                          where v.languageCode.Contains(indexer.voiceLanguages[indexer.voiceLanguageIndex])
            //                          select v.name).Distinct().OrderBy(l => l).ToArray();
            //}

            //if (indexer.voiceIndex != (indexer.voiceIndexTmp = EditorGUILayout.Popup("Voice", indexer.voiceIndexTmp, indexer.voiceNames)))
            //{
            //    indexer.voiceIndex = indexer.voiceIndexTmp;
            //}

            //bitRateIndex = EditorGUILayout.Popup("Bitrate", bitRateIndex, bitRateNames);

            //return indexer.voiceLanguageIndex >= 0 && indexer.voiceIndex >= 0 && bitRateIndex >= 0;
            return false;
        }
    }
}