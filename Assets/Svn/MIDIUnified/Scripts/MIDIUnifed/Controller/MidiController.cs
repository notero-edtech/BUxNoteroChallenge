using System;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified.Interfaces;
using UnityEngine;

namespace ForieroEngine.MIDIUnified.Classes
{
    public abstract class MidiController<T> : MonoBehaviour where T : MidiController<T>, IMidiController, IMidiSender, IMidiReceiver
    {
        public static MidiController<T> Instance;
        public string id;
        public RectTransform containerRT;
        public string Id => id;
        public ControllerAlignment Alignment { get; private set; } = ControllerAlignment.Bottom;
        public bool Colored { get; set; } = true;
        public Transform Transform => this.transform;
        public RectTransform RectTransform => this.transform as RectTransform;

        public IMidiKeys Keys { get; } = new MidiKeys();
        public IMidiPercussions Percussions { get; } = new MidiPercussions();
        public IMidiButtons Buttons { get; } = new MidiButtons();
        public IMidiKnobs Knobs { get; } = new MidiKnobs();
        public IMidiFaders Faders { get; } = new MidiFaders();
      
        public event ShortMessageEventHandler ShortMessageEvent;
        protected virtual void OnShortMessageEvent(int aCommand, int aData1, int aData2, int aDeviceId) => ShortMessageEvent?.Invoke(aCommand, aData1, aData2, aDeviceId);
        public virtual void OnMidiMessageReceived(int aCommand, int aData1, int aData2, int aDeviceId) { }
        
        protected virtual void Awake()
        {
            Debug.Log("Midi Controller ( assigned ) : " + typeof(T).Name);
            (this as IMidiController).Register();
            Keys.SetId(Id); Percussions.SetId(Id); Buttons.SetId(Id); Knobs.SetId(Id); Faders.SetId(Id);
            Instance = this;
        }
        protected virtual void OnDestroy()
        {
            (this as IMidiController).Unregister();
            if (Instance != this) return;
            Debug.Log("Midi Controller ( released ) : " + typeof(T).Name); 
            Instance = null;
        }

        public bool Hidden { get; protected set; } = false;

        public virtual void Show(bool animated = true) { }
        public virtual void Hide(bool animated = true) { }

        public virtual void Align(ControllerAlignment alignment)
        {
            switch (alignment)
            {
                case ControllerAlignment.Right:
                case ControllerAlignment.Left:
                case ControllerAlignment.Bottom:
                    containerRT.pivot = new Vector2(0.5f, 0f);
                    break;
                case ControllerAlignment.Top:
                    containerRT.pivot = new Vector2(0.5f, 1f);
                    break;
            }
            Alignment = alignment;
        }

        private readonly Vector3[] _corners = new Vector3[4];
        public virtual Vector3 GetWorldPosition(ControllerPosition p)
        {
            RectTransform.GetWorldCorners(_corners);
            switch (p)
            {
                case ControllerPosition.Left: return _corners[0].SetY(_corners[1].y - _corners[0].y);
                case ControllerPosition.Right: return _corners[3].SetY(_corners[2].y - _corners[3].y);;
                case ControllerPosition.Top: return _corners[1].SetX(_corners[2].x - _corners[1].x);
                case ControllerPosition.Bottom: return _corners[0].SetX(_corners[3].x - _corners[0].x);
            }
            return Vector3.zero;
        }
    }
}
