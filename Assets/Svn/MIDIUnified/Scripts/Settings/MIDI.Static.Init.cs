/* Copyright © Marek Ledvina, Foriero s.r.o. */
using System;
using System.Runtime.InteropServices;
using System.Threading;
using AOT;
using ForieroEngine.Collections.NonBlocking;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified.Plugins;
using ForieroEngine.MIDIUnified.Synthesizer;
using UnityEngine;

namespace ForieroEngine.MIDIUnified
{
    public partial class MIDI : MonoBehaviour
    {
        public static MIDI instance;
        public static volatile bool terminateMidiThreads = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void AutoInit()
        {
            System.Diagnostics.Stopwatch stopWatch = ForieroDebug.CodePerformance ? System.Diagnostics.Stopwatch.StartNew() : null;
            if (ForieroDebug.CodePerformance) Debug.Log("METHOD STOPWATCH (MIDI.AutoInit - AfterSceneLoad): " + stopWatch?.Elapsed.ToString());
            if (instance)
            {
                Debug.Log("MU | MIDIUnified already in scene.");
                return;
            }

#if UNITY_STANDALONE
            if(Application.isEditor || Debug.isDebugBuild) RegisterDebugCallback(OnDebugCallback);
#endif

#if UNITY_EDITOR
            UnityEditor.EditorApplication.wantsToQuit += () =>
            {
                CleanUp();
                return true;
            };
#endif            

            if (MIDISettings.instance.initialize)
            {
                if (MIDISettings.IsDebug) Debug.Log("MU | MIDI AutoInitializing");

                MIDISettings.soundFontPersistentPath = (Application.persistentDataPath + @"\soundfont.sf2").FixOSPath();

                GameObject go = new GameObject("MIDIUnified - AutoInit");
                go.AddComponent<MIDI>();

                if (MIDISettings.instance.inputSettings.initialize && MidiInput.singleton == null)
                {
                    go.AddComponent<MidiInput>();
                }

                if (MIDISettings.instance.keyboardSettings.initialize && MidiKeyboardInput.singleton == null)
                {
                    go.AddComponent<MidiKeyboardInput>();
                }

                if (MIDISettings.instance.playmakerSettings.initialize && MidiPlayMakerInput.singleton == null)
                {
                    go.AddComponent<MidiPlayMakerInput>();
                }
            }
            if (ForieroDebug.CodePerformance) Debug.Log("METHOD STOPWATCH (MIDI.AutoInit - AfterSceneLoad): " + stopWatch?.Elapsed.ToString());
        }

        #if UNITY_STANDALONE || UNITY_EDITOR 
        private const string DllName = "rtmidi";
        #elif UNITY_IOS
        private const string DllName = "__Internal";
        #endif
        
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_IOS

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RegisterDebugCallback(debugCallback cb);
        
        private delegate void debugCallback(IntPtr request, int color, int size);

        private enum Color { red, green, blue, black, white, yellow, orange };

        [MonoPInvokeCallback(typeof(debugCallback))]
        private static void OnDebugCallback(IntPtr request, int color, int size)
        {
            //Ptr to string
            string debug_string = Marshal.PtrToStringAnsi(request, size);

            //Add Specified Color
            debug_string = String.Format("{0}{1}{2}{3}{4}", "<color=", ((Color)color).ToString(),">",debug_string,"</color>");

            Debug.Log(debug_string);
        }

#endif

        private static bool cleanedUp = false;

        private static void CleanUp()
        {
            if (initialized && !cleanedUp)
            {
                if (MIDISettings.IsDebug) Debug.Log("MU CleanUp : AllPedalsOff()");
                MidiOut.AllPedalsOff();

                if (MIDISettings.IsDebug) Debug.Log("MU CleanUp : AllSoundsOff()");
                MidiOut.AllSoundOff();

                if (MIDISettings.IsDebug) Debug.Log("MU CleanUp : ResetAllControllers()");
                MidiOut.ResetAllControllers();

                if (MIDISettings.IsDebug) Debug.Log("MU CleanUp : DisconnectDevicesIN()");
                MidiINPlugin.DisconnectDevices();

                if (MIDISettings.IsDebug) Debug.Log("MU CleanUp : DisconnectDevicesOUT()");
                MidiOUTPlugin.DisconnectDevices();

                if (MIDISettings.IsDebug) Debug.Log("MU CleanUp : CloseVirtualPortsIN()");
                MidiINPlugin.CloseVirtualPorts();

                if (MIDISettings.IsDebug) Debug.Log("MU CleanUp : CloseVirtualPortsOUT()");
                MidiOUTPlugin.CloseVirtualPorts();
                                
                if (MIDISettings.IsDebug) Debug.Log("MU CleanUp : Synth.Stop()");
                Synth.Stop();

                MidiOUTPlugin.Terminate();
                MidiINPlugin.Terminate();
                Synth.Terminate();

                cleanedUp = true;
            }
        }
    }
}
