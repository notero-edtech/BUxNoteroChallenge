using System;
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified.Midi;
using Sanford.Multimedia.Midi;
using UnityEngine;
using MidiEvent = ForieroEngine.MIDIUnified.Midi.MidiEvent;

namespace ForieroEngine.MIDIUnified.Recording
{
    public static partial class Recorders
    {
        public static class MidiEvents
        {
            public class MidiEventsRecorder: IDisposable
            {
                public readonly string fileName = "midievents.mid";

                MidiInternalClock clock = new MidiInternalClock();
                RecordingSession session;
                MIDIUnified.MidiEvents midiEvents = new MIDIUnified.MidiEvents();

                private void OnDisable() { StopRecording(); }
                private void OnDestroy() { StopRecording(); }

                public void StartRecording(IMidiSender sender)
                {
                    clock.Stop();

                    midiEvents = new MIDIUnified.MidiEvents();
                    midiEvents.AddSender(sender);

                    midiEvents.ShortMessageEvent += (int aCommand, int aData1, int aData2, int aDeviceId) => {
                        if (session == null) return;
                        session.Record(new ChannelMessage((ChannelCommand)aCommand.ToMidiCommand(), aCommand.ToMidiChannel(), aData1, aData2));
                    };

                    session = new RecordingSession(clock);
                    clock.Start();
                }

                public MidiEventsRecorder(string fileName){
                    this.fileName = fileName;
                }

                public void Dispose(){
                    StopRecording();
                }

                public void StopRecording()
                {
                    if (session == null) return;

                    clock.Stop();
                    midiEvents.Dispose();
                    midiEvents = null;

                    session.Build();

                    var track = session.Result;

                    MidiEventCollection eventCollection = new MidiEventCollection(0, clock.Ppqn);
                    var events = eventCollection.AddTrack();
                    
                    long lastAbsoluteTime = 0;
                    
                    foreach (var e in track.Iterator())
                    {
                        if (e.MidiMessage.MessageType == MessageType.Channel)
                        {
                            var bytes = e.MidiMessage.GetBytes();
                            if (bytes.Length == 3)
                            {
                                Array.Resize(ref bytes, 4);
                                bytes[3] = 0;

                                var midiEvent = MidiEvent.FromRawMessage(BitConverter.ToInt32(bytes, 0));
                                lastAbsoluteTime = midiEvent.AbsoluteTime = e.AbsoluteTicks;
                                events.Add(midiEvent);
                            }
                        }
                    }
                    
                    events.Add(new MetaEvent(MetaEventType.EndTrack, 0, lastAbsoluteTime + 1));
                    
                    eventCollection.PrepareForExport();

                    session = null;

                    string path = fileName.PrependPersistentPath();
                    Debug.Log("Saving : " + path);
                    MidiFile.Export(path, eventCollection);
                }
            }

            public static string fileName = "midievents.mid";
            public static List<MidiEventsRecorder> recorders = new List<MidiEventsRecorder>();

            public static void Start(IMidiSender sender, string fileName = null)
            {
                if (string.IsNullOrEmpty(fileName)) fileName = MidiEvents.fileName;

                var recorder = recorders.FirstOrDefault(r => r.fileName == fileName);
                if (recorder == null)
                {
                    recorder = new MidiEventsRecorder(fileName);
                    recorders.Add(recorder);
                    recorder.StartRecording(sender);
                }
            }

            public static void Stop(string fileName = null)
            {
                if (string.IsNullOrEmpty(fileName)) fileName = MidiEvents.fileName;

                var recorder = recorders.FirstOrDefault(r => r.fileName == fileName);
                if (recorder != null){
                    recorder.StopRecording();
                    recorders.Remove(recorder);
                    recorder.Dispose();
                }
            }
        }
    }
}
