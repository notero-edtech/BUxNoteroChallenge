/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.MIDIUnified;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSPedal : NSObjectSMuFL
    {
        public class Options : INSObjectOptions<Options>
        {
            public PedalEnum pedalEnum = PedalEnum.Undefined;

            public void Reset()
            {
                pedalEnum = PedalEnum.Undefined;
            }

            public void CopyValuesFrom(Options o)
            {
                pedalEnum = o.pedalEnum;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public override void Reset()
        {
            base.Reset();
            options.Reset();

            DestroyChildren();
        }

        public override void Commit()
        {
            base.Commit();

            this.text.SetText(options.pedalEnum.ToSMuFL());
        }

        public void ApplyMidiMessage(int channel = 0)
        {
            switch (options.pedalEnum)
            {
                case PedalEnum.Sostenuto:
                case PedalEnum.SostenutoLetter:
                case PedalEnum.Pedal:
                case PedalEnum.PedalLetter:
                    midiData.Add(new MidiMessage() { Channel = channel, Command = CommandEnum.MIDI_CONTROL_CHANGE.ToInt(), Data1 = ControllerEnum.Sostenuto.ToInt(), Data2 = byte.MaxValue / 2 });
                    break;
                case PedalEnum.Stop:
                    midiData.Add(new MidiMessage() { Channel = channel, Command = CommandEnum.MIDI_CONTROL_CHANGE.ToInt(), Data1 = ControllerEnum.Sostenuto.ToInt(), Data2 = 0 });
                    break;
            }
        }
    }
}
