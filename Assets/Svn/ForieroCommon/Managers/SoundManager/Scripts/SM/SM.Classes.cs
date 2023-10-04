using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ForieroEngine;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public partial class SM : MonoBehaviour
{
    public enum Provider
    {
        Default,
        Undefined = int.MaxValue
    }

    [System.Serializable]
    public class SoundItem
    {
        public string id = "";
        public AudioClip clip = null;
        [Range(0f, 1f)]
        public float volume = 1f;
        public bool loop = false;
    }
       
    public enum Tab
    {
        Self,
        Bank
    }
}