/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;
using ForieroEngine.Music.MusicXML.Xsd;

namespace ForieroEngine.Music.MusicXML.Sequencer
{
    public class MidiSequencer
    {
        //--Variables
        public MidiFile _MidiFile;
        private List<byte> blockList;
        private bool playing = false;
        private bool looping = false;
        //private MidiSequencerEvent seqEvt;
        private int sampleTime;
        private int eventIndex;
        private int sampleRate;

        //--Events

        public delegate void NoteOnEventHandler(int channel, int note, int velocity);
        public event NoteOnEventHandler NoteOnEvent;

        public delegate void NoteOffEventHandler(int channel, int note);
        public event NoteOffEventHandler NoteOffEvent;

        public delegate void ShortMessageEventHandler(int Command, int Data1, int Data2, note Note);
        public event ShortMessageEventHandler ShortMessageEvent;

        public delegate void ControlEventHandler();
        public event ControlEventHandler PlayEvent;
        public event ControlEventHandler StopEvent;
        //		public event ControlEventHandler PauseEvent;
        //		public event ControlEventHandler ContinueEvent;
        public event ControlEventHandler FinishedEvent;

        //--Public Properties
        public bool isPlaying
        {
            get { return playing; }
        }
        public int SampleTime
        {
            get { return sampleTime; }
        }
        public int EndSampleTime
        {
            get { return (int)_MidiFile.Tracks[0].TotalTime; }
        }
        public TimeSpan EndTime
        {
            get { return new TimeSpan(0, 0, (int)SynthHelper.getTimeFromSample(sampleRate, (int)_MidiFile.Tracks[0].TotalTime)); }
        }
        public TimeSpan Time
        {
            get { return new TimeSpan(0, 0, (int)SynthHelper.getTimeFromSample(sampleRate, sampleTime)); }
            set { SetTime(value); }
        }
        public bool Looping
        {
            get { return looping; }
            set { looping = value; }
        }
        //--Public Methods
        public MidiSequencer(int aSampleRate)
        {
            sampleRate = aSampleRate;
            blockList = new List<byte>();
            //seqEvt = new MidiSequencerEvent();
        }

        public bool LoadMusicXML(byte[] bytes)
        {
            if (playing == true)
                return false;
            _MidiFile = new MidiFile(bytes);
            if (_MidiFile.SequencerReady == false)
            {
                _MidiFile.CombineTracks();
                //Convert delta time to sample time
                eventIndex = 0;
                double lastSample = 0;
                for (int x = 0; x < _MidiFile.Tracks[0].MidiEvents.Length; x++)
                {
                    _MidiFile.Tracks[0].MidiEvents[x].deltaTime = DeltaTimetoSamples((double)_MidiFile.Tracks[0].MidiEvents[x].totalTime + 960);
                    if (_MidiFile.Tracks[0].MidiEvents[x].midiMetaEvent == MidiHelper.MidiMetaEvent.Tempo)
                    {
                        _MidiFile.BeatsPerMinute = MidiHelper.MicroSecondsPerMinute / System.Convert.ToDouble(_MidiFile.Tracks[0].MidiEvents[x].Parameters[0]);
                    }
                }
                //Set total time to proper value
                _MidiFile.Tracks[0].TotalTime = _MidiFile.Tracks[0].MidiEvents[_MidiFile.Tracks[0].MidiEvents.Length - 1].deltaTime;
                _MidiFile.SequencerReady = true;
            }
            blockList.Clear();
            return true;
        }

        public void Play()
        {
            if (playing == true)
            {
                return;
            }
            //set bpm
            _MidiFile.BeatsPerMinute = 120;
            //Let the synth know that the sequencer is ready.
            eventIndex = 0;
            if (PlayEvent != null) PlayEvent();
            playing = true;
        }

        public void Stop(bool immediate)
        {
            if (StopEvent != null) StopEvent();
            playing = false;
            sampleTime = 0;
            //            if (immediate)
            //                synth.NoteOffAll(true);
            //            else
            //                synth.NoteOffAll(false);
        }

        public void Pause()
        {
            playing = false;
        }

        public void Continue()
        {
            playing = true;
        }

