using System.Collections.Generic;
using System.IO;
using ForieroEditor.CommandLine;
using ForieroEditor.Extensions;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

//Get List Of Voices : say -v '?'
//Command to say someting : say -v "voice name" -o "~/Desktop/hi.wav" --data-format=LEF32@22100 "hello"

public partial class LangVoiceGenerator : EditorWindow
{
    static partial class GOOGLE
    {
        public static void Init()
        {
            jsonPath = jsonPath;
            Debug.LogError(jsonPath);
            sdkPath = sdkPath;
            Debug.LogError(sdkPath);
        }
        
        static string _googleCoudTextToSpeechVoicesSH = "";

        public static string googleCloudTextToSpeechVoicesSH{
            get{
                if(string.IsNullOrEmpty(_googleCoudTextToSpeechVoicesSH)){
                    _googleCoudTextToSpeechVoicesSH = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "google_cloud_text2speech_voices.sh", SearchOption.AllDirectories)[0];
                }
                return _googleCoudTextToSpeechVoicesSH;
            }
        }

        static string _googleCoudTextToSpeechSH = "";

        public static string googleCloudTextToSpeechSH
        {
            get
            {
                if (string.IsNullOrEmpty(_googleCoudTextToSpeechSH))
                {
                    _googleCoudTextToSpeechSH = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "google_cloud_text2speech.sh", SearchOption.AllDirectories)[0];
                }
                return _googleCoudTextToSpeechSH;
            }
        }

        static string _jsonPath = null;

        public static string jsonPath
        {
            get
            {
                if (_jsonPath == null){ _jsonPath = EditorPrefs.GetString("LANG_GOOGLE_JSON_PATH", ""); }
                return _jsonPath;
            }
            set
            {
                _jsonPath = value;
                EditorPrefs.SetString("LANG_GOOGLE_JSON_PATH", _jsonPath);
            }
        }

        static string _sdkPath = null;

        public static string sdkPath
        {
            get
            {
                if (_sdkPath == null)
                {
                    _sdkPath = EditorPrefs.GetString("LANG_GOOGLE_SDK_PATH", "");
                }
                return _sdkPath;
            }
            set
            {
                _sdkPath = value;
                EditorPrefs.SetString("LANG_GOOGLE_SDK_PATH", _sdkPath);
            }
        }

        class GoogleVoice
        {
            public List<string> languageCodes { get; set; }
            public string name { get; set; }
            public string ssmlGender { get; set; }
            public int naturalSampleRateHertz { get; set; }
        }

        class GoogleVoices
        {
            public List<GoogleVoice> voices { get; set; }
        }
               
        public static Indexer indexer = new Indexer();

        public static void GenerateVoice(Voice voice, string path, string text = "")
        {
            text = RemoveActorTag(text);
            text = RemoveInterjections(text);
            
            text = text.Replace("\"", "\\\"");
            text = text.Replace("'", "\\'");
            text = text.Replace("`", "\\`");
            text = text.Replace("/", "\\/");
            if (string.IsNullOrEmpty(text) || text.Equals("."))
                return;

            string fileName = Path.GetFileNameWithoutExtension(path);
            string tmpPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp");
            string destPath = Path.GetDirectoryName(path);

            string cmd = googleCloudTextToSpeechSH;
            string args = jsonPath.DoubleQuotes() + " " + sdkPath.DoubleQuotes() + " " +
                    voice.languageCodeRegion.DoubleQuotes() + " " +
                    voice.name.DoubleQuotes() + " " +
                    voice.voiceGender.ToString().ToUpper().DoubleQuotes() + " " +
                    text.DoubleQuotes() + " " +
                    fileName.DoubleQuotes() + " " +
                    tmpPath.DoubleQuotes() + " " +
                    destPath.DoubleQuotes();


            CMD.GenerateProcess(cmd, args);
        }

        public static void TestVoice(Voice voice, string text = "")
        {
            text = RemoveActorTag(text);
            text = RemoveInterjections(text);
            
            string tmpPath = Application.dataPath.Replace("/Assets", "/Temp/test.ogg");
            
            if(File.Exists(tmpPath)) File.Delete(tmpPath);

            GenerateVoice(voice, tmpPath, text);

            CMD.GenerateProcess("play", tmpPath.DoubleQuotes());            
        }

        public static List<Voice> GetVoices(bool force = false)
        {
            if (indexer.voices.Count > 0 && !force) return indexer.voices;

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            string result = "";
            try
            {
                p.StartInfo.FileName = googleCloudTextToSpeechVoicesSH;
                p.StartInfo.Arguments = jsonPath.DoubleQuotes() + " " + sdkPath.DoubleQuotes();
                //p.StartInfo.WorkingDirectory = sdkPath.FixOSPath();
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.Start();                               
                p.WaitForExit();

                string output = p.StandardOutput.ReadToEnd();
                string error = p.StandardError.ReadToEnd();

                p.Close();

                if (!string.IsNullOrEmpty(output))
                {
                    result = output;
                    if (!result.Contains("\"voices\""))
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            Debug.Log(output);
                            Debug.LogError(error);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Voices are empty!!!");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
            finally
            {
                p.Dispose();
                System.GC.Collect();
            }

            if (!result.Contains("\"voices\"")) return indexer.voices;

            GoogleVoices googleVoices = JsonConvert.DeserializeObject<GoogleVoices>(result);

            if (googleVoices == null || googleVoices.voices == null) return indexer.voices;

            indexer.voices = new List<Voice>();

            foreach(var v in googleVoices.voices){
                foreach(var l in v.languageCodes){
                    var voice = new Voice();
                    voice.name = v.name;
                    voice.languageCodeRegion = l;
                    voice.bitRate = v.naturalSampleRateHertz;
                    switch(v.ssmlGender){
                        case "MALE": voice.voiceGender = VoiceGender.Male; break;
                        case "FEMALE": voice.voiceGender = VoiceGender.Female; break;
                    }
                    indexer.voices.Add(voice);
                }
            }

            indexer.UpdateLanguageRegions();

            return indexer.voices;
        }
    }
}