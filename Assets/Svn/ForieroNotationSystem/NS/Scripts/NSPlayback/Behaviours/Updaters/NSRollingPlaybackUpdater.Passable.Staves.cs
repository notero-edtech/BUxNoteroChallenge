/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

public partial class NSRollingPlaybackUpdater : NSUpdaterBase
{
    private List<NSStave> staveFixedObjects = new ();
    private NSStave nsStave;
    
    public List<StaveMidiState> stavePositionStates = new ();
    public class StaveMidiState
    {
        public readonly int index = 0;
        public readonly List<NSObject> midiObjects = new ();
        
        public List<NSObject> current { get; } = new ();
        public List<NSObject> next { get; } = new();

        public StaveMidiState(int index, List<NSObject> midiObjects)
        {
            this.index = index;
            this.midiObjects = midiObjects;
        }
        
        private int _index = 0;
        
        public void Update()
        {
            if (midiObjects == null || midiObjects.Count == 0) return;

            _index = Mathf.Clamp(_index, 0, midiObjects.Count - 1);
            
            current.Clear();
            for (var i = _index; i >= 0; i--)
            {
                if (!midiObjects[i].midiData.passed) { _index = i; continue; }
                if (current.Count > 0 && current[0].pixelTime > midiObjects[i].pixelTime) break;
                current.Add(midiObjects[i]);
                _index = i;
            }

            switch (NSPlayback.NSRollingPlayback.rollingMode)
            {
                case NSPlayback.NSRollingPlayback.RollingMode.Left:
                case NSPlayback.NSRollingPlayback.RollingMode.Right:
                    current.Sort((a,b)=>a.rectTransform.anchoredPosition.y.CompareTo(b.rectTransform.anchoredPosition.y));
                    break;
                case NSPlayback.NSRollingPlayback.RollingMode.Top: 
                case NSPlayback.NSRollingPlayback.RollingMode.Bottom:
                    current.Sort((a,b)=>a.rectTransform.anchoredPosition.x.CompareTo(b.rectTransform.anchoredPosition.x));
                    break;
            }
            
            next.Clear();
            for (var i = _index; i < midiObjects.Count; i++)
            {
                if (midiObjects[i].midiData.passed) { _index = i; continue; };
                if (next.Count > 0 && next[0].pixelTime < midiObjects[i].pixelTime) break;
                next.Add(midiObjects[i]);
                _index = i;
            }
            
            switch (NSPlayback.NSRollingPlayback.rollingMode)
            {
                case NSPlayback.NSRollingPlayback.RollingMode.Left:
                case NSPlayback.NSRollingPlayback.RollingMode.Right:
                    next.Sort((a,b)=>a.rectTransform.anchoredPosition.y.CompareTo(b.rectTransform.anchoredPosition.y));
                    break;
                case NSPlayback.NSRollingPlayback.RollingMode.Top: 
                case NSPlayback.NSRollingPlayback.RollingMode.Bottom:
                    next.Sort((a,b)=>a.rectTransform.anchoredPosition.x.CompareTo(b.rectTransform.anchoredPosition.x));
                    break;
            }
        }
    }
}
