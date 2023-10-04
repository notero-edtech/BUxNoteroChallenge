using ForieroEngine.MIDIUnified.Interfaces;
using UnityEngine;

namespace ForieroEngine.MIDIUnified.Classes
{
    public abstract class MidiInstrument<T> : MonoBehaviour where T : MidiInstrument<T>, IMidiInstrument, IMidiSender, IMidiReceiver
    {
        public static MidiInstrument<T> Instance;
        
        public string id;
        public string Id => id;

        protected virtual void Awake() { Instance = this; }
        protected virtual void OnDestroy() { Instance = null; }
        
        public virtual void Show(bool animated = true) { }
        public virtual void Hide(bool animated = true) { }
        
        public event ShortMessageEventHandler ShortMessageEvent;
        protected virtual void OnShortMessageEvent(int aCommand, int aData1, int aData2, int aDeviceId) => ShortMessageEvent?.Invoke(aCommand, aData1, aData2, aDeviceId);
        public virtual void OnMidiMessageReceived(int aCommand, int aData1, int aData2, int aDeviceId) { }
    }
}
