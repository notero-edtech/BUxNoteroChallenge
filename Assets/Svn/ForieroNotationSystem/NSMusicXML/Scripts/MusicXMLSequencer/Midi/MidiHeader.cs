/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.MusicXML.Xsd;

namespace ForieroEngine.Music.MusicXML.Sequencer
{
    public class MidiHeader
    {
        //--Variables
        public int DeltaTiming;
        public MidiHelper.MidiFormat MidiFormat;
        public MidiHelper.MidiTimeFormat TimeFormat;
        //--Public Methods
        public MidiHeader()
        {

        }
        public void setMidiFormat(int format)
        {
            if (format == 0)
                MidiFormat = MidiHelper.MidiFormat.SingleTrack;
            else if (format == 1)
                MidiFormat = MidiHelper.MidiFormat.MultiTrack;
            else if (format == 2)
                MidiFormat = MidiHelper.MidiFormat.MultiSong;
        }
    }
}
