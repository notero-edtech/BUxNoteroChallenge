using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Interfaces;
using UnityEngine;
#if FMOD
using FMODUnity;
#endif

public partial class MidiSeqKaraoke : IMidiSender
{
    public AudioSourceWrapper MusicInterface { get; } = new AudioSourceWrapper();
    public AudioSourceWrapper VocalsInterface { get; } = new AudioSourceWrapper();

    public class AudioSourceWrapper
    {
        public bool Enabled { get; set; } = false;
        public IAudioSource AudioInterface { get; set; } 
        
        public bool IsValid => Enabled && AudioInterface != null;

        public AudioSourceState GetState()
        {
            if (!IsValid) return AudioSourceState.Stopped;
            return AudioInterface.State;
        }

        public void Play()
        {
            if (!IsValid) return;
            AudioInterface?.Play();
        }

        public void Stop()
        {
            if (!IsValid) return;
            AudioInterface?.Stop();
        }

        public void Pause()
        {
            if (!IsValid) return;
            AudioInterface?.Pause();
        }

        public double GetTime()
        {
            if (!IsValid) return 0;
            return AudioInterface.Time;
        }

        public void SetTime(double t)
        {
            if (!IsValid) return;
            AudioInterface.Time = (float) t;
        }

        public void Mute()
        {
            if (!IsValid) return;
            AudioInterface.Mute = true;
        }

        public void UnMute()
        {
            if (!IsValid) return;
            AudioInterface.Mute = false;
        }

        public void SetVolume(float volume)
        {
            if (!IsValid) return;
            AudioInterface.Volume = volume;
        }

        public float GetVolume()
        {
            if (!IsValid) return 1;
            return AudioInterface.Volume;
        }

        public void SetClip(AudioClip clip) => AudioInterface?.Init(clip);
        public void SetClip(string clip) => AudioInterface?.Init(clip);
#if FMOD
        public void SetClip(EventReference clip) => AudioInterface?.Init(clip);
#endif

        public void SetSemitone(int t)
        {
            if (!IsValid) return;
            AudioInterface.Semitone = t;
        }

        public void SetSpeed(float s)
        {
            if (!IsValid) return;
            AudioInterface.Speed = s;
        }
    }
}