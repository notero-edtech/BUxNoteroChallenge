/* Copyright © Marek Ledvina, Foriero s.r.o. */

using ForieroEngine.MIDIUnified;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("MIDI Unified - Conditions")]
    [Tooltip("Play note.")]
    public class MidiGeneratorNoteCondition : FsmStateAction
    {
        [RequiredField] [ObjectType(typeof(FsmMidiGeneratorInstance))]
        public FsmObject instance;

        [ActionSection("Condition")] [Tooltip("Specify NoteON or NoteOFF hook.")]
        public Command command;
        
        [Tooltip("You can specify a note by name. ID has to be None.")]
        public NoteEnum note;

        [Tooltip("You can specify a note accidental.")]
        public AccidentalEnum accidental;

        [Tooltip("If you use 'noteEnum' insteead of 'ID' an octaveEnum has to be specified.")]
        public OctaveEnum octave;

        [Tooltip(
            "Channel to be played on(0-15). If 'None' the default channel is used. If you use channelEnum instead of channel, channel has to be set to 'None'")]
        public ChannelEnum channel;

        [ActionSection("Events")] public FsmEvent trueEvent;
        public FsmEvent falseEvent;

        public enum Command
        {
            NoteON,
            NoteOFF,
            NoteONOFF
        }

        private IMidiSender _sender;
        private MidiEvents _midiEvents;
        private MidiEvents.NoteDef _noteDef;

        public override void Reset()
        {
            note = NoteEnum.None;
            accidental = AccidentalEnum.None;
            channel = ChannelEnum.None;
            octave = OctaveEnum.None;
            command = Command.NoteONOFF;
            trueEvent = null;
            falseEvent = null;

            _noteEventHandler = null;
        }

        private MidiEvents.NoteEventHandler _noteEventHandler;
        
        public override void OnEnter()
        {
            _midiEvents = new MidiEvents();
            _sender = (instance.Value as FsmMidiGeneratorInstance)?.instance;
            if (MIDISettings.IsDebug && _sender == null) Debug.LogError("Generator is NULL!!!");
            _midiEvents.AddSender(_sender);
            _noteEventHandler = NoteEvent;
            switch (command)
            {
                case Command.NoteON: _midiEvents.NoteOnEvent += _noteEventHandler; break;
                case Command.NoteOFF: _midiEvents.NoteOffEvent += _noteEventHandler; break;
                case Command.NoteONOFF: _midiEvents.NoteOnEvent += _noteEventHandler; _midiEvents.NoteOffEvent += _noteEventHandler; break;
            }
        }

        public override void OnExit()
        {
            _midiEvents.NoteOnEvent -= _noteEventHandler;
            _midiEvents.NoteOffEvent -= _noteEventHandler;
            _noteEventHandler = null;
            _midiEvents.RemoveSender(_sender);
            _midiEvents = null;
            _sender = null;
        }

        void NoteEvent(int id, int value, int ch)
        {
            _noteDef = new MidiEvents.NoteDef(channel, note, accidental, octave);
            if (MidiEvents.Evaluator.Matches(_noteDef, id, value, ch))
                if (trueEvent != null) Fsm.Event(trueEvent);
                else if (falseEvent != null) Fsm.Event(falseEvent);
        }
    }
}