using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Utils;
using System;

public class MidiDispatcher : MonoBehaviour, IMidiSender
{

    public static MidiDispatcher singleton;

    public string id = "";
    public string Id => id;
    public event ShortMessageEventHandler ShortMessageEvent;

    class MidiMessage
    {
        public int midiIndex;
        public int channel;
        public int volume;
        public float duration;
        public float delay;
        public bool played;
        public Action started;
        public Action finished;
        public bool remove = false;
        public int deviceId;
    }


    static List<MidiMessage> dispatchedNotes = new List<MidiMessage>(20);

    void OnEnable()
    {
        singleton = this;
    }

    void Awake()
    {
        singleton = this;
    }

    void OnDestroy()
    {
        singleton = null;
    }

    public static void DispatchNote(int midiIndex, int volume, int channel, int deviceId = -1, float duration = 0f, float delay = 0f, Action started = null, Action finished = null)
    {
        SetupDispatcher();

        var m = new MidiMessage()
        {
            midiIndex = midiIndex,
            channel = channel,
            volume = volume,
            duration = duration,
            delay = delay,
            played = delay <= 0f ? true : false,
            started = started,
            finished = finished,
            deviceId = deviceId
        };

        if (m.delay <= 0f)
        {
            MidiOut.NoteOn(m.midiIndex, m.volume, m.channel);
            singleton.ShortMessageEvent?.Invoke(m.channel + CommandEnum.MIDI_NOTE_ON.ToInt(), m.midiIndex, m.volume, deviceId);
            started?.Invoke();
            if (duration > 0) dispatchedNotes.Add(m);
        }
        else
        {
            dispatchedNotes.Add(m);
        }
    }

    public static void SetupDispatcher()
    {
        if (singleton == null)
        {
            GameObject go = new GameObject();
            go.transform.name = "MIDIUnified Dispatcher";
            singleton = go.AddComponent<MidiDispatcher>();
        }
    }

    void Update()
    {
        for (int i = 0; i < dispatchedNotes.Count; i++)
        {
            MidiMessage m = dispatchedNotes[i];
            if (!m.played)
            {
                m.delay -= Time.deltaTime;
                if (m.delay <= 0)
                {
                    MidiOut.NoteOn(m.midiIndex, m.volume, m.channel);

                    ShortMessageEvent?.Invoke(m.channel + CommandEnum.MIDI_NOTE_ON.ToInt(), m.midiIndex, m.volume, m.deviceId);

                    m.played = true;

                    m.started?.Invoke();

                    if (m.duration <= 0) m.remove = true;
                }
            }
            else
            {
                m.duration -= Time.deltaTime;
            }

            if (m.duration <= 0f && !m.remove)
            {
                MidiOut.NoteOff(m.midiIndex, m.channel);

                ShortMessageEvent?.Invoke(m.channel + CommandEnum.MIDI_NOTE_OFF.ToInt(), m.midiIndex, 0, m.deviceId);

                m.finished?.Invoke();

                m.remove = true;
            }
        }

        for (int i = dispatchedNotes.Count - 1; i >= 0; i--)
        {
            if (dispatchedNotes[i].duration <= 0f)
            {
                dispatchedNotes.RemoveAt(i);
            }
        }
    }
}
