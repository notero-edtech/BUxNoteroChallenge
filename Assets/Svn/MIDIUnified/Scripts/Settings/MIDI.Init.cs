/* Copyright © Marek Ledvina, Foriero s.r.o. */
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using ForieroEngine.MIDIUnified.Plugins;
using ForieroEngine.MIDIUnified.Synthesizer;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace ForieroEngine.MIDIUnified
{
    public partial class MIDI : MonoBehaviour
    {
        private static IEnumerator Init()
        {
            if (MIDISettings.instance.initialize)
            {
                #if UNITY_ANDROID
                if (!Permission.HasUserAuthorizedPermission("BLUETOOTH")) Permission.RequestUserPermission("BLUETOOTH");
                if (!Permission.HasUserAuthorizedPermission("BLUETOOTH_ADMIN")) Permission.RequestUserPermission("BLUETOOTH_ADMIN");
                if (!Permission.HasUserAuthorizedPermission("ACCESS_COARSE_LOCATION")) Permission.RequestUserPermission("ACCESS_COARSE_LOCATION");
                #endif
                
                if (MIDISettings.IsDebug) Debug.Log("MIDI Init");
                yield return InitSoundFont();
                yield return InitMidiIO();
                InitSynth();
                InitMIDIUnfied();
                initialized = true;
            }
            else
            {
                if (MIDISettings.IsDebug) Debug.Log("MIDI was set to be not initialized. Go to Menu->Foriero->Settings->Midi");
            }
        }

        public static bool forceDefaultMidiIn = false;
        public static int defaultMidiIn = 0;
        public static bool forceDefaultMidiOut = false;
        public static int defaultMidiOut = 0;

        public static int channelMask = -1;
        public static int synthChannelMask = -1;

        public static bool initialized = false;

        public static void RefreshMidiIO()
        {
            MidiINPlugin.Refresh();
            MidiOUTPlugin.Refresh();
        }

        public static void RefreshSynth()
        {

        }

        private static bool soundFontCopiedToPersistentPath = false;
        public static TextAsset soundFontAsset = null;

        private static IEnumerator InitSoundFontResources()
        {
            ResourceRequest request = null;

            if (MIDISettings.IsDebug) Debug.Log("MU | Soundfont | LOADING Async from resources : soundfont.sf2");
            request = Resources.LoadAsync<TextAsset>("soundfont.sf2");

            yield return request;

            soundFontAsset = request.asset as TextAsset;
        }

        private static IEnumerator InitSoundFontPersistentPath()
        {
            if (File.Exists(MIDISettings.soundFontPersistentPath)) yield break;

            yield return InitSoundFontResources();

            if (soundFontAsset && soundFontAsset.bytes != null && soundFontAsset.bytes.Length > 0)
            {
                byte[] bytes = soundFontAsset.bytes;
                Task.Run(() =>
                {
                    File.WriteAllBytes(MIDISettings.soundFontPersistentPath, bytes);

                    if (File.Exists(MIDISettings.soundFontPersistentPath)) if (MIDISettings.IsDebug) Debug.Log("MU | Soundfont | Saved to : " + MIDISettings.soundFontPersistentPath);
                        else Debug.LogError("MU | Soundfont | Error saving soundfont to : " + MIDISettings.soundFontPersistentPath);

                    soundFontCopiedToPersistentPath = true;
                });
            }

            if (soundFontAsset) Resources.UnloadAsset(soundFontAsset);

            yield return new WaitUntil(() => soundFontCopiedToPersistentPath);
        }

        private static IEnumerator InitSoundFont()
        {

            switch (MIDISynthSettings.GetPlatformSettings().GetSynthEnum())
            {
                case Synth.SynthEnum.NONE: break;
                case Synth.SynthEnum.NATIVE: break;
                case Synth.SynthEnum.BASS24: yield return InitSoundFontPersistentPath(); break;
                case Synth.SynthEnum.CSHARP: yield return InitSoundFontResources(); break;
#if MIDIUNIFIED_BETA
                case Synth.SynthEnum.FLUID: yield return InitSoundFontPersistentPath(); break;
                case Synth.SynthEnum.TINYSOUNDFONT: yield return InitSoundFontRousources(); break;
#endif
            }

            yield return null;
        }

        private static void InitMIDIUnfied()
        {
            if (MIDISettings.IsDebug) Debug.Log("Initializing MIDIUnified");

            MidiOut.channelMask = channelMask;
            MidiOut.channelMask = PlayerPrefs.GetInt("MIDIOUT_MIDI_MASK", MIDISettings.instance.channelMask);

            if (MIDISettings.IsDebug) Debug.Log("Channel Mask : " + MidiOut.channelMask);

            MidiOut.synthChannelMask = synthChannelMask;
            MidiOut.synthChannelMask = PlayerPrefs.GetInt("SYNTH_MIDI_MASK", MIDISettings.instance.synthChannelMask);

            if (MIDISettings.IsDebug) Debug.Log("SynthChannel Mask : " + MidiOut.synthChannelMask);

            if (MIDISettings.instance.instrumentsSettings.initialize)
            {
                for (int i = 0; i < MIDISettings.instance.instrumentsSettings.instruments.Length; i++)
                {
                    if (MIDISettings.instance.instrumentsSettings.instruments[i] != ProgramEnum.None)
                    {
                        MidiOut.SetInstrument(MIDISettings.instance.instrumentsSettings.instruments[i], (ChannelEnum)i);
                    }
                }
            }

            MidiOut.IgnoreProgramMessages = MIDISettings.instance.ignoreProgramMessages;
        }

        private static IEnumerator InitMidiIO()
        {
            if (!MidiINPlugin.initialized) { Task.Run(() => { MidiINPlugin.Init(); }); }
            if (!MidiOUTPlugin.initialized) { Task.Run(() => { MidiOUTPlugin.Init(); }); }

            yield return new WaitUntil(MidiINPlugin.Initialized);
            yield return new WaitUntil(MidiOUTPlugin.Initialized);

            foreach (var p in MIDISettings.instance.virtualIns) MidiINPlugin.OpenVirtualPort(p);
            foreach (var p in MIDISettings.instance.virtualOuts) MidiOUTPlugin.OpenVirtualPort(p);

            forceDefaultMidiIn = MIDISettings.instance.forceDefaultMidiIn;
            defaultMidiIn = MIDISettings.instance.defaultMidiIn;

            void ConnectDefaultMidiIn() { if (defaultMidiIn >= 0 && defaultMidiIn < MidiINPlugin.GetDeviceCount()) { MidiINPlugin.ConnectDevice(defaultMidiIn); } }
            
            if (forceDefaultMidiIn) ConnectDefaultMidiIn();
            else
            {
                if(MIDISettings.instance.restoreMidiInConnections) MidiINPlugin.RestoreConnections();
                if (MidiINPlugin.connectedDevices.Count == 0) ConnectDefaultMidiIn();
            }

            forceDefaultMidiOut = MIDISettings.instance.forceDefaultMidiOut;
            defaultMidiOut = MIDISettings.instance.defaultMidiOut;

            void ConnectDefaultMidiOut() { if (defaultMidiOut >= 0 && defaultMidiOut < MidiOUTPlugin.GetDeviceCount()) { MidiOUTPlugin.ConnectDevice(defaultMidiOut); } }
            
            if (forceDefaultMidiOut) ConnectDefaultMidiOut();
            else
            {
                if(MIDISettings.instance.restoreMidiOutConnections) MidiOUTPlugin.RestoreConnections();
                if (MidiOUTPlugin.connectedDevices.Count == 0) ConnectDefaultMidiOut();                
            }
        }

        static void InitSynth()
        {
            Synth.Init();
            Synth.Start();
        }
    }
}