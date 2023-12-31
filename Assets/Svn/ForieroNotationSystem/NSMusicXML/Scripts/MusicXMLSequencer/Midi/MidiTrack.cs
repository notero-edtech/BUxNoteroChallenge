/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.MusicXML.Xsd;

namespace ForieroEngine.Music.MusicXML.Sequencer
{
    public class MidiTrack
    {
        //--Variables
        public uint NotesPlayed;
        public double TotalTime;
        public byte[] Programs;
        public byte[] DrumPrograms;
        public MidiEvent[] MidiEvents;
        //--Public Properties
        public int EventCount
        {
            get { return MidiEvents.Length; }
        }
        //--Public Methods
        public MidiTrack()
        {
            NotesPlayed = 0;
            TotalTime = 0;
        }
        public bool ContainsProgram(byte program)
        {
            for (int x = 0; x < Programs.Length; x++)
            {
                if (Programs[x] == program)
                    return true;
            }
            return false;
        }
        public bool ContainsDrumProgram(byte drumprogram)
        {
            for (int x = 0; x < DrumPrograms.Length; x++)
            {
                if (DrumPrograms[x] == drumprogram)
                    return true;
            }
            return false;
        }
    }
}
