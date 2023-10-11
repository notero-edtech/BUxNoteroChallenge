using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Notero.Unity.MidiNoteInfo
{
    public static class MidiNoteInfoHelper
    {
        private const int m_TrackNameEventTime = 0;
        private const int m_TrackNameEventType = 3;

        public static float BPMMultiplier(MidiFile midiFile) => midiFile.BottomNumberTimeSignature / 4;

        /// <summary>
        /// Try get track index of midi track, 
        /// track 0 is right hand notes, track 1 is left hand notes
        /// </summary>
        /// <param name="track"></param>
        /// <param name="trackIndex">default = 0</param>
        /// <returns></returns>
        public static bool TryGetTrackIndex(MidiTrack track, out int trackIndex)
        {
            TextEvent textEvent = track.TextEvents.FirstOrDefault(e => e.Time == m_TrackNameEventTime && e.Type == m_TrackNameEventType);
            int.TryParse(textEvent.Value, out trackIndex);

            return textEvent.Value != null;
        }

        /// <summary>
        /// Generate list of MidiNoteInfo that pair MidiEvent NoteOn, NoteOff of each midi note
        /// </summary>
        /// <param name="midiFile"></param>
        /// <returns></returns>
        public static List<MidiNoteInfo> GenerateMidiNoteInfoListByFile(MidiFile midiFile, float bpm = default)
        {
            List<MidiNoteInfo> noteInfoList = new List<MidiNoteInfo>();
            Dictionary<int, MidiEvent> noteOnEvents = new Dictionary<int, MidiEvent>();
            int ticksPerQuarterNote = midiFile.TicksPerQuarterNote;

            List<byte> currentKeySignatureEvent = new List<byte>();

            if(bpm == default) bpm = midiFile.BPM;

            foreach(MidiTrack track in midiFile.Tracks)
            {
                if(!TryGetTrackIndex(track, out int trackIndex))
                {
                    trackIndex = 0;
                }

                foreach(MidiEvent midiEvent in track.MidiEvents)
                {
                    bool hasNoteOn = noteOnEvents.TryGetValue(midiEvent.Note, out MidiEvent noteOn);

                    if(midiEvent.MetaEventType == MetaEventType.KeySignature)
                    {
                        if(currentKeySignatureEvent.Count == 2)
                        {
                            currentKeySignatureEvent[0] = midiEvent.Arg2;
                            currentKeySignatureEvent[1] = midiEvent.Arg3;
                        }
                        else
                        {
                            currentKeySignatureEvent.Add(midiEvent.Arg2);
                            currentKeySignatureEvent.Add(midiEvent.Arg3);
                        }
                    }

                    if(midiEvent.MidiEventType == MidiEventType.NoteOn)
                    {
                        if(!hasNoteOn)
                        {
                            noteOnEvents[midiEvent.Note] = midiEvent;
                        }
                        else
                        {
                            Debug.LogError($"[{nameof(MidiNoteInfoHelper)}.{nameof(GenerateMidiNoteInfoListByFile)}] " +
                                           $"Midi contain duplicate note on event on note: {midiEvent.Note}");
                        }
                    }
                    else if(midiEvent.MidiEventType == MidiEventType.NoteOff)
                    {
                        if(hasNoteOn)
                        {
                            double noteOnTime = ConvertMidiTimeToMilliseconds(noteOn.Time, bpm, ticksPerQuarterNote);
                            double noteOffTime = ConvertMidiTimeToMilliseconds(midiEvent.Time, bpm, ticksPerQuarterNote);
                            var accidentalNumber = currentKeySignatureEvent.Count == 0 ? 0 : (sbyte)currentKeySignatureEvent[0];
                            var scale = currentKeySignatureEvent.Count == 0 ? 0 : (sbyte)currentKeySignatureEvent[1];

                            MidiNoteInfo newInfo = new MidiNoteInfo(
                                trackIndex: trackIndex,
                                midiId: midiEvent.Note,
                                noteOnTime: noteOnTime,
                                noteOffTime: noteOffTime,
                                accidentalNumber: accidentalNumber,
                                scale: scale,
                                bpm: bpm);

                            noteInfoList.Add(newInfo);
                            noteOnEvents.Remove(midiEvent.Note);
                        }
                        else
                        {
                            Debug.LogError($"[{nameof(MidiNoteInfoHelper)}.{nameof(GenerateMidiNoteInfoListByFile)}] " +
                                           $"Midi contain note off without note on event on note: {midiEvent.Note}");
                        }
                    }
                }
            }

            return noteInfoList.OrderBy(info => info.NoteOnTime).ToList();
        }

        /// <summary>
        /// Convert midi time to milliseconds
        /// </summary>
        /// <param name="midiTime"></param>
        /// <param name="bpm"></param>
        /// <param name="ticksPerQuarterNote"></param>
        /// <returns>time in milliseconds</returns>
        public static double ConvertMidiTimeToMilliseconds(int midiTime, double bpm, double ticksPerQuarterNote)
        {
            return ConvertMidiTimeToSeconds(midiTime, bpm, ticksPerQuarterNote) * 1000;
        }

        /// <summary>
        /// Convert midi time to seconds
        /// </summary>
        /// <param name="midiTime"></param>
        /// <param name="bpm"></param>
        /// <param name="ticksPerQuarterNote"></param>
        /// <returns>time in seconds</returns>
        public static double ConvertMidiTimeToSeconds(int midiTime, double bpm, double ticksPerQuarterNote)
        {
            double quarterNoteSecondsDuration = 60 / bpm;
            return midiTime / ticksPerQuarterNote * quarterNoteSecondsDuration;
        }
    }
}