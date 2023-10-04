using UnityEngine;
using System;
using System.Runtime.InteropServices;

#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Midi;
using Un4seen.Bass.AddOn.Mix;
//using Un4seen.Bass.AddOn.Enc;
#if UN4SEEN_BASS_MISC
using Un4seen.Bass.Misc;
#endif
#endif

#if ((UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 

namespace ForieroEngine.MIDIUnified.Plugins
{
    public static partial class BASS24SynthPlugin
    {
        static int soundFont = 0;
        //static BASS_MIDI_FONTINFO soundFontInfo;
        static int midiStream = 0;
        static int mixerStream = 0;

        static int sampleRate = 44100;

        public static int Start(int freq = 44100, int channels = 1)
        {
            if (midiStream != 0)
            {
                return 1;
            }

            if (MIDISettings.IsDebug) Debug.Log("MU | BASS24 | BASS24NETSynth - Start : " + freq.ToString() + " " + channels.ToString());
            sampleRate = freq;

            MIDISoundSettings.Init();

            if (string.IsNullOrEmpty(MIDISettings.soundFontPersistentPath))
            {
                return 0;
            }

            bool sampleFloat = true;

            midiStream = BassMidi.BASS_MIDI_StreamCreate(channels, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT, freq);

            if (midiStream == 0 && Bass.BASS_ErrorGetCode() == BASSError.BASS_ERROR_FORMAT)
            {
                midiStream = BassMidi.BASS_MIDI_StreamCreate(channels, BASSFlag.BASS_STREAM_DECODE, freq);

                if (midiStream == 0)
                {
                    Debug.LogError("MU | BASS24 | BassMidi.BASS_MIDI_StreamCreate " + Bass.BASS_ErrorGetCode().ToString());
                    return 0;
                }

                sampleFloat = false;
                if (MIDISettings.IsDebug) Debug.Log("MU | BASS24 | Bass channels are 16bit non-floatable.");
            }

            mixerStream = BassMix.BASS_Mixer_StreamCreate(sampleRate, 2, sampleFloat ? BASSFlag.BASS_SAMPLE_FLOAT : 0);
            if (mixerStream == 0)
            {
                Debug.LogError("MU | BASS24 | BassMix.BASS_Mixer_StreamCreate " + Bass.BASS_ErrorGetCode().ToString());
                return 0;
            }

            if (midiStream == 0)
            {
                Debug.LogError("MU | BASS24 | BASS_MIDI_StreamCreate " + Bass.BASS_ErrorGetCode().ToString());
                return 0;
            }

            Bass.BASS_ChannelSetSync(midiStream, BASSSync.BASS_SYNC_MIDI_EVENT | BASSSync.BASS_SYNC_MIXTIME, (long)BASSMIDIEvent.MIDI_EVENT_PROGRAM, null, System.IntPtr.Zero);
            Bass.BASS_ChannelSetAttribute(midiStream, BASSAttribute.BASS_ATTRIB_NOBUFFER, 1);

            int newSoundFont = BassMidi.BASS_MIDI_FontInit(MIDISettings.soundFontPersistentPath);
            if (MIDISettings.IsDebug) Debug.Log("MU | BASS24 | BASS_MIDI_FontInit : " + newSoundFont.ToString());

            if (!BassMidi.BASS_MIDI_FontSetVolume(newSoundFont, 2f))
            {
                Debug.LogError("MU | BASS24 | BASS_MIDI_FontSetVolume " + Bass.BASS_ErrorGetCode().ToString());
                return 0;
            }
            ;

            BASS_MIDI_FONT bassMidiFont = new BASS_MIDI_FONT();
            bassMidiFont.font = newSoundFont;
            bassMidiFont.preset = -1;
            bassMidiFont.bank = 0;

            if (!BassMidi.BASS_MIDI_StreamSetFonts(0, new BASS_MIDI_FONT[] { bassMidiFont }, 1))
            {
                Debug.LogError("MU | BASS24 | BASS_MIDI_StreamSetFonts " + Bass.BASS_ErrorGetCode().ToString());
                return 0;
            }

            if (!BassMidi.BASS_MIDI_StreamSetFonts(midiStream, new BASS_MIDI_FONT[] { bassMidiFont }, 1))
            {
                Debug.LogError("MU | BASS24 | BASS_MIDI_StreamSetFonts " + Bass.BASS_ErrorGetCode().ToString());
                return 0;
            }

            if (!BassMidi.BASS_MIDI_FontFree(soundFont))
            {

            }

            soundFont = newSoundFont;

            // soundFontInfo = BassMidi.BASS_MIDI_FontGetInfo(soundFont); //

            if (!BassMix.BASS_Mixer_StreamAddChannel(mixerStream, midiStream, BASSFlag.BASS_DEFAULT))
            {
                Debug.LogError("MU | BASS24 | BassMix.BASS_Mixer_StreamAddChannel(mixerStream, midiStream, BASSFlag.BASS_DEFAULT " + Bass.BASS_ErrorGetCode().ToString());
            }

            if (!Bass.BASS_ChannelPlay(mixerStream, false))
            {
                Debug.LogError("MU | BASS24 | BASS_ChannelPlay " + Bass.BASS_ErrorGetCode().ToString());
                return 0;
            }

            return 1;
        }

        public static int Stop()
        {
            if (mixerStream == 0) return 0;

            StopRecording();

            if (midiStream != 0)
            {
                BassMix.BASS_Mixer_ChannelRemove(midiStream);
                Bass.BASS_StreamFree(midiStream);
                midiStream = 0;
            }

            Bass.BASS_ChannelStop(mixerStream);
            Bass.BASS_StreamFree(mixerStream);
            mixerStream = 0;

            DestroyAudioSource();
            return 1;
        }

        public static int SendMidiMessage(int Command, int Data1, int Data2)
        {
            if (midiStream == 0) return 0;
            byte[] events = { (byte)Command, (byte)Data1, (byte)Data2 };
            int size = Marshal.SizeOf(events[0]) * events.Length;
            IntPtr p = Marshal.AllocHGlobal(size);
            Marshal.Copy(events, 0, p, events.Length);
            BassMidi.BASS_MIDI_StreamEvents(midiStream, BASSMIDIEventMode.BASS_MIDI_EVENTS_RAW | BASSMIDIEventMode.BASS_MIDI_EVENTS_NORSTATUS, 0, p, 3);
            Marshal.FreeHGlobal(p);
            return 1;
        }
    }
}

#endif
