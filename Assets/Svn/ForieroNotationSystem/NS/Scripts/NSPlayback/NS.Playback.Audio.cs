/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {
        public static class Audio
        {
            public static IAudioProvider iAudioProvider => AudioProviders.iAudioProvider;
           
            public static void Init(AudioProvider audioProvider, AudioClip clip)
            {
                switch (audioProvider)
                {
                    case AudioProvider.UnityAudioSource: AudioProviders.Init("NSUNITYAUDIO"); break;
                    //case AudioProvider.BASS24AudioSource: AudioProviders.Init("NSBASS24AUDIO"); break;
                    case AudioProvider.Unknown: AudioProviders.Init(""); break;
                }

                iAudioProvider?.InitAudioClip(clip);
            }
        }
    }
    
    public static class AudioProviders
    {
        public static IAudioProvider iAudioProvider { get; private set; }
        private static readonly List<IAudioProvider> _providers = new List<IAudioProvider>();
        public static void Register(this IAudioProvider i, bool ignoreNullOrEmptyId = true)
        {
            Debug.Log($"Registering Audio Provider:{i.Id}");
            if (string.IsNullOrEmpty(i.Id) && ignoreNullOrEmptyId) return;
            if (_providers.Contains(i)) Debug.LogError($"IMidiGenerator instance with id {i.Id} already exists!!!");
            else if(!string.IsNullOrEmpty(i.Id)) _providers.Add(i);
        }
        public static IAudioProvider GetById(string id) => _providers.FirstOrDefault(i => i != null && i.Id == id);

        public static void Unregister(this IAudioProvider i)
        {
            if (_providers.Contains(i))
            {
                if(i == iAudioProvider) iAudioProvider = null;
                _providers.Remove(i);
            }
        }
        public static void Init(string id)
        {
            iAudioProvider = GetById(id);
            foreach (var p in _providers)
            {
                if(p == iAudioProvider) p.EnableAudioProvider();
                else p.DisableAudioProvider();
            }
        }
    }
}
