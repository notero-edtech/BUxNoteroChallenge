/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using ForieroEngine.Music.MusicXML.Xsd;

namespace ForieroEngine.Music.MusicXML.Sequencer
{
    public partial class MidiFile
    {
        int TPQ = 960;

        int convert2tick(decimal aValue, decimal division)
        {
            return (int)(TPQ * aValue / division);
        }

        byte convert2volume(double aValue)
        {
            return (byte)(aValue * 90 / 100);
        }

        step i2step(int i)
        {
            step result = step.A;
            switch (i)
            {
                case 5: result = step.A; break;
                case 6: result = step.B; break;
                case 0: result = step.C; break;
                case 1: result = step.D; break;
                case 2: result = step.E; break;
                case 3: result = step.F; break;
                case 4: result = step.G; break;
            }
            return result;
        }

        int step2i(step step)
        {
            switch (step)
            {
                case step.A: return 5;
                case step.B: return 6;
                case step.C: return 0;
                case step.D: return 1;
                case step.E: return 2;
                case step.F: return 3;
                case step.G: return 4;
            }
            return -1;
        }

        decimal getMidiPitch(note aNote)
        {
            pitch pitch = (aNote.Items[aNote.ItemsElementName.IndexOf(ItemsChoiceType1.pitch)] as pitch);
            step note = pitch.step;
            decimal octave = pitch.octave.Exists() ? decimal.Parse(pitch.octave) + 1 : 0;
            decimal alter = pitch.alter;

            int step = step2i(note);
            if (step >= 0)
            {
                short[] step2pitch = new short[7] { 0, 2, 4, 5, 7, 9, 11 };
                decimal pitchValue = (octave * 12) + step2pitch[step];
                //Debug.Log(note + octave + " : " + (pitch + alter).ToString());
                return (pitchValue + alter);
            }
            return -1;
        }

        void AddMidiEvent(List<MidiEvent> midiEventList, int date, int channel, decimal pitch, decimal velocity, decimal duration, note aNote)
        {
            //Debug.Log(date + " " + channel + " " + pitch.ToString() + " " + velocity + " " + duration);
            MidiEvent noteOn = new MidiEvent();
            noteOn.note = aNote;
            noteOn.totalTime = date + 1;
            noteOn.durationTime = duration;
            noteOn.Parameters = new object[4] { (object)(0x09 << 4 | channel), (object)pitch, (object)velocity, 0 };
            noteOn.channel = (byte)(channel);
            noteOn.parameter1 = (byte)pitch;
            noteOn.parameter2 = 127;//(byte)velocity;
            noteOn.midiChannelEvent = MidiHelper.MidiChannelEvent.Note_On;
            midiEventList.Add(noteOn);


            MidiEvent noteOff = new MidiEvent();

            noteOff.totalTime = date + duration - 1;
            noteOff.note = aNote;
            noteOff.durationTime = 0;
            noteOff.Parameters = new object[4] { (object)(0x08 << 4 | channel), (object)pitch, 0, 0 };
            noteOff.channel = (byte)(0x08 << 4 | channel);
            noteOff.parameter1 = (byte)pitch;
            noteOff.parameter2 = 0;
            noteOff.midiChannelEvent = MidiHelper.MidiChannelEvent.Note_Off;
            midiEventList.Add(noteOff);
        }


        class TrackHelper
        {
            public List<VoiceHelper> voices;
            public TrackHelper()
            {
                voices = new List<VoiceHelper>(10);
                for (int i = 0; i < 10; i++)
                {
                    VoiceHelper vh = new VoiceHelper();
                    vh.index = i;
                    voices.Add(vh);
                }
            }
        }

        class VoiceHelper
        {
            public int index = -1;
            public List<MidiEvent> midiEvents = new List<MidiEvent>();
            public int lastPosition = 0;
            public int currentDate = 0;
            public List<TieHelper> tieList = new List<TieHelper>();
            public int endMeasureDate = 0;
            public decimal volume = 80;
            public decimal totalTime = 0;
            public int duration = 0;
            public int date = 0;
            public TieHelper GetTie(note aNote)
            {
                return tieList.Find(delegate (TieHelper t1)
                {
                    pitch pitch1 = (t1.note.Items[t1.note.ItemsElementName.IndexOf(ItemsChoiceType1.pitch)] as pitch);

                    step step1 = pitch1.step;
                    decimal octave1 = pitch1.octave.Exists() ? decimal.Parse(pitch1.octave) + 1 : 0;
                    decimal alter1 = pitch1.alter;

                    pitch pitch2 = (aNote.Items[aNote.ItemsElementName.IndexOf(ItemsChoiceType1.pitch)] as pitch);
                    step step2 = pitch2.step;
                    decimal octave2 = pitch1.octave.Exists() ? decimal.Parse(pitch1.octave) + 1 : 0;
                    decimal alter2 = pitch2.alter;

                    return (step1 == step2 && octave1 == octave2 && alter1 == alter2);
                });
            }
            public TieHelper GetAndRemoveTie(note aNote)
            {
                TieHelper result = GetTie(aNote);
                if (result != null) tieList.Remove(result);
                return result;
            }
        }

        class TieHelper
        {
            public note note;
            public int pendingDuration;
            public TieHelper(note aNote, int aPendingDuration)
            {
                note = aNote;
                pendingDuration = aPendingDuration;
            }
        }
    }
}





