/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;

namespace ForieroEngine.Music.MusicXML.Sequencer
{
    public class MidiSequencerEvent
    {
        //--Variables
        public List<MidiEvent> Events; //List of Events
        public MidiSequencerEvent()
        {
            Events = new List<MidiEvent>();
        }
    }
}
