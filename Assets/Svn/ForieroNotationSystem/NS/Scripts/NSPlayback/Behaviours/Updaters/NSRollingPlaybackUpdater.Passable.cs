/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;
using UnityEngine.Assertions;

public partial class NSRollingPlaybackUpdater : NSUpdaterBase
{
    private bool _passed;
    
    public void InitPassables()
    {
        var midiObjects = nsBehaviour.ns.GetMidiObjectsSortedByPixelTime<NSObject>();
        NSPlayback.Midi.SetMidiObjects(midiObjects);
        passableObjects = nsBehaviour.ns.GetPassableObjectsSortedByPixelTime<NSObject>();
        staveFixedObjects = nsBehaviour.ns.GetObjectsOfType<NSStave>(true).Where(x => x.pool == PoolEnum.NS_FIXED).ToList();
        keySignatureObjects = nsBehaviour.ns.GetPassableObjectsSortedByPixelTime<NSKeySignature>();
        timeSignatureObjecs = nsBehaviour.ns.GetPassableObjectsSortedByPixelTime<NSTimeSignature>();
        clefObjects = nsBehaviour.ns.GetPassableObjectsSortedByPixelTime<NSClef>();
        metronomeMarkObjects = nsBehaviour.ns.GetPassableObjectsSortedByPixelTime<NSMetronomeMark>();

        passableIndexCursor = 0;
        midiIndexCursor = 0;
        nsObject = null;
        
        stavePositionStates = new();
        foreach (var stave in staveFixedObjects)
        {
            var state = new StaveMidiState(
                stave.options.index,
                midiObjects.Where(o => o.midiData.noteOn && o.stave?.options != null && o.stave.options.index == stave.options.index).OrderBy(o=>o.pixelTime).ToList()
                );
            stavePositionStates.Add(state);
        }
        
        InitSignatures();        
    }

    public void InitSignatures()
    {
        if (staveFixedObjects.Count == 0)
        {
            NSPlayback.SetTimeSignatureAndInvoke(0, 0, new NSTimeSignature.Options() { timeSignatureEnum = TimeSignatureEnum.Common, timeSignatureStruct = new TimeSignatureStruct(4, 4) });
            NSPlayback.OnKeySignatureChanged?.Invoke(0, 0, new NSKeySignature.Options() { keySignatureEnum = KeySignatureEnum.CMaj_AMin, keyModeEnum = KeyModeEnum.Major });
            NSPlayback.OnMetronomeMarkChanged?.Invoke(0, 0, new NSMetronomeMark.Options() { beatsPerMinute = 120, noteEnum = NoteEnum.Quarter, dots = 0 });
        }

        for (var i = 0; i < staveFixedObjects.Count; i++)
        {
            nsStave = staveFixedObjects[i];

            Assert.IsNotNull(nsStave.IsNotNull());
            Assert.IsNotNull(nsStave.part.IsNotNull());
            Assert.IsNotNull(nsStave.timeSignature.IsNotNull());
            Assert.IsNotNull(nsStave.keySignature.IsNotNull());
            //Assert.IsNotNull(nsStave.metronomeMark.IsNotNull());

            nsStave.Arrange(NSPlayback.NSRollingPlayback.rollingMode.ToHorizontalDirectionEnum());

            NSPlayback.SetTimeSignatureAndInvoke(nsStave.part.options.index, nsStave.options.index, nsStave.timeSignature.options);
            NSPlayback.OnKeySignatureChanged?.Invoke(nsStave.part.options.index, nsStave.options.index, nsStave.keySignature.options);
            if (nsStave.metronomeMark) NSPlayback.OnMetronomeMarkChanged?.Invoke(nsStave.part.options.index, nsStave.options.index, nsStave.metronomeMark.options);
        }
    }

    public void ResetPassables()
    {
        passableIndexCursor = 0;
        midiIndexCursor = 0;
        nsObject = null;
        
        passableObjects.ForEach(o => o.passed = false);
        NSPlayback.Midi.ResetPassable();
        
        nsBehaviour.ResetAllControllers();
        nsBehaviour.AllSoundOff();
        nsBehaviour.ClearSynthQueue();

        foreach (var kv in NSPlayback.Interaction.midiChannelSound)
        {
            if (!kv.Value) continue;
            for(var i = 0; i<128; i++) nsBehaviour.SendMidiMessage(kv.Key, 0x80, i, 0);
        }

        UpdateStaves();
    }

    public override void UpdatePassables()
    {
        base.UpdatePassables();
        UpdateObjects(passableObjects, ref passableIndexCursor, null, null);
        UpdateStaves();
    }
    
    private void UpdateObjects(IReadOnlyList<NSObject> objects, ref int index, Action<NSObject> onPassed, Action<NSObject> onUnPassed)
    {
        if (objects.Count <= 0) return;

        var pixelTime = NSPlayback.NSRollingPlayback.pixelPosition;

        index = Mathf.Clamp(index, 0, objects.Count);

        for (var i = index - 1; i >= 0; i--){
            if (objects[i].passed && objects[i].pixelTimeZoomed > pixelTime - 1)
            {
                objects[i].passed = false;
                onUnPassed?.Invoke(objects[i]);
                index = i;
            }
            else break;
        }

        for (var i = index; i < objects.Count; i++)
        {
            if (!objects[i].passed && objects[i].pixelTimeZoomed <= pixelTime - 1)
            {
                objects[i].passed = true;
                onPassed?.Invoke(objects[i]);
                index++;
            }
            else break;
        }
    }

