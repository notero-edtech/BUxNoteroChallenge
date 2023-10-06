using UnityEngine;

namespace Notero.Unity.AudioModule
{
    public class LocalAudioClip : CloudAudioClip
    {
        public LocalAudioClip(string filepath, AudioType audioType) : base("file://" + filepath, audioType) { }
    }
}
