/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.MusicXML.Xsd;

namespace ForieroEngine.Music.MusicXML.Sequencer
{
    public class MidiEvent
    {
        //--Variables
        public double deltaTime;
        public decimal totalTime;
        public decimal durationTime;
        public MidiHelper.MidiMetaEvent midiMetaEvent;
        public MidiHelper.MidiChannelEvent midiChannelEvent;
        public object[] Parameters;
        public byte parameter1;
        public byte parameter2;
        public byte channel;
        public note note;
        //--Public Methods
        public MidiEvent()
        {
            this.Parameters = new object[5];
            this.midiMetaEvent = MidiHelper.MidiMetaEvent.None;
            this.midiChannelEvent = MidiHelper.MidiChannelEvent.None;
        }
        public bool isMetaEvent()
        {
            return midiChannelEvent == MidiHelper.MidiChannelEvent.None;
        }
        public bool isChannelEvent()
        {
            return midiMetaEvent == MidiHelper.MidiMetaEvent.None;
        }
        public MidiHelper.ControllerType GetControllerType()
        {
            if (midiChannelEvent != MidiHelper.MidiChannelEvent.Controller)
                return MidiHelper.ControllerType.None;
            switch (parameter1)
            {
                case 1:
                    return MidiHelper.ControllerType.Modulation; //msb coarse
                case 6:
                    return MidiHelper.ControllerType.DataEntry; //coarse
                case 38:
                    return MidiHelper.ControllerType.DataEntryLSB; //fine
                case 7:
                    return MidiHelper.ControllerType.MainVolume; //msb
                case 10:
                    return MidiHelper.ControllerType.Pan;
                case 100:
                    return MidiHelper.ControllerType.RegisteredParameterLSB; //fine
                case 101:
                    return MidiHelper.ControllerType.RegisteredParameterMSB; //coarse
                case 121:
                    return MidiHelper.ControllerType.ResetControllers;
                case 123:
                    return MidiHelper.ControllerType.AllNotesOff;
                default:
                    return MidiHelper.ControllerType.Unknown;
            }
        }
    }
}
