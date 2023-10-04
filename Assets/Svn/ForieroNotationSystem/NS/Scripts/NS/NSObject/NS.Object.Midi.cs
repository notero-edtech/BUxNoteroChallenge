/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;
using ForieroEngine.MIDIUnified.Plugins;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class NSObject
    {
        public struct MidiMessage
        {
            public int Channel, Command, Data1, Data2;
            public float Time, Duration;
            public void SetDuration(float d) => this.Duration = d;
                       
            public MidiMessage(MidiMessage m)
            {
                this.Channel = m.Channel;
                this.Command = m.Command;
                this.Data1 = m.Data1;
                this.Data2 = m.Data2;
                this.Time = m.Time;
                this.Duration = m.Duration;
            }
        }
        
        public class MidiMessageData
        {
            public readonly MidiMessage Message;
            public int Channel => Message.Channel;
            public int Command => Message.Command;
            public int Data1 => Message.Data1;
            public int Data2 => Message.Data2;
            public float Time => Message.Time;
            public float Duration => Message.Duration;
            public float EndTime => Message.Time + Message.Duration;
            
            private float TimeRT => _measurementStartTimeRT + (NSPlayback.Midi.MeasurementOffset); 
            private float EndTimeRT => _measurementStartTimeRT + (Duration + NSPlayback.Midi.MeasurementOffset); 

            private static MidiINPlugin.DSP.Data[] Tone => MidiINPlugin.DSP.Tone;
            private static MidiINPlugin.DSP.Data[] ControlChange => MidiINPlugin.DSP.ControlChange;

            //private static float RunningTime => NSPlayback.Time.iTimeProvider.GetTime();
            private static float RunningTime => ((float)AudioSettings.dspTime - NSPlayback.Time.DSP.startTime) * NSPlayback.speed;

            public float ToneOnDiff()
            {
                //Debug.Log($"ToneOnDiff => {_userTimeOnRT} - {TimeRT} = {_userTimeOnRT - TimeRT}");
                return _userTimeOnRT - TimeRT;
            }

            public int ToneOnDiffMS => Mathf.RoundToInt(ToneOnDiff() * 1000f);

            public float ToneOffDiff()
            {
                //Debug.Log($"ToneOffDiff => {_userTimeOffRT} - {EndTimeRT} = {_userTimeOffRT - EndTimeRT}");
                return _userTimeOffRT - EndTimeRT;
            }

            public int ToneOffDiffMS => Mathf.RoundToInt(ToneOffDiff() * 1000f);

            public static Action<MidiMessageData> OnToneOnFeedbackThreaded;
            public static Action<MidiMessageData> OnToneOffFeedbackThreaded;
            
            public void Measure()
            {
                //if (!Command.IsToneON()) return;
                //Debug.Log($"Measure {RunningTime} = ({AudioSettings.dspTime} - {NSPlayback.Time.DSP.startTime}) * {NSPlayback.speed}");
                _measurementStartTimeRT = RunningTime + Mathf.Clamp(-NSPlayback.Midi.MeasurementOffset + RunningTime, -NSPlayback.Midi.MeasurementOffset , 0);
                _userTimeOnRT = _userTimeOffRT = float.MinValue;
            }
            
            public void Passed(bool v)  => _passedTimeRT = v ? RunningTime : float.MinValue;
            public MidiMessageData(MidiMessage m) { this.Message = m; } 
            
            private volatile float _passedTimeRT = float.MinValue;
            private volatile float _measurementStartTimeRT = float.MinValue;
            private volatile float _userTimeOnRT = float.MinValue;
            private volatile float _userTimeOffRT = float.MinValue;

            public void ResetTimes()
            {
                _passedTimeRT = float.MinValue;
                _measurementStartTimeRT = float.MinValue;
                _userTimeOnRT = float.MinValue;
                _userTimeOffRT = float.MinValue;
            }
            public bool HasBeenPressedRT => _userTimeOnRT > float.MinValue;
            public bool HasBeenReleasedRT => _userTimeOffRT > float.MinValue;

            public FeedbackEnum ToneOnFeedback => _userTimeOnRT > float.MinValue ? ToneOnDiff().GetFeedbackEnum() : FeedbackEnum.Undefined;
            public FeedbackEnum ToneOffFeedback => _userTimeOffRT > float.MinValue ? ToneOffDiff().GetFeedbackEnum() : FeedbackEnum.Undefined;
            
            public void UpdateMeasurement()
            {
                if (!Command.IsToneON()) return;
                if (_userTimeOnRT > float.MinValue && _userTimeOffRT > float.MinValue) return;
                
                switch (Tone[Data1].On)
                {
                    case true when _userTimeOnRT <= float.MinValue && NSPlayback.Midi.MeasuredTone[Data1] == null:
                        if (NSPlayback.Midi.FindClosestMidiMessageData(Data1, true) != this) break;
                        NSPlayback.Midi.MeasuredTone[Data1] = this;
                        _userTimeOnRT = RunningTime;
                        OnToneOnFeedbackThreaded?.Invoke(this);
                        break;
                    case false when _userTimeOnRT > float.MinValue && _userTimeOffRT <= float.MinValue && this == NSPlayback.Midi.MeasuredTone[Data1]:
                        NSPlayback.Midi.MeasuredTone[Data1] = null;
                        _userTimeOffRT = RunningTime;
                        OnToneOffFeedbackThreaded?.Invoke(this);
                        break;
                }
            }
        }
        
        public class MidiData
        {
            //private static float RunningTime => NSPlayback.Time.iTimeProvider.GetTime();
            private static float RunningTime => ((float)AudioSettings.dspTime - NSPlayback.Time.DSP.startTime) * NSPlayback.speed;
            
            public readonly List<MidiMessageData> messages = new ();
            public void Add(MidiMessage m) => messages.Add(new (m));
            
            private volatile bool _passed = false;
            public bool passed
            {
                get => _passed;
                set
                {
                    _passed = value; 
                    Passed(value);
                }
            }
            public volatile bool played = false;
            
            public float time = 0;
            public float duration = 0;
            public float endTime => time + duration;
            public bool noteOn = false;

            private volatile bool _measure = false;
            public bool Measure
            {
                get => _measure;
                set
                {
                    if (value) { MeasureInternal(); }
                    _measure = value;
                }
            }

            protected volatile float dspTimeRT = 0;
            protected volatile float timeRT = 0;

            public void Reset()
            {
                messages.Clear();
                _passed = false;
                played = false;
                duration = 0;
                time = 0f;
                noteOn = false;
                dspTimeRT = 0;
            }

            public void ResetTimes()
            {
                for (var i = 0; i < messages.Count; i++)
                {
                    messages[i].ResetTimes();
                    if (messages[i].Command.IsToneON()) NSPlayback.Midi.MeasuredTone[messages[i].Data1] = null;
                }
            }

            private void Passed(bool v)
            {
                dspTimeRT = v ? (float)AudioSettings.dspTime : 0f;
                timeRT = v ? RunningTime : 0f;
                for (var i = 0; i < messages.Count; i++) { messages[i].Passed(v); }
            }
            private void MeasureInternal() { for (var i = 0; i < messages.Count; i++) { messages[i].Measure(); } }
            public void UpdateMeasurement() { for (var i = 0; i < messages.Count; i++) messages[i].UpdateMeasurement(); }
        }
       
        public readonly MidiData midiData = new ();
    }
}
