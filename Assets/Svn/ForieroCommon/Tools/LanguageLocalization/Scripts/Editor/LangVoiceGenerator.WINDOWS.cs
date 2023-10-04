using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

using ForieroEditor.Extensions;

public partial class LangVoiceGenerator : EditorWindow
{
    static partial class WINDOWS
    {
        public static Indexer indexer = new Indexer();

        public static void GenerateVoice(Voice voice, string path, string text = "")
        {
            text = RemoveActorTag(text);
            text = RemoveInterjections(text);

            if (string.IsNullOrEmpty(text) || text.Equals("."))
                return;

        }

        public static void TestVoice(Voice voice, string text = "")
        {
            text = RemoveActorTag(text);
            text = RemoveInterjections(text);

            if (string.IsNullOrEmpty(text) || text.Equals("."))
                return;
        }

        public static List<Voice> GetVoices(bool force = false)
        {
            if (indexer.voices.Count > 0 && !force) return indexer.voices;

            return indexer.voices;
        }
    }
}