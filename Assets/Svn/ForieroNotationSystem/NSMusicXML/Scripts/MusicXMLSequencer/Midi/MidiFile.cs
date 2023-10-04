/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using ForieroEngine.Music.MusicXML.Xsd;

namespace ForieroEngine.Music.MusicXML.Sequencer
{
    public partial class MidiFile
    {
        //--Variables
        private bool sequenceReadyFormat = false;
        private MidiHeader mheader;
        private MidiTrack[] tracks;
        //--Public Properties
        public bool SequencerReady
        {
            get { return sequenceReadyFormat; }
            set { sequenceReadyFormat = value; }
        }
        public MidiTrack[] Tracks
        {
            get { return tracks; }
        }
        public double BeatsPerMinute
        {
            get { return tempo; }
            set
            {
                tempo = value;
            }
        }
        public int MicroSecondsPerQuarterNote
        {
            get { return TPQ; }
            set
            {
                TPQ = value;
            }
        }
        public MidiHeader MidiHeader
        {
            get { return mheader; }
        }
        //--Public Methods

        public MidiFile(byte[] bytes)
        {
            loadScorePartwise(bytes);
        }

        public List<MidiEvent> getAllMidiEventsofType(MidiHelper.MidiChannelEvent eventChannelType, MidiHelper.MidiMetaEvent eventMetaType)
        {
            List<MidiEvent> matchList = new List<MidiEvent>();
            for (int x = 0; x < tracks.Length; x++)
            {
                for (int i = 0; i < tracks[x].MidiEvents.Length; i++)
                {
                    if (tracks[x].MidiEvents[i].midiMetaEvent == eventMetaType && tracks[x].MidiEvents[i].midiChannelEvent == eventChannelType)
                        matchList.Add(tracks[x].MidiEvents[i]);
                }
            }
            return matchList;
        }
        public List<MidiEvent> getAllMidiEventsofType(byte channel, MidiHelper.MidiChannelEvent eventChannelType, MidiHelper.MidiMetaEvent eventMetaType)
        {
            List<MidiEvent> matchList = new List<MidiEvent>();
            for (int x = 0; x < tracks.Length; x++)
            {
                for (int i = 0; i < tracks[x].MidiEvents.Length; i++)
                {
                    if (tracks[x].MidiEvents[i].midiMetaEvent == eventMetaType
                        && tracks[x].MidiEvents[i].midiChannelEvent == eventChannelType
                        && tracks[x].MidiEvents[i].channel == channel)
                        matchList.Add(tracks[x].MidiEvents[i]);
                }
            }
            return matchList;
        }
        public void CombineTracks()
        {
            if (tracks.Length < 2)
                return;
            int total_eventCount = 0;
            UInt64 total_notesPlayed = 0;
            List<byte> programsUsed = new List<byte>();
            List<byte> DrumprogramsUsed = new List<byte>();
            //Loop to get track info
            for (int x = 0; x < tracks.Length; x++)
            {
                total_eventCount = total_eventCount + tracks[x].MidiEvents.Length;
                total_notesPlayed = total_notesPlayed + tracks[x].NotesPlayed;
                for (int x2 = 0; x2 < tracks[x].Programs.Length; x2++)
                {
                    if (programsUsed.Contains(tracks[x].Programs[x2]) == false)
                        programsUsed.Add(tracks[x].Programs[x2]);
                }
                for (int x2 = 0; x2 < tracks[x].DrumPrograms.Length; x2++)
                {
                    if (DrumprogramsUsed.Contains(tracks[x].DrumPrograms[x2]) == false)
                        DrumprogramsUsed.Add(tracks[x].DrumPrograms[x2]);
                }
            }
            //Now process the midi events
            Dictionary<double, LinkedList<MidiEvent>> OrderedTrack = new Dictionary<double, LinkedList<MidiEvent>>(total_eventCount);

            for (int x = 0; x < tracks.Length; x++)
            {
                double CurrentDeltaTime_ = 0;
                MidiEvent[] TrackSeq = new MidiEvent[tracks[x].MidiEvents.Length];
                tracks[x].MidiEvents.CopyTo(TrackSeq, 0);
                for (int x2 = 0; x2 < tracks[x].MidiEvents.Length; x2++)
                {
                    CurrentDeltaTime_ = CurrentDeltaTime_ + TrackSeq[x2].deltaTime;
                    TrackSeq[x2].deltaTime = CurrentDeltaTime_;
                    if (OrderedTrack.ContainsKey(TrackSeq[x2].deltaTime) == true)
                    {
                        LinkedList<MidiEvent> tmplist;
                        tmplist = OrderedTrack[TrackSeq[x2].deltaTime];
                        tmplist.AddLast(TrackSeq[x2]);
                    }
                    else
                    {
                        LinkedList<MidiEvent> tmplist = new LinkedList<MidiEvent>();
                        tmplist.AddLast(TrackSeq[x2]);
                        OrderedTrack.Add(TrackSeq[x2].deltaTime, tmplist);
                    }
                }
            }
            //Sort the Dictionary
            double[] keys = new double[OrderedTrack.Keys.Count];
            OrderedTrack.Keys.CopyTo(keys, 0);
            Array.Sort(keys);

            tracks = new MidiTrack[1];
            LinkedList<MidiEvent>[] ArrayofTrkEvts = new LinkedList<MidiEvent>[OrderedTrack.Values.Count];
            for (int x = 0; x < ArrayofTrkEvts.Length; x++)
                ArrayofTrkEvts[x] = OrderedTrack[keys[x]];
            OrderedTrack.Clear();
            keys = null;
            tracks[0] = new MidiTrack();
            tracks[0].Programs = programsUsed.ToArray();
            tracks[0].DrumPrograms = DrumprogramsUsed.ToArray();
            tracks[0].MidiEvents = new MidiEvent[total_eventCount];
            double PreviousDeltaTime = 0;
            uint cc = 0;
            for (int x = 0; x < ArrayofTrkEvts.Length; x++)
            {
                LinkedListNode<MidiEvent> tmpN = ArrayofTrkEvts[x].First;
                while (tmpN != null)
                {
                    double old1 = tmpN.Value.deltaTime;
                    tmpN.Value.deltaTime = (tmpN.Value.deltaTime - PreviousDeltaTime);
                    PreviousDeltaTime = old1;
                    tracks[0].MidiEvents[cc] = tmpN.Value;
                    tracks[0].TotalTime = tracks[0].TotalTime + (ulong)tmpN.Value.deltaTime;
                    tmpN = tmpN.Next;
                    cc++;
                }
            }
            tracks[0].NotesPlayed = (uint)total_notesPlayed;
        }