        public bool isChannelMuted(int channel)
        {
            if (blockList.Contains((byte)channel))
                return true;
            return false;
        }
        public void MuteChannel(int channel)
        {
            if (channel > -1 && channel < 16)
                if (blockList.Contains((byte)channel) == false)
                    blockList.Add((byte)channel);
        }
        public void UnMuteChannel(int channel)
        {
            if (channel > -1 && channel < 16)
                blockList.Remove((byte)channel);
        }
        public void MuteAllChannels()
        {
            for (int x = 0; x < 16; x++)
                blockList.Add((byte)x);
        }
        public void UnMuteAllChannels()
        {
            blockList.Clear();
        }
        public void ProcessFrame(float deltaTime)
        {
            ProcessFrame((int)(deltaTime * sampleRate));
        }
        public void ProcessFrame(int framesperBuffer)
        {
            MidiSequencerEvent seqEvt = new MidiSequencerEvent();
            if (isPlaying)//Use sequencer
            {
                //stop or loop
                if (sampleTime >= (int)_MidiFile.Tracks[0].TotalTime)
                {
                    sampleTime = 0;
                    if (looping == true)
                    {
                        //set bpm
                        _MidiFile.BeatsPerMinute = 120;
                        //Let the synth know that the sequencer is ready.
                        eventIndex = 0;
                    }
                    else
                    {
                        playing = false;
                        if (FinishedEvent != null)
                        {
                            FinishedEvent();
                        }
                        return;
                    }
                }
                while (eventIndex < _MidiFile.Tracks[0].EventCount && _MidiFile.Tracks[0].MidiEvents[eventIndex].deltaTime < (sampleTime + framesperBuffer))
                {
                    seqEvt.Events.Add(_MidiFile.Tracks[0].MidiEvents[eventIndex]);
                    //Debug.Log(seqEvt.Events.Count);
                    eventIndex++;
                }

                if (seqEvt == null) return;

                int oldtime = 0;
                int waitTime = 0;
                for (int x = 0; x < seqEvt.Events.Count; x++)
                {

                    oldtime = oldtime + waitTime;
                    //Now process the event
                    ProcessMidiEvent(seqEvt.Events[x]);
                }

                IncrementSampleCounter(framesperBuffer);
            }
        }
        public void IncrementSampleCounter(int framesperBuffer)
        {
            sampleTime = sampleTime + framesperBuffer;
        }

        public MidiEvent currentMidiEvent;

        public void ProcessMidiEvent(MidiEvent midiEvent)
        {
            if (midiEvent.midiChannelEvent != MidiHelper.MidiChannelEvent.None)
            {
                if (blockList.Contains(midiEvent.channel)) return;
                if (ShortMessageEvent != null)
                {
                    currentMidiEvent = midiEvent;
                    ShortMessageEvent(midiEvent.channel + (int)midiEvent.midiChannelEvent, midiEvent.parameter1, midiEvent.parameter2, midiEvent.note);
                }
                switch (midiEvent.midiChannelEvent)
                {
                    case MidiHelper.MidiChannelEvent.Note_On:
                        if (this.NoteOnEvent != null)
                            this.NoteOnEvent(midiEvent.channel, midiEvent.parameter1, midiEvent.parameter2);
                        break;
                    case MidiHelper.MidiChannelEvent.Note_Off:
                        if (this.NoteOffEvent != null)
                            this.NoteOffEvent(midiEvent.channel, midiEvent.parameter1);
                        break;
                }
            }
            else
            {
                switch (midiEvent.midiMetaEvent)
                {
                    case MidiHelper.MidiMetaEvent.Tempo:
                        _MidiFile.BeatsPerMinute = (double)MidiHelper.MicroSecondsPerMinute / System.Convert.ToDouble(midiEvent.Parameters[0]);
                        break;
                    default:
                        break;
                }
            }
        }
        //--Private Methods
        private int DeltaTimetoSamples(double DeltaTime)
        {
            return SynthHelper.getSampleFromTime(sampleRate, (DeltaTime * (60.0 / ((_MidiFile.BeatsPerMinute) * _MidiFile.MidiHeader.DeltaTiming))));
        }
        private void SetTime(TimeSpan time)
        {
            int _stime = SynthHelper.getSampleFromTime(sampleRate, time.TotalSeconds);
            if (_stime > sampleTime)
            {
                SilentProcess(_stime - sampleTime);
            }
            else if (_stime < sampleTime)
            {
                //we have to restart the midi to make sure we get the right temp, instrument, etc
                sampleTime = 0;
                _MidiFile.BeatsPerMinute = 120;
                eventIndex = 0;
                SilentProcess(_stime);
            }
        }
        private void SilentProcess(int amount)
        {
            while (eventIndex < _MidiFile.Tracks[0].EventCount && _MidiFile.Tracks[0].MidiEvents[eventIndex].deltaTime < (sampleTime + amount))
            {
                if (_MidiFile.Tracks[0].MidiEvents[eventIndex].midiChannelEvent != MidiHelper.MidiChannelEvent.Note_On)
                    ProcessMidiEvent(_MidiFile.Tracks[0].MidiEvents[eventIndex]);
                eventIndex++;
            }
            sampleTime = sampleTime + amount;
        }
    }
}
