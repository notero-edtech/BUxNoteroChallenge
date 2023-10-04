using UnityEngine;

namespace ForieroEngine.MIDIUnified
{
    public partial class MidiEvents {
        public struct NoteDef
        {
            public readonly ChannelEnum Channel;
            public readonly NoteEnum Note;
            public readonly AccidentalEnum Accidental;
            public readonly OctaveEnum Octave;

            public NoteDef(ChannelEnum channel, NoteEnum note, AccidentalEnum accidental, OctaveEnum octave)
            {
                this.Channel = channel;
                this.Note = note;
                this.Accidental = accidental;
                this.Octave = octave;
            }
        }
        
        public static class Evaluator
        {
            public static bool Matches(NoteDef n, int id, int value, int channel)
            {
                bool r = !(n.Channel != ChannelEnum.None && (int) n.Channel != channel);
                if (n.Octave != OctaveEnum.None && (int) n.Octave != id.Octave()) r = false; 
                if (n.MidiIndex().BaseMidiIndex() != id.BaseMidiIndex()) return false;
                return r;
            }
        }	    
    }
}
