using ForieroEngine.Extensions;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public partial class SM : MonoBehaviour
{
    public static void SetMixerGroupVolume(string mixerGroupId, float volume)
    {
        if (SoundSettings.instance.audioMixer)
        {
            SoundSettings.instance.audioMixer.SetFloat(mixerGroupId, volume.ToDecibel());
        }
    }
}