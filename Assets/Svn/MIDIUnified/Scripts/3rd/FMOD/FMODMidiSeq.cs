using System.Collections;
using System.Collections.Generic;
using ForieroEngine.MIDIUnified;
using UnityEngine;

public class FMODMidiSeq : MonoBehaviour, IMidiSender
{
    public string id = "";

    public string Id => id;

    [HideInInspector] 
    public MidiSeqKaraoke midiSeq = new MidiSeqKaraoke();

    public void Initialize(byte[] midiBytes, double startTime, string name = null)
    {
        midiSeq = new MidiSeqKaraoke(id, name);
        midiSeq.ShortMessageEvent += (commnad, data1, data2, id) => ShortMessageEvent?.Invoke(commnad, data1, data2, id);
        midiSeq.synchronizationContext = MidiSeqKaraoke.SynchronizationContext.Manual;
        midiSeq.timelineStartTime = startTime;
        midiSeq.pickUpBar = false;
        midiSeq.pickUpBarOnRepeat = false;
        midiSeq.Initialize(midiBytes);
        midiSeq.Play(false);
    }

    public void ManualUpdate(double t) { if(midiSeq.initialized) midiSeq.Update(t); }
    
    public event ShortMessageEventHandler ShortMessageEvent;
}