        double tempo = 60;
        decimal division = 1;
        public scorepartwise root;
        List<TrackHelper> staves = new List<TrackHelper>();

        private void loadScorePartwise(byte[] bytes)
        {
            mheader = new MidiHeader();
            mheader.DeltaTiming = 960;
            mheader.TimeFormat = MidiHelper.MidiTimeFormat.TicksPerBeat;
            mheader.MidiFormat = MidiHelper.MidiFormat.SingleTrack;


            XmlSerializer serializer = new XmlSerializer(typeof(scorepartwise), "MusicXML.Generated");
            MemoryStream stream = new MemoryStream(bytes);
            root = serializer.Deserialize(stream) as scorepartwise;

            MidiTrack track = new MidiTrack();
            List<MidiEvent> trackevents = new List<MidiEvent>();
            //Debug.Log(root.part.Count);
            foreach (scorepartwisePart part in root.part)
            {

                division = 1;

                staves.Clear();

                foreach (scorepartwisePartMeasure measure in part.measure)
                {
                    int staffIndex = -1;
                    foreach (object measureItem in measure.Items)
                    {
                        int voiceId = 0;
                        //ATTRIBUTES//
                        if (measureItem is attributes)
                        {
                            //DIVISIONS//
                            attributes attributes = measureItem as attributes;
                            division = attributes.divisions;
                            //STAFF COUNT//
                            int stavesCount = 1;
                            if (attributes.staves.Exists())
                            {
                                stavesCount = int.Parse(attributes.staves);
                                for (int k = 0; k < stavesCount; k++)
                                {
                                    staves.Add(new TrackHelper());
                                }
                            }
                        }

                        //DIRECTION//
                        if (measureItem is direction)
                        {
                            direction direction = measureItem as direction;
                            if (direction.sound != null)
                            {
                                tempo = (double)direction.sound.tempo;
                                Debug.Log(tempo);
                            }
                        }

                        if (measureItem is note)
                        {
                            note note = measureItem as note;
                            if (note.staff.Exists())
                            {
                                staffIndex = int.Parse(note.staff) - 1;
                            }
                            else
                            {
                                if (staffIndex == -1) Debug.LogError("stave does not exists");
                            }

                            if (note.ItemsElementName.Contains(ItemsChoiceType1.cue)) continue;

                            if (note.voice.Exists())
                            {
                                voiceId = int.Parse(note.voice);
                                UpdateStaff(note, staves[staffIndex], voiceId);
                            }
                            else
                            {
                                for (int j = 0; j < staves[staffIndex].voices.Count; j++)
                                {
                                    UpdateStaff(note, staves[staffIndex], j);
                                }
                            }

                        }
                    }

                    foreach (TrackHelper trackHelper in staves)
                    {
                        foreach (VoiceHelper vh in trackHelper.voices)
                        {
                            if (track.TotalTime < (double)vh.endMeasureDate) track.TotalTime = (double)vh.endMeasureDate;
                        }
                    }
                }

                foreach (TrackHelper trackHelper in staves)
                {
                    foreach (VoiceHelper vh in trackHelper.voices)
                    {
                        bool containsNote = false;
                        foreach (MidiEvent me in vh.midiEvents)
                        {
                            if (!me.note.ItemsElementName.Contains(ItemsChoiceType1.rest))
                            {
                                containsNote = true;
                                break;
                            }
                        }
                        if (containsNote) trackevents.AddRange(vh.midiEvents);
                    }
                }
            }

            trackevents.Sort(delegate (MidiEvent e1, MidiEvent e2)
            {
                if (e1.totalTime == e2.totalTime) return 0;
                else if (e1.totalTime > e2.totalTime) return 1;
                else return -1;
            });

            track.MidiEvents = trackevents.ToArray();
            track.DrumPrograms = new List<byte>().ToArray();
            track.Programs = new List<byte>().ToArray();
            tracks = new MidiTrack[1] { track };
        }


