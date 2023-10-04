/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;


namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("MIDI Unified - Generators")]
    [Tooltip("Adds Midi PlayMaker Input Generator. A midi input coming from PlayMaker actions and other components outside PlayMaker scope hook to MidiPlayMakerInput generator.")]
    public class AddGeneratorMidiPlayMakerInput : FsmStateAction
    {
        [Tooltip("Send all midi input midimessages to midi output device(port).")]
        public bool midiOut;
        [Tooltip("Sets midi out messages volume.")]
        [HasFloatSlider(0, 1)]
        public FsmFloat volume;
        [Tooltip("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
        public FsmInt channel;
        [Tooltip("Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
        public ChannelEnum channelEnum;

        public override void Reset()
        {
            midiOut = false;
            channel = new FsmInt { UseVariable = true };
            channelEnum = ChannelEnum.None;
            volume = new FsmFloat { Value = 1 };
        }

        public override void OnEnter()
        {
            this.Owner.AddComponent<MidiPlayMakerInput>();
            MIDISettings.instance.playmakerSettings.midiOut = midiOut;
            MIDISettings.instance.playmakerSettings.customVolume = volume.IsNone ? 1f : volume.Value;
            MIDISettings.instance.playmakerSettings.midiChannel = channel.IsNone ? (channelEnum == ChannelEnum.None ? ChannelEnum.C0 : channelEnum) : (ChannelEnum)channel.Value;
            Finish();
        }
    }
}