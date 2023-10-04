using UnityEditor;
using UnityEngine;
using System.IO;
using ForieroEditor.Extensions;
using System.Linq;

namespace ForieroEditor.MIDIUnified
{
    public static class MIDIUnifiedFixes
    {
        [MenuItem("Foriero/Settings/Midi/Fixes/Placeholders")]
        public static void FixPlaceholders()
        {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "midi_win_store.dll", SearchOption.AllDirectories).Select((f) => f.GetAssetPathFromFullPath());
            var o = "";
            var p = "";

            foreach(var f in files)
            {
                if (f.Contains("WSA/midi_win_store.dll")) o = f;
                if (f.Contains("WSA/dummy/midi_win_store")) p = f;
            }
            
            SetPlaceholder(o, p);

            files = Directory.GetFiles(Directory.GetCurrentDirectory(), "ForieroEngine.Ionic.Zip.dll", SearchOption.AllDirectories).Select((f) => f.GetAssetPathFromFullPath());
           
            foreach (var f in files)
            {
                if (f.Contains("WSA/ForieroEngine.Ionic.Zip.dll")) o = f;
                if (f.Contains("Ionic.Zip/ForieroEngine.Ionic.Zip.dll")) p = f;
            }

            SetPlaceholder(o, p);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original">"Assets/Plugins/Windows/WindowsPlugin.dll"</param>
        /// <param name="placeholder">"Assets/Plugins/Placeholder/WindowsPlugin.dll"</param>
        static void SetPlaceholder(string original, string placeholder)
        {
            Debug.Log(original);
            Debug.Log(placeholder);
            PluginImporter p = PluginImporter.GetAtPath(original) as PluginImporter;
            p.SetPlatformData(BuildTarget.WSAPlayer, "PlaceholderPath", placeholder);
            p.SaveAndReimport();
        }
    }
}