        void UpdateStaff(note note, TrackHelper trackHelper, int voiceId)
        {
            trackHelper.voices[voiceId].currentDate = trackHelper.voices[voiceId].endMeasureDate;
            trackHelper.voices[voiceId].duration = 0;
            if (note.ItemsElementName.Contains(ItemsChoiceType1.duration)) trackHelper.voices[voiceId].duration = convert2tick((decimal)note.Items[note.ItemsElementName.IndexOf(ItemsChoiceType1.duration)], division);
            decimal pitch = 0;
            int channel = 0;
            if (!note.ItemsElementName.Contains(ItemsChoiceType1.rest))
            {
                trackHelper.voices[voiceId].volume = note.dynamics;
                trackHelper.voices[voiceId].date = note.ItemsElementName.Contains(ItemsChoiceType1.chord) ? trackHelper.voices[voiceId].lastPosition : trackHelper.voices[voiceId].currentDate;
                if (note.ItemsElementName.Contains(ItemsChoiceType1.grace))
                {
                    return;
                    //										trackHelper.voices[voiceId].duration = (int)(TPQ / 6);
                    //										trackHelper.voices[voiceId].date -= trackHelper.voices[voiceId].duration;
                    //										if(date[i] < 0) date[i] = 0;
                    //										AddMidiEvent(trackHelper.voices[voiceId].midiEvents, trackHelper.voices[voiceId].date, channel, trackHelper.voices[voiceId].pitch, trackHelper.voices[voiceId].volume, trackHelper.voices[voiceId].duration);										
                }
                else if (!note.ItemsElementName.Contains(ItemsChoiceType1.tie))
                {

                    if (note.ItemsElementName.Contains(ItemsChoiceType1.pitch))
                    {
                        pitch = getMidiPitch(note);
                    }
                    else
                    {
                        pitch = 60;
                    }

                    AddMidiEvent(trackHelper.voices[voiceId].midiEvents, trackHelper.voices[voiceId].date, channel, pitch, trackHelper.voices[voiceId].volume, trackHelper.voices[voiceId].duration, note);

                }
                else if (note.Items.CountOfType(typeof(tie)) > 1)
                { //THERE IS START STOP TIE//

                    TieHelper tie = trackHelper.voices[voiceId].GetTie(note);

                    if (tie != null)
                    {
                        tie.pendingDuration += trackHelper.voices[voiceId].duration;
                    }

                }
                else if ((note.Items[note.ItemsElementName.IndexOf(ItemsChoiceType1.tie)] as tie).type == startstop.start)
                {

                    trackHelper.voices[voiceId].tieList.Add(new TieHelper(note, trackHelper.voices[voiceId].duration));
                    return;

                }
                else if ((note.Items[note.ItemsElementName.IndexOf(ItemsChoiceType1.tie)] as tie).type == startstop.stop)
                {

                    if (note.ItemsElementName.Contains(ItemsChoiceType1.pitch))
                    {
                        pitch = getMidiPitch(note);
                    }
                    else
                    {
                        pitch = 60;
                    }

                    TieHelper tie = trackHelper.voices[voiceId].GetAndRemoveTie(note);

                    if (tie != null)
                    {
                        trackHelper.voices[voiceId].duration += tie.pendingDuration;
                        AddMidiEvent(trackHelper.voices[voiceId].midiEvents, trackHelper.voices[voiceId].date, channel, pitch, trackHelper.voices[voiceId].volume, trackHelper.voices[voiceId].duration, note);
                    }
                }
            }
            else
            {
                //ADD MIDI EVENT REST//	
                trackHelper.voices[voiceId].date = trackHelper.voices[voiceId].currentDate;
                AddMidiEvent(trackHelper.voices[voiceId].midiEvents, trackHelper.voices[voiceId].date, channel, 0, 0, trackHelper.voices[voiceId].duration, note);
            }
            if (note.ItemsElementName.Contains(ItemsChoiceType1.chord)) return;
            trackHelper.voices[voiceId].lastPosition = trackHelper.voices[voiceId].currentDate;
            trackHelper.voices[voiceId].currentDate += trackHelper.voices[voiceId].duration;
            trackHelper.voices[voiceId].endMeasureDate = Math.Max(trackHelper.voices[voiceId].endMeasureDate, trackHelper.voices[voiceId].currentDate);
        }
    }
}