    private void UpdateStaves()
    {
        var arrange = false;
        for (var i = 0; i < staveFixedObjects.Count; i++)
        {
            arrange = false;
            nsStave = staveFixedObjects[i];

            for (var k = clefObjects.Count - 1; k >= 0; k--)
            {
                nsClef = clefObjects[k];
                if (nsClef.stave != nsStave) continue;
                if (!nsClef.passed && k != 0) continue;
                if (!nsStave.clef || nsStave.clef.options.clefEnum != nsClef.options.clefEnum)
                {
                    switch (NSPlayback.NSRollingPlayback.rollingMode)
                    {
                        case NSPlayback.NSRollingPlayback.RollingMode.Left:
                        case NSPlayback.NSRollingPlayback.RollingMode.Right:
                            NSClef._options.CopyValuesFrom(nsClef.options);
                            NSClef._options.changing = false;
                            nsStave.SetClef(NSClef._options);
                            break;
                        case NSPlayback.NSRollingPlayback.RollingMode.Top:
                        case NSPlayback.NSRollingPlayback.RollingMode.Bottom:
                            nsStave.clef = nsClef;
                            break;
                    }
                    arrange = true;
                }
                break;
            }

            for (var k = metronomeMarkObjects.Count - 1; k >= 0; k--)
            {
                nsMetronomeMark = metronomeMarkObjects[k];
                if (nsMetronomeMark.stave != nsStave) continue;
                if (!nsMetronomeMark.passed && k != 0) continue;
                if (!nsStave.metronomeMark || !nsStave.metronomeMark.options.Same(nsMetronomeMark.options))
                {
                    switch (NSPlayback.NSRollingPlayback.rollingMode)
                    {
                        case NSPlayback.NSRollingPlayback.RollingMode.Left:
                        case NSPlayback.NSRollingPlayback.RollingMode.Right:
                            nsStave.SetMetronomeMark(nsMetronomeMark.options);
                            break;
                        case NSPlayback.NSRollingPlayback.RollingMode.Top:
                        case NSPlayback.NSRollingPlayback.RollingMode.Bottom:
                            nsStave.metronomeMark = nsMetronomeMark;
                            break;

                    }
                    NSPlayback.OnMetronomeMarkChanged?.Invoke(nsStave.part.options.index, nsStave.options.index, nsMetronomeMark.options);
                    arrange = true;
                }
                break;
            }

            for (var k = timeSignatureObjecs.Count - 1; k >= 0; k--)
            {
                nsTimeSignature = timeSignatureObjecs[k];
                if (nsTimeSignature.stave != nsStave) continue;
                if (!nsTimeSignature.passed && k != 0) continue;
                if (!nsStave.timeSignature || nsStave.timeSignature.options.timeSignatureEnum != nsTimeSignature.options.timeSignatureEnum
                                           || nsStave.timeSignature.options.timeSignatureStruct.numerator != nsTimeSignature.options.timeSignatureStruct.numerator
                                           || nsStave.timeSignature.options.timeSignatureStruct.denominator != nsTimeSignature.options.timeSignatureStruct.denominator)
                {
                    switch (NSPlayback.NSRollingPlayback.rollingMode)
                    {
                        case NSPlayback.NSRollingPlayback.RollingMode.Left:
                        case NSPlayback.NSRollingPlayback.RollingMode.Right:
                            NSTimeSignature._options.CopyValuesFrom(nsTimeSignature.options);
                            NSTimeSignature._options.changing = false;
                            nsStave.SetTimeSignature(NSTimeSignature._options);
                            break;
                        case NSPlayback.NSRollingPlayback.RollingMode.Top:
                        case NSPlayback.NSRollingPlayback.RollingMode.Bottom:
                            nsStave.timeSignature = nsTimeSignature;
                            break;

                    }
                    NSPlayback.SetTimeSignatureAndInvoke(nsStave.part.options.index, nsStave.options.index, nsTimeSignature.options);
                    arrange = true;
                }
                break;
            }

            for (var k = keySignatureObjects.Count - 1; k >= 0; k--)
            {
                nsKeySignature = keySignatureObjects[k];
                if (nsKeySignature.stave != nsStave) continue;
                if (!nsKeySignature.passed && k != 0) continue;
                if (!nsStave.keySignature || nsStave.keySignature.options.keySignatureEnum != nsKeySignature.options.keySignatureEnum
                                          || nsStave.keySignature.options.keyModeEnum != nsKeySignature.options.keyModeEnum)
                {
                    switch (NSPlayback.NSRollingPlayback.rollingMode)
                    {
                        case NSPlayback.NSRollingPlayback.RollingMode.Left:
                        case NSPlayback.NSRollingPlayback.RollingMode.Right:
                            NSKeySignature._options.CopyValuesFrom(nsKeySignature.options);
                            NSKeySignature._options.changing = false;
                            nsStave.SetKeySignature(NSKeySignature._options);
                            break;
                        case NSPlayback.NSRollingPlayback.RollingMode.Top:
                        case NSPlayback.NSRollingPlayback.RollingMode.Bottom:
                            nsStave.keySignature = nsKeySignature;
                            break;

                    }
                    NSPlayback.OnKeySignatureChanged?.Invoke(nsStave.part.options.index, nsStave.options.index, nsKeySignature.options);
                    arrange = true;
                }
                break;
            }

            if (arrange)
            {
                switch (NSPlayback.NSRollingPlayback.rollingMode)
                {
                    case NSPlayback.NSRollingPlayback.RollingMode.Left:
                    case NSPlayback.NSRollingPlayback.RollingMode.Right:
                        nsStave.Arrange(NSPlayback.NSRollingPlayback.rollingMode.ToHorizontalDirectionEnum());
                        break;
                }
            }
        }
    }
    private void UpdateStaveMidiStates() { for (var i = 0; i < stavePositionStates.Count; i++) { stavePositionStates[i].Update(); } }
}
