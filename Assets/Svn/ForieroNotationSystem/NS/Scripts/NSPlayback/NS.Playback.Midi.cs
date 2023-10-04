/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ForieroEngine.Collections.NonBlocking;
using ForieroEngine.MIDIUnified.Synthesizer;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;
using ThreadPriority = System.Threading.ThreadPriority;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {
        public static partial class Midi
        {
            public const float MeasurementOffset = 1f;
            public const float MeasurementMissedOffset = 0.75f;
            
            public static readonly NSObject.MidiMessageData[] MeasuredTone = new NSObject.MidiMessageData[128];
            public static void ResetMeasuredTone()
            {
                for (var i = 0; i < 128; i++) { MeasuredTone[i] = null; } 
            }

            public static volatile bool waitForInput = false;
            public static volatile bool play = false;
            public static readonly NonBlockingQueue<NSObject.MidiMessage> InvokeMidiEvents = new();
            
            private static Thread _thread = null;
            private static int _sleep = 2;
            private static List<NSObject> _midiObjects = new();
            private static int _index = 0;
            private static int _userInputIndex = 0;
            private static volatile bool _resetPassable = false;
            private static volatile bool _clearWaitForObjects = false;
            private static readonly List<NSObject> _waitForInputObjects = new();
            private static readonly List<NSObject> _measureObjects = new();
            // this is for finding fast the closest MidiMessageData for a specific tone midi index //
            private static readonly List<NSObject>[] _measureToneObjects = new List<NSObject>[128];
            
            private static NSObject.MidiMessageData _closestMidiMessageData = null;
            private static NSObject.MidiMessageData _midiMessageData = null;
            private static float _closestTime = -1;
            
            //private static float RunningTime => Time.iTimeProvider.GetTime();
            private static float RunningTime => ((float)AudioSettings.dspTime - NSPlayback.Time.DSP.startTime) * NSPlayback.speed;
            public static NSObject.MidiMessageData FindClosestMidiMessageData(int index, bool on)
            {
                if (MeasuredTone[index] != null) return null;

                _closestMidiMessageData = null;
                _closestTime = float.MaxValue;
                _midiMessageData = null;
                for (var i = 0; i < _measureToneObjects[index].Count; i++)
                {
                    for (var m = 0; m < _measureToneObjects[index][i].midiData.messages.Count; m++)
                    {
                        _midiMessageData = _measureToneObjects[index][i].midiData.messages[m];
                        if (!(Math.Abs(RunningTime - (on ? _midiMessageData.Time : _midiMessageData.EndTime)) < _closestTime)) continue;
                        _closestTime = Math.Abs(RunningTime - (on ? _midiMessageData.Time : _midiMessageData.EndTime));
                        _closestMidiMessageData = _midiMessageData;
                    }
                }
                return _closestMidiMessageData;
            }
            
            public static void ResetPassable() { if (_thread == null) ResetPassableInternal(); else _resetPassable = true; }
            public static void ClearWaitForObjects() { if(_thread == null) ClearWaitForObjectsInternal(); else _clearWaitForObjects = true; }

            public static void  SetMidiObjects(List<NSObject> objects)
            {
                _thread?.Abort();
                _thread?.Join();
                _midiObjects = objects;

                for (var i = 0; i < 128; i++)
                {
                    if(_measureToneObjects[i] == null) _measureToneObjects[i] = new();
                    else _measureToneObjects[i].Clear();
                } 
                
                ResetPassableInternal();
                if (_midiObjects == null) { _thread = null; return; }
                _thread = new Thread(() =>
                {
                    while (true)
                    {
                        if (_resetPassable) ResetPassableInternal();
                        if (_clearWaitForObjects) ClearWaitForObjectsInternal();
                        if (NSPlayback.playbackState is PlaybackState.Playing or PlaybackState.WaitingForInput )
                        {
                            UpdateMeasurementObjects(_midiObjects, ref _userInputIndex, OnAddMeasurementObject, OnRemoveMeasurementObject);
                        }
                        UpdateMeasurementEndObjects();
                        
                        UpdateObjects(_midiObjects, ref _index, OnMidiPassed, OnMidiUnPassed);

                        if (NSPlayback.playbackState is PlaybackState.Playing or PlaybackState.WaitingForInput)
                        {
                            UpdateWaitForInput();
                        }
                        
                        Thread.Sleep(_sleep);
                    }
                });
                _thread.Priority = ThreadPriority.Highest;
                _thread.Start();
            }

            private static void UpdateMeasurementEndObjects()
            {
                if (playbackState is not (PlaybackState.Playing or PlaybackState.WaitingForInput)) return;
                
                for (var i = _measureObjects.Count - 1; i >= 0; i--)
                {
                    var remove = true;
                    if (_measureObjects[i].midiData.endTime + MeasurementOffset < Time.time)
                    {
                         for (var m = 0; m < _measureObjects[i].midiData.messages.Count; m++)
                         {
                             _m = _measureObjects[i].midiData.messages[m];
                             if (MeasuredTone[_m.Data1] == _m) { remove = false; }
                             else { _measureToneObjects[_m.Data1].Remove(_measureObjects[i]); }
                         }

                         if (remove)
                         {
                             _measureObjects.RemoveAt(i);
                             continue;
                         }
                    }
                    _measureObjects[i].midiData.UpdateMeasurement();
                }
            }
            
            private static void OnAddMeasurementObject(NSObject o)
            {
                if (!o.midiData.noteOn) return;
                _measureObjects.Add(o);
                for (var m = 0; m < o.midiData.messages.Count; m++)
                {
                    _measureToneObjects[o.midiData.messages[m].Data1].Add(o);
                }
            }
            
            private static void OnRemoveMeasurementObject(NSObject o)
            {
                if (!o.midiData.noteOn) return;
                if (playbackState is (PlaybackState.Playing or PlaybackState.WaitingForInput)) return;
                
                _measureObjects.Remove(o);
                for (var m = 0; m < o.midiData.messages.Count; m++)
                {
                    _measureToneObjects[o.midiData.messages[m].Data1].Remove(o);
                    MeasuredTone[o.midiData.messages[m].Data1] = null;
                }
            }

            private static void ResetPassableInternal()
            {
                ResetMeasuredTone();
                _midiObjects?.ForEach(o => o.midiData.passed = o.midiData.played = o.midiData.Measure = false);
                _waitForInputObjects?.Clear();
                _index = 0; 
                _measureObjects?.Clear();
                _userInputIndex = 0;
                _resetPassable = false;
            }

            private static void ClearWaitForObjectsInternal()
            {
                _waitForInputObjects?.ForEach(o => o.midiData.passed = true);
                _waitForInputObjects?.Clear();
                _clearWaitForObjects = false;
            }
            
            private static void UpdateMeasurementObjects(IReadOnlyList<NSObject> objects, ref int index, Action<NSObject> onAdd, Action<NSObject> onRemove)
            {
                if (objects.Count <= 0) return;
                
                index = Mathf.Clamp(index, 0, objects.Count);
                
                for (var i = index - 1; i >= 0; i--){
                    if (objects[i].midiData.Measure && (objects[i].midiData.time - MeasurementOffset) > Time.time)
                    {
                        objects[i].midiData.Measure = false;
                        onRemove?.Invoke(objects[i]);
                        index = i;
                    }
                    else break;
                }
                
                for (var i = index; i < objects.Count; i++)
                {
                    if (!objects[i].midiData.Measure && (objects[i].midiData.time - MeasurementOffset) <= Time.time)
                    {
                        objects[i].midiData.Measure = true;
                        onAdd?.Invoke(objects[i]);
                        index++;
                    }
                    else break;
                }
            }
            
            private static void UpdateObjects(IReadOnlyList<NSObject> objects, ref int index, Action<NSObject> onPassed, Action<NSObject> onUnPassed)
            {
                if (objects.Count <= 0) return;
                
                index = Mathf.Clamp(index, 0, objects.Count);

                for (var i = index - 1; i >= 0; i--){
                    if (objects[i].midiData.passed && objects[i].midiData.time > Time.time + (Time.time >= Time.TotalTime ? 0.001 : -0.001f))
                    {
                        objects[i].midiData.passed = false;
                        onUnPassed?.Invoke(objects[i]);
                        index = i;
                    }
                    else break;
                }

                for (var i = index; i < objects.Count; i++)
                {
                    if (!objects[i].midiData.passed && objects[i].midiData.time <= Time.time + (Time.time >= Time.TotalTime ? 0.001 : -0.001f))
                    {
                        objects[i].midiData.passed = true;
                        onPassed?.Invoke(objects[i]);
                        index++;
                    }
                    else break;
                }
            }

            private static NSObject.MidiMessageData _m;
            private static bool _allNotesOn = true;
            private static NSObject _o = null;
            private static Dictionary<int, bool> Interactive => Interaction.midiChannelInteractive;
            private static Dictionary<int, bool> Sound => Interaction.midiChannelSound;
            private static void OnMidiPassed(NSObject o)
            {
                if (o.midiData.messages.Count == 0) { return; }
                _m = o.midiData.messages.First();
                if (!Interactive[_m.Channel]) { PlayMidiMessages(o); return; }
                if (!Interaction.waitForInput) { PlayMidiMessages(o); return; }
        
                if (playbackState is not (PlaybackState.Playing or PlaybackState.WaitingForInput)) return;
                _waitForInputObjects.Insert(0,o);
            }
    
            private static void OnMidiUnPassed(NSObject o)
            {
                o.midiData.passed = false;
                UnPlayMidiMessages(o);
                o.midiData.ResetTimes();
            }
            
            private static void UpdateWaitForInput()
            {
                if (waitForInput) return;
                if (playbackState is not (PlaybackState.Playing or PlaybackState.WaitingForInput)) { _waitForInputObjects.Clear(); return; }
                if (!Interaction.waitForInput) { _waitForInputObjects.Clear(); return; }
                
                _allNotesOn = true;
                for (var i = _waitForInputObjects.Count - 1; i >= 0; i--)
                {
                    _o = _waitForInputObjects[i];
            
                    for (var mi = 0; mi < _o.midiData.messages.Count; mi++)
                    {
                        _m = _o.midiData.messages[mi];
                        if (_m.Command.IsToneON())
                        {
                            switch (playbackState)
                            {
                                case PlaybackState.Playing:
                                    if (Interaction.IsNoteOn(_m.Data1) && !_m.HasBeenPressedRT)
                                    {
                                        waitForInput = true; return;
                                    }
                                    if (!Interaction.IsNoteOn(_m.Data1))
                                    {
                                        waitForInput = true; return;
                                    } 
                                    break;
                                case PlaybackState.WaitingForInput:
                                    if (Interaction.IsNoteOn(_m.Data1) && !_m.HasBeenPressedRT)
                                    {
                                        _allNotesOn = false; return;
                                    }
                                    if (!Interaction.IsNoteOn(_m.Data1))
                                    {
                                        _allNotesOn = false; return;
                                    }
                                    break;
                            }
                        }
                    }
                }
                if (!_allNotesOn) return;
                _waitForInputObjects.Clear();
                if(playbackState == PlaybackState.WaitingForInput) play = true;
            }

            private static void PlayMidiMessages(NSObject mo)
            {
                if (mo.midiData.played) return;
                mo.midiData.played = true;
                for (var m = 0; m < mo.midiData.messages.Count; m++)
                {
                    if (Sound[mo.midiData.messages[m].Message.Channel] && !Interactive[mo.midiData.messages[m].Message.Channel])
                    {
                        switch (playbackState)
                        {
                            default:
                                InvokeMidiEvents.Enqueue(mo.midiData.messages[m].Message);
                                Synth.SendShortMessage(mo.midiData.messages[m].Message.Command, mo.midiData.messages[m].Message.Channel, mo.midiData.messages[m].Message.Data1, mo.midiData.messages[m].Message.Data2);
                                break;
                        }
                    }
                }
                
            }

            
            private static void UnPlayMidiMessages(NSObject mo)
            {
                if (!mo.midiData.played) return;
                mo.midiData.played = false;
                
                for (var m = 0; m < mo.midiData.messages.Count; m++)
                {
                    if (Sound[mo.midiData.messages[m].Message.Channel] && !Interactive[mo.midiData.messages[m].Message.Channel])
                    {
                        switch (mo.midiData.messages[m].Message.Command)
                        {
                            case 0x90:
                                switch (playbackState)
                                {
                                    default:
                                        InvokeMidiEvents.Enqueue(new NSObject.MidiMessage(mo.midiData.messages[m].Message){ Command =  0x80, Data2 = 0 });
                                        Synth.SendShortMessage(0x80, mo.midiData.messages[m].Message.Channel, mo.midiData.messages[m].Message.Data1, 0);
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}
