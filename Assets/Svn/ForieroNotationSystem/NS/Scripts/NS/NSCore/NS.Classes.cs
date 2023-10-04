/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    [Serializable] public class Margins
    {
        public float right;
        public float left;
        public float top;
        public float bottom;
        public void Reset() { right = left = top = bottom = 0; }
    }

    [Serializable] public class MarginsLeftRight
    {
        public float right;
        public float left;
        public void Reset() { right = left = 0; }
    }
    
    [Serializable] public class MarginsTopBottom
    {
        public float top;
        public float bottom;
        public void Reset() { top = bottom = 0; }
    }
    
    [Serializable] public class Padding
    {
        public float right;
        public float left;
        public float top;
        public float bottom;
        public void Reset() { right = left = top = bottom = 0; }
    }
    
    [Serializable] public class PaddingLeftRight
    {
        public float right;
        public float left;
        public void Reset() { right = left = 0; }
    }
    
    [Serializable] public class PaddingTopBottom
    {
        public float top;
        public float bottom;
        public void Reset() { top = bottom = 0; }
    }
    
    [Serializable] public class PageLayout
    {
        public Margins margins;
        public int width;
        public int height;
    }
    
    [Serializable] public class SystemLayout
    {
        public MarginsLeftRight margins;
        public float distance;
        public float topSystemDistance;
    }
    
    [Serializable] public class Session
    {
        public string name;
        [TextArea] public string description;
        [TextArea] public string instructions;
        public SystemEnum system = SystemEnum.Undefined;
        public HandsEnum handsEnum = HandsEnum.Both;
        public TextAsset xml;
        public TextAsset midi;
        public TimeProvider timeProvider = TimeProvider.Unknown;
        public AudioProvider audioProvider = AudioProvider.Unknown;
        public AudioClip accompaniment;
    }
}
