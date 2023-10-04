using System;
using System.Collections.Generic;
using UnityEngine;

public class IntegratorTool2DSpineMidiClipsScriptableObject : ScriptableObject
{
    [Serializable]
    public class Clip{
        public string spineEventName;
        public string spineAudioPath;
        public TextAsset midi;
    }
	public List<Clip> clips = new List<Clip>();
}
