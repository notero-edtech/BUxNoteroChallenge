using System;
using System.Collections.Generic;
using UnityEngine;

#if FMOD
using FMOD;
using FMOD.Studio;
#endif


public class IntegratorTool2DSpineAudioClipsScriptableObject : ScriptableObject
{
    [Serializable]
    public class Clip{
        public string spineEventName;
        public string spineAudioPath;
        public AudioClip audioClip;
        
        public string FMODEventName => spineEventName.Replace("audio/", "");
        
        #if FMOD
        [NonSerialized] public bool EventDescriptionCached = false;  
        [NonSerialized] public EventDescription EventDescription;
        [NonSerialized] public GUID Guid;
        #endif
        public string WwiseEventName => spineEventName.Replace("audio/", "");
    }
    
	public List<Clip> clips = new List<Clip>();
}
