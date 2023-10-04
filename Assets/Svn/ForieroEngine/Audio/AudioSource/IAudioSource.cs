using UnityEngine;
#if FMOD
using FMODUnity;
#endif

namespace ForieroEngine.MIDIUnified.Interfaces
{
       public interface IAudioSource
       {
              bool IsPlaying { get; }
              AudioSourceState State { get; }

              bool PlayOnAwake { get; set; }

              float Time { get; set; }
              float Speed { get; set; }
              int Semitone { get; set; }
              float Volume { get; set; }
              bool Mute { get; set; }
              
              void Play();
              void Pause();
              void Resume();
              void Stop();

              void Init(AudioClip clip);
              #if FMOD
              void Init(EventReference clip);
              #endif
              void Init(string clip);

              bool Initialized { get; set; }

              void PlayOneShot(AudioClip clip);
              void PlayOneShot(AudioClip clip, float volume);
              #if FMOD
              void PlayOneShot(EventReference clip, float volume);
              #endif
              void PlayOneShot(string clip, float volume);
              void Release();
       }

       public enum AudioSourceState
       {
              Stopped,
              Playing,
              Paused,
              Finished,
              //Undefined = int.MaxValue
       }
}
