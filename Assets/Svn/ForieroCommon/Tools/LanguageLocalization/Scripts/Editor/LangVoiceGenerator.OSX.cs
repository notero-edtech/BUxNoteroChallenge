using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

using ForieroEditor.Extensions;
using System;

//Get List Of Voices : say -v '?'
//Command to say someting : say -v "voice name" -o "~/Desktop/hi.wav" --data-format=LEF32@22100 "hello"

public partial class LangVoiceGenerator : EditorWindow
{
    static partial class OSX
    {
        public static Indexer indexer = new Indexer();

        static int bitRateIndex = 0;

        static string[] bitRateNames = new string[2] {
            "22050",
            "44100"
        };

        public static void GenerateVoice(Voice voice, string path, string text = "")
        {
            text = RemoveActorTag(text);
            text = RemoveInterjections(text);

            if (string.IsNullOrEmpty(text) || text.Equals("."))
                return;

            string guid = Guid.NewGuid().ToString();
            string tmpFile = Path.Combine(Directory.GetCurrentDirectory(), "Temp/" + guid + ".wav");

            Debug.Log(tmpFile);

            System.Diagnostics.Process p1 = new System.Diagnostics.Process();
            try
            {
                p1.StartInfo.FileName = "say";
                p1.StartInfo.Arguments = "-v \"" + voice.name + "\" -o \"" + tmpFile + "\" --data-format=LEF32@" + voice.bitRate.ToString() + " \"" + text + "\"";
                p1.StartInfo.CreateNoWindow = true;
                p1.StartInfo.UseShellExecute = false;
                p1.StartInfo.RedirectStandardOutput = true;
                p1.StartInfo.RedirectStandardError = true;
                p1.Start();
                string output = p1.StandardOutput.ReadToEnd();
                string error = p1.StandardError.ReadToEnd();
                p1.WaitForExit();
                p1.Close();
                if (!string.IsNullOrEmpty(output))
                {
                }

                if (!string.IsNullOrEmpty(error))
                {
                    Debug.Log("ERROR : " + error);
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogWarning(e.Message);
            }
            finally
            {
                p1.Dispose();
                System.GC.Collect();
            }



            System.Diagnostics.Process p2 = new System.Diagnostics.Process();
            try
            {
                p2.StartInfo.FileName = "/opt/local/bin/lame";
                p2.StartInfo.Arguments = "-V2 '" + tmpFile + "' '" + path + "'";
                p2.StartInfo.CreateNoWindow = true;
                p2.StartInfo.UseShellExecute = false;
                p2.StartInfo.RedirectStandardOutput = true;
                p2.StartInfo.RedirectStandardError = true;
                p2.Start();
                string output = p2.StandardOutput.ReadToEnd();
                string error = p2.StandardError.ReadToEnd();
                p2.WaitForExit();
                p2.Close();
                if (!string.IsNullOrEmpty(output))
                {
                }

                if (!string.IsNullOrEmpty(error))
                {
                    Debug.Log("ERROR : " + error);
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogWarning(e.Message);
            }
            finally
            {
                p2.Dispose();
                System.GC.Collect();
            }
        }

        public static void TestVoice(Voice voice, string text = "")
        {
            text = RemoveActorTag(text);
            text = RemoveInterjections(text);
            
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            try
            {
                p.StartInfo.FileName = "say";
                p.StartInfo.Arguments = "-v \"" + voice.name + "\" \"" + text + "\"";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                string error = p.StandardError.ReadToEnd();
                p.WaitForExit();
                p.Close();
                if (!string.IsNullOrEmpty(output))
                {
                }

                if (!string.IsNullOrEmpty(error))
                {
                    Debug.Log("ERROR : " + error);
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogWarning(e.Message);
            }
            finally
            {
                p.Dispose();
                System.GC.Collect();
            }
        }


        public static List<Voice> GetVoices(bool force = false)
        {
            if (indexer.voices.Count > 0 && !force) return indexer.voices;

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            string result = "";
            try
            {
                p.StartInfo.FileName = "say";
                p.StartInfo.Arguments = "-v '?'";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                string error = p.StandardError.ReadToEnd();
                p.WaitForExit();
                p.Close();
                if (!string.IsNullOrEmpty(output))
                {
                    result = output;
                }
                else
                {
                    Debug.Log("Voices are empty!!!");
                }

                if (!string.IsNullOrEmpty(error))
                {
                    Debug.Log("ERROR : " + error);
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogWarning(e.Message);
            }
            finally
            {
                p.Dispose();
                System.GC.Collect();
            }

            indexer.voices = new List<Voice>();
            StringReader sr = new StringReader(result);
            string line = "";
            while (!string.IsNullOrEmpty((line = sr.ReadLine())))
            {
                Voice voice = new Voice();
                //Debug.Log(line);
                voice.name = line.Substring(0, 20).Trim();
                voice.languageCodeRegion = line.Substring(20, 5);
                //Debug.Log(voice.languageCode);
                indexer.voices.Add(voice);
            }

            indexer.UpdateLanguageRegions();
                       
            return indexer.voices;
        }
    }
}