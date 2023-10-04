using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ForieroEditor.Extensions;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ForieroEditor.CommandLine
{
    public static class CMD
    {
        public static void GenerateProcess(string aCommand, string anArguments, bool threaded = false, bool copyToClipboard = false, bool continuous = false, Action<string> onOutput = null, Action<string> onError = null, bool debug = true)
        {
            if(debug) Debug.Log("CMD : " + aCommand + " " + anArguments);

            if (copyToClipboard) {
                (aCommand + " " + anArguments).CopyToClipboard();
                if(debug) Debug.Log("CMD : Copied to clipboard.");
            }

            if (threaded)
            {
                void OnOutput(string o) => EditorDispatcher.Dispatch(()=>onOutput?.Invoke(o));
                void OnError(string e) => EditorDispatcher.Dispatch(()=>onError?.Invoke(e));                
                Task.Run(() => GenerateProcessInternal(aCommand, anArguments, continuous, OnOutput, OnError, debug));
            }
            else
            {
                GenerateProcessInternal(aCommand, anArguments, continuous, onOutput, onError, debug);
            }
        }

        public static void Bash(string aCommand, bool threaded = false, bool copyToClipboard = false, bool continuous = false, Action<string> onOutput = null, Action<string> onError = null, bool debug = true)
        {
            var escapedArgs = aCommand.Replace("\"", "\\\"");
            string cmd = "/bin/bash";
            string args = $"-c \"{escapedArgs}\"";

            GenerateProcess(cmd, args, threaded, copyToClipboard, continuous, onOutput, onError);                                  
        }

        static void GenerateProcessInternal(string aCommand, string anArguments, bool continuous, Action<string> onOutput = null, Action<string> onError = null, bool debug = true)
        {
            using (Process p = new Process())
            {
                string output = "";
                string error = "";
                try
                {
                    p.StartInfo.FileName = aCommand;
                    p.StartInfo.Arguments = anArguments;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;

                    if (continuous)
                    {
                        p.OutputDataReceived += (o, e) => { if (debug) Debug.Log(e.Data); };
                        p.ErrorDataReceived += (o, e) => { if (debug) Debug.LogError(e.Data); };
                    }

                    p.Start();

                    if (continuous)
                    {
                        p.BeginOutputReadLine();
                        p.BeginErrorReadLine();
                    }

                    p.WaitForExit();

                    if (!continuous)
                    {
                        output = p.StandardOutput.ReadToEnd();
                        if (!string.IsNullOrEmpty(output))
                        {
                            if(debug) Debug.Log("OUTPUT : " + output);
                            onOutput?.Invoke(output);
                        }
                        error = p.StandardError.ReadToEnd();
                        if (!string.IsNullOrEmpty(error))
                        {
                            if(debug) Debug.LogError("ERROR : " + error);
                            onError?.Invoke(error);
                        }
                    }

                    p.Close();
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
            System.GC.Collect();
        }
    }
}
