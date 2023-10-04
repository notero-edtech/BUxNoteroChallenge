/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public abstract class NSUpdaterBase
{
    protected readonly NSBehaviour nsBehaviour;
    protected readonly NSBackgroundDrag nsBackgroundDrag;

    [Tooltip("Fixed camera for staves and vertical movement.")]
    protected readonly Camera fixedCamera;
    protected readonly RectTransform fixedCameraRT;

    [Tooltip("Movable camera for notation objects and horizontal movement.")]
    protected readonly Camera movableCamera;
    protected readonly RectTransform movableCameraRT;
    
    protected List<NSObject> passableObjects = new ();
    protected List<NSKeySignature> keySignatureObjects = new ();
    protected List<NSTimeSignature> timeSignatureObjecs = new ();
    protected List<NSClef> clefObjects = new ();
    protected List<NSMetronomeMark> metronomeMarkObjects = new ();

    protected NSObject nsObject;
    protected NSTimeSignature nsTimeSignature;
    protected NSKeySignature nsKeySignature;
    protected NSClef nsClef;
    protected NSMetronomeMark nsMetronomeMark;
    
    // PASSABLE OBJECTS //
    protected int passableIndexCursor = 0;
    
    // MIDI MESSAGES //
    protected int midiIndexCursor = 0;
    
    public bool blocked = true;
    
    protected static Dictionary<int, bool> Interactive => NSPlayback.Interaction.midiChannelInteractive;
    protected static Dictionary<int, bool> Sound => NSPlayback.Interaction.midiChannelSound;

    public NSUpdaterBase(NSBehaviour nsBehaviour)
    {
        Assert.IsNotNull(nsBehaviour);
        this.nsBehaviour = nsBehaviour;

        Assert.IsNotNull(nsBehaviour.fixedCamera);
        fixedCamera = nsBehaviour.fixedCamera;

        Assert.IsNotNull(nsBehaviour.fixedCameraRT);
        fixedCameraRT = nsBehaviour.fixedCameraRT;

        Assert.IsNotNull(nsBehaviour.movableCamera);
        movableCamera = nsBehaviour.movableCamera;
        Assert.IsNotNull(nsBehaviour.movableCameraRT);
        movableCameraRT = nsBehaviour.movableCameraRT;
                
        Assert.IsNotNull(nsBehaviour.backgroundDragRT);
        nsBackgroundDrag = nsBehaviour.backgroundDragRT.GetComponent<NSBackgroundDrag>();
        Assert.IsNotNull(nsBackgroundDrag);
    }

    public virtual void SubscribeEvents()
    {
        NSPlayback.OnPlaybackStateChanged += OnPlaybackStateChanged;
        NSPlayback.OnPlaybackModeChanged += OnPlaybackModeChanged;
        NSPlayback.NSRollingPlayback.OnRollingModeChanged += OnRollingModeChanged;
        NSPlayback.OnZoomChanged += OnZoomChanged;
        
        nsBackgroundDrag.OnBeginDragEvent += NSBackgroundDrag_OnBeginDragEvent;
        nsBackgroundDrag.OnDragEvent += NSBackgroundDrag_OnDragEvent;
        nsBackgroundDrag.OnEndDragEvent += NSBackgroundDrag_OnEndDragEvent;
    }
    
    public virtual void UnsubscribeEvents()
    {
        NSPlayback.OnPlaybackStateChanged -= OnPlaybackStateChanged;
        NSPlayback.OnPlaybackModeChanged -= OnPlaybackModeChanged;
        NSPlayback.NSRollingPlayback.OnRollingModeChanged -= OnRollingModeChanged;
        NSPlayback.OnZoomChanged -= OnZoomChanged;
    
        nsBackgroundDrag.OnBeginDragEvent -= NSBackgroundDrag_OnBeginDragEvent;
        nsBackgroundDrag.OnDragEvent -= NSBackgroundDrag_OnDragEvent;
        nsBackgroundDrag.OnEndDragEvent -= NSBackgroundDrag_OnEndDragEvent;
    }

    protected abstract void OnZoomChanged(float obj);
    protected abstract void NSBackgroundDrag_OnBeginDragEvent(PointerEventData obj);
    protected abstract void NSBackgroundDrag_OnDragEvent(PointerEventData obj);
    protected abstract void NSBackgroundDrag_OnEndDragEvent(PointerEventData obj);
    protected virtual void OnPlaybackStateChanged(NSPlayback.PlaybackState state)
    {
        if (blocked) return;

        switch (state)
        {
            case NSPlayback.PlaybackState.Pickup: break;
            case NSPlayback.PlaybackState.Playing: NSPlayback.Midi.ClearWaitForObjects(); break;
            case NSPlayback.PlaybackState.WaitingForInput: break;
            case NSPlayback.PlaybackState.Pausing: break;
            case NSPlayback.PlaybackState.Stop: break;
            case NSPlayback.PlaybackState.Finished: break;
            case NSPlayback.PlaybackState.Undefined: break;
        }
    }

    protected abstract void OnPlaybackModeChanged(NSPlayback.PlaybackMode playbackMode);
    protected abstract void OnRollingModeChanged(NSPlayback.NSRollingPlayback.RollingMode rollingMode);

    private NSObject.MidiMessage _m;
    public virtual void UpdatePassables()
    {
        if (NSPlayback.Midi.waitForInput)
        {
            NSPlayback.WaitForInput(); 
            NSPlayback.Midi.waitForInput = false;
        }

        if (NSPlayback.Midi.play)
        {
            NSPlayback.Play();
            NSPlayback.Midi.play = false;
        }
        
        while (NSPlayback.Midi.InvokeMidiEvents.Dequeue(ref _m)) { nsBehaviour.SendMidiMessage(_m.Channel, _m.Command, _m.Data1, _m.Data2); }
    }
}
