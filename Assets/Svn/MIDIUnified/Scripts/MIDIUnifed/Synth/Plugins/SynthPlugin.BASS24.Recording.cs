using UnityEngine;

#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Midi;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.AddOn.Enc;
#if UN4SEEN_BASS_MISC
using Un4seen.Bass.Misc;
#endif
#endif

#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
using System;
using System.Runtime.InteropServices;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified.Recording;

namespace ForieroEngine.MIDIUnified.Plugins
{
    public static partial class BASS24SynthPlugin
    {
        static int recordingStream = 0;
        public static AudioSourceBass24 audioSourceBass24;

        private static void CreateAudioSourceBass24()
        {
            if (!audioSourceBass24)
            {
                audioSourceBass24 = new GameObject("Bass24AudioSource").AddComponent<AudioSourceBass24>();
                UnityEngine.Object.DontDestroyOnLoad(audioSourceBass24.gameObject);
            }
        }

        private static void DestroyAudioSource()
        {
            if (!audioSourceBass24) return;

            UnityEngine.Object.Destroy(audioSourceBass24.gameObject);
            audioSourceBass24 = null;
        }

        private static void InitRecordingClip(AudioClip clip, float volume)
        {
            if (!audioSourceBass24) return;

            audioSourceBass24.Init(clip, true);
            audioSourceBass24.volume = volume;
        }

#if UN4SEEN_BASS_MISC
        static EncoderWAV wavRecorder;

        public static void StartRecording(AudioClip clip = null, float volume = 1f){
            if(wavRecorder != null){
                Debug.LogError("FileStream alread open. Seems like recording session is in the proccess!");
                return;
            }

            string path = Recorders.SynthR.fileName.PrependPersistentPath();

            wavRecorder = new EncoderWAV(stream);
            wavRecorder.InputFile = null;
            // will be a 32-bit IEEE float WAVE file, since the stream is float //
            wavRecorder.OutputFile = path;  
            wavRecorder.Start(null, IntPtr.Zero, false);
        }

        public static void StopRecording(){
            if (wavRecorder == null) return;
            wavRecorder.Stop();
            wavRecorder = null;
        }
#else
        public static void StartRecording(AudioClip clip = null, float volume = 1f, float speed = 1f, int semitone = 0)
        {
            if (midiStream == 0) return;

            if (recordingStream != 0)
            {
                Debug.LogError("FileStream alread open. Seems like recording session is in the proccess!");
                return;
            }

            var path = Recorders.Synth.fileName.PrependPersistentPath();
            Debug.Log("Saving : " + path);

            if (clip)
            {
                CreateAudioSourceBass24();
                InitRecordingClip(clip, volume);
                audioSourceBass24.speed = speed;
                audioSourceBass24.semitone = semitone;
                audioSourceBass24.Play();

                if (!BassMix.BASS_Mixer_StreamAddChannel(mixerStream, audioSourceBass24.clipBass24.streamFX, BASSFlag.BASS_DEFAULT))
                {
                    Debug.LogError("BassMix.BASS_Mixer_StreamAddChannel(mixerStream, audioSourceBass24.clipBass24.streamFX, BASSFlag.BASS_DEFAULT " + Bass.BASS_ErrorGetCode().ToString());
                }

                BassMix.BASS_Mixer_ChannelSetPosition(audioSourceBass24.clipBass24.streamFX, 0);

                recordingStream = BassEnc.BASS_Encode_Start(mixerStream, path, BASSEncode.BASS_ENCODE_PCM, null, System.IntPtr.Zero);
            }
            else
            {
                recordingStream = BassEnc.BASS_Encode_Start(midiStream, path, BASSEncode.BASS_ENCODE_PCM, null, System.IntPtr.Zero);
            }

            if (recordingStream == 0) Debug.LogError("BASS24 : BASS_Encode_Start " + Bass.BASS_ErrorGetCode().ToString());
        }

        public static void StopRecording()
        {
            if (recordingStream == 0) return;
            BassEnc.BASS_Encode_Stop(recordingStream);
            recordingStream = 0;
            if (audioSourceBass24 && audioSourceBass24.clipBass24 != null)
            {
                BassMix.BASS_Mixer_ChannelRemove(audioSourceBass24.clipBass24.streamFX);
            }
            audioSourceBass24?.Stop();
        }
#endif
    }
}
#else
namespace ForieroEngine.MIDIUnified.Plugins
{
    public static partial class BASS24SynthPlugin
    {
        public static AudioSourceBass24 audioSourceBass24;

        static void CreateAudioSourceBass24()
        {

        }

        static void DestroyAudioSource()
        {

        }

        static void InitRecordingClip(AudioClip clip, float volume)
        {

        }

        public static void StartRecording(AudioClip clip = null, float volume = 1f, float speed = 1f, int semitone = 0)
        {

        }

        public static void StopRecording()
        {

        }
    }
}
#endif
