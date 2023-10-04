/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;


namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("MIDI Unified - Generators")]
    [Tooltip("Adds Midi Input Generator. A midi input might be likely an external midi keyboard device.")]
    public class AddGeneratorMidiInput : FsmStateAction
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
        [Tooltip("If there is no Midi Input in the scene, there will be likely messages in the receiving buffer.")]
        public bool cleanBuffer;

        public override void Reset()
        {
            midiOut = false;
            channel = new FsmInt { UseVariable = true };
            channelEnum = ChannelEnum.None;
            volume = new FsmFloat { Value = 1 };
            cleanBuffer = true;
        }

        public override void OnEnter()
        {
            this.Owner.AddComponent<MidiInput>();
            MIDISettings.instance.inputSettings.midiOut = midiOut;
            MIDISettings.instance.inputSettings.customVolume = volume.IsNone ? 1f : volume.Value;
            MIDISettings.instance.inputSettings.midiChannel = channel.IsNone ? (channelEnum == ChannelEnum.None ? ChannelEnum.C0 : channelEnum) : (ChannelEnum)channel.Value;
            MIDISettings.instance.inputSettings.cleanBuffer = true;
            Finish();
        }
    }
}