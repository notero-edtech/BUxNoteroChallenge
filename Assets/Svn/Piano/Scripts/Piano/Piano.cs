using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Classes;
using ForieroEngine.MIDIUnified.Interfaces;

public class Piano : MidiInstrument<Piano>, IMidiInstrument, IMidiSender, IMidiReceiver
{
	public static IMidiController SelectedController { get => MidiControllers.Selected; set => MidiControllers.Selected = value; }
	public static List<IMidiController> Controllers { get; } = new();
	//	public MusicXMLSequencer[] musicXMLsequencers;
	public MidiSeqKaraokeScript[] karaokeSequencers;

	public bool autoUpdate = true;
	TheorySystemEnum theorySystem = TheorySystemEnum.Undefined;
	KeySignatureEnum keySignature = KeySignatureEnum.CMaj_AMin;

	public bool hookMidiOut = true;
	public bool hookSequencer = false;
	public bool hookKaraokeSequencer = false;
	public bool hookKeyboard = false;
	public bool hookInput = false;
	public bool hookPlaymaker = false;
	public bool hookSynthSequencer = false;
	public bool hookMidiDispatcher = false;
	public bool hookNSBehaviour = false;

	public MidiOutMonitor midiOutMonitor;
	public MidiSeqKaraokeScript karaokeSequencer;		
	public NSBehaviour nsBehaviour;

	public bool midiOut = true;
	public ChannelEnum midiChannel = ChannelEnum.None;
	public ProgramEnum instrument = ProgramEnum.AcousticGrandPiano;
	public bool useCustomVolume = false;
	public float customVolume = 1f;
	
	public PianoPedal rightPedal;
	public PianoPedal leftPedal;
	public PianoPedal centerPedal;

	public enum GeneratorsEnum
	{
		MidiSequencerInput,
		MidiInput,
		MidiKeyboardInput,
		MidiPlayMakerInput,
		MidiDispatcher
	}

	private readonly MidiEvents _midiEvents = new MidiEvents();

	protected override void Awake()
	{
		base.Awake();
		
		if (hookKaraokeSequencer) { for (var i = 0; i < karaokeSequencers.Length; i++) { _midiEvents.AddSender(karaokeSequencers[i]); } }
		if (hookInput) StartCoroutine(HookMidiInputEvents());
		if (hookKeyboard) StartCoroutine(HookKeyboardEvents());
		if (hookPlaymaker) StartCoroutine(HookPlaymakerEvents());
		if (hookMidiDispatcher) StartCoroutine(HookDispatcherEvents());

		if (hookMidiOut) _midiEvents.AddSender(midiOutMonitor);
		if (hookKaraokeSequencer) _midiEvents.AddSender(karaokeSequencer);
				
		if (hookNSBehaviour) _midiEvents.AddSender(nsBehaviour);

		_midiEvents.NoteOnEvent += NoteOn;
		_midiEvents.NoteOffEvent += NoteOff;
		_midiEvents.PedalOnEvent += PedalOn;
		_midiEvents.PedalOffEvent += PedalOff;
	}

	protected override void OnDestroy()
    {
		_midiEvents.RemoveAllSenders();
		base.OnDestroy();
    }

    private IEnumerator HookDispatcherEvents()
	{
		yield return new WaitUntil(() => MidiDispatcher.singleton != null);
		_midiEvents.AddSender(MidiDispatcher.singleton);
	}

    private IEnumerator HookMidiInputEvents()
    {
		yield return new WaitUntil(()=>MidiInput.singleton != null);
		_midiEvents.AddSender(MidiInput.singleton);
	}

    private IEnumerator HookPlaymakerEvents()
	{
		yield return new WaitUntil(() => MidiPlayMakerInput.singleton != null);
		_midiEvents.AddSender(MidiPlayMakerInput.singleton);
	}

    private IEnumerator HookKeyboardEvents()
	{
		yield return new WaitUntil(() => MidiKeyboardInput.singleton != null);
		_midiEvents.AddSender(MidiKeyboardInput.singleton);
	}

	private void Update()
	{
		if (!autoUpdate) return;
		var changed = theorySystem != MIDITheorySettings.instance.theorySystem || keySignature != MIDITheorySettings.instance.keySignature;
		if (!changed) return;
		theorySystem = MIDITheorySettings.instance.theorySystem;
		keySignature = MIDITheorySettings.instance.keySignature;
		SetTheorySystem(theorySystem, keySignature);
	}

	public bool isIn = true;
	public override void Show(bool animated = true) { isIn = true; SelectedController?.Show(animated); }
	public override void Hide(bool animated = true) { isIn = false; SelectedController?.Hide(animated); }

	[HideInInspector]
	public bool[] userInputNote = new bool[128];

	private void NoteOn(int aMidiIdx, int aValue, int aChannel)
	{
		if (aChannel == (int)midiChannel) userInputNote[aMidiIdx] = true;
		KeyDown(aMidiIdx, aValue);
	}

	private void NoteOff(int aMidiIdx, int aValue, int aChannel)
	{
		if (aChannel == (int)midiChannel) userInputNote[aMidiIdx] = false;
        KeyUp(aMidiIdx);
	}

	private void PedalOn(PedalEnum aPedal, int aValue, int aChannel) => PedalDown(aPedal);
	private void PedalOff(PedalEnum aPedal, int aValue, int aChannel) => PedalUp(aPedal);
	
	public void DisableInput(GeneratorsEnum anInput)
	{
		switch (anInput)
		{
			case GeneratorsEnum.MidiInput:
				_midiEvents.RemoveSender(MidiInput.singleton);
				break;
			case GeneratorsEnum.MidiKeyboardInput:
				_midiEvents.RemoveSender(MidiKeyboardInput.singleton);
				break;
			case GeneratorsEnum.MidiSequencerInput:
				for (var i = 0; i < karaokeSequencers.Length; i++)
				{
					_midiEvents.RemoveSender(karaokeSequencers[i]);
				}
				break;
			case GeneratorsEnum.MidiPlayMakerInput:
				_midiEvents.RemoveSender(MidiPlayMakerInput.singleton);
				break;
			case GeneratorsEnum.MidiDispatcher:
				_midiEvents.RemoveSender(MidiDispatcher.singleton);
				break;
		}
	}

	public void EnableInput(GeneratorsEnum anInput)
	{
		switch (anInput)
		{
			case GeneratorsEnum.MidiInput: _midiEvents.AddSender(MidiInput.singleton); break;
			case GeneratorsEnum.MidiKeyboardInput: _midiEvents.AddSender(MidiKeyboardInput.singleton); break;
			case GeneratorsEnum.MidiSequencerInput:
				for (var i = 0; i < karaokeSequencers.Length; i++) { _midiEvents.AddSender(karaokeSequencers[i]); }
				break;
			case GeneratorsEnum.MidiPlayMakerInput: _midiEvents.AddSender(MidiPlayMakerInput.singleton); break;
			case GeneratorsEnum.MidiDispatcher: _midiEvents.AddSender(MidiDispatcher.singleton); break;
		}
	}

	public bool rightPedalDown = false;
	public bool centerPedalDown = false;
	public bool leftPedalDown = false;

	public bool useColors = true;
	
	public void PedalDown(PedalEnum aPianoPedal)
	{
		switch (aPianoPedal)
		{
			case PedalEnum.DamperPedal: rightPedalDown = true; //if(rightPedal) rightPedal.PedalDown();
				break;
			case PedalEnum.SoftPedal: leftPedalDown = true; //if(leftPedal) leftPedal.PedalDown();
				break;
			case PedalEnum.Sostenuto: centerPedalDown = true; //if(centerPedal) centerPedal.PedalDown();	
				break;
		}
	}

	public void PedalUp(PedalEnum aPianoPedal)
	{
		switch (aPianoPedal)
		{
			case PedalEnum.DamperPedal:
				rightPedalDown = false;
				if (rightPedal)
				{
					//rightPedal.PedalUp();
					rightPedal.isDown = false;
				}
				break;
			case PedalEnum.SoftPedal:
				leftPedalDown = false;
				if (leftPedal)
				{
					//leftPedal.PedalUp();
					leftPedal.isDown = false;
				}
				break;
			case PedalEnum.Sostenuto:
				centerPedalDown = false;
				if (centerPedal)
				{
					//centerPedal.PedalUp();
					centerPedal.isDown = false;
				}
				break;
		}
	}

	public void KeyDownEvent(int aMidiIdx, float aVolume)
	{
        OnShortMessageEvent((midiChannel == ChannelEnum.None ? 0 : (int)midiChannel) + (int)CommandEnum.MIDI_NOTE_ON, aMidiIdx, MidiConversion.GetByteVolume(aVolume * (useCustomVolume ? customVolume : 1f)), -1);
        if (midiOut){ MidiOut.NoteOn(aMidiIdx, MidiConversion.GetByteVolume(aVolume * (useCustomVolume ? customVolume : 1f)), (int)midiChannel); }
	}

	public void KeyUpEvent(int aMidiIdx)
	{
        OnShortMessageEvent((midiChannel == ChannelEnum.None ? 0 : (int)midiChannel) + (int)CommandEnum.MIDI_NOTE_OFF, aMidiIdx, 0, -1);
        if (midiOut){ MidiOut.NoteOff(aMidiIdx, (int)midiChannel); }
	}

	public void KeyDown(int aMidiIdx, int aVolume) => SelectedController?.Keys.KeyDown(aMidiIdx, aVolume);
	public void KeyUp(int aMidiIdx) => SelectedController?.Keys.KeyUp(aMidiIdx);
	public bool KeyExists(int aMidiId) => SelectedController.Keys.KeyExists(aMidiId);
	public Vector3 GetKeyPosition(int aMidiId) => SelectedController.Keys.GetKeyPosition(aMidiId);
	public Vector3 GetKeyLocalPosition(int aMidiIdx) => SelectedController.Keys.GetKeyLocalPosition(aMidiIdx);
	public void ColorKey(int aMidiId, Color aColor)=> SelectedController?.Keys.ColorKey(aMidiId, aColor);
	public Color GetKeyDownColor(int aMidiId) => SelectedController.Keys.GetKeyDownColor(aMidiId);
	public Color GetKeyUpColor(int aMidiId) => SelectedController.Keys.GetKeyUpColor(aMidiId);
	public Color GetKeyDownColorDefault(int aMidiId) => SelectedController.Keys.GetKeyDownColorDefault(aMidiId); 
	public Color GetKeyUpColorDefault(int aMidiId) => SelectedController.Keys.GetKeyUpColorDefault(aMidiId);
	public void SetKeyDownColor(int aMidiId, Color aColor) { SelectedController?.Keys.SetKeyDownColor(aMidiId, aColor); }
	public void SetKeyUpColor(int aMidiId, Color aColor) { SelectedController?.Keys.SetKeyUpColor(aMidiId, aColor); }
	public void ColorKeyDown(int aMidiId) => SelectedController?.Keys.ColorKeyDown(aMidiId);
	public void ColorKeyUp(int aMidiId) => SelectedController?.Keys.ColorKeyUp(aMidiId);
	public void OctaveSetWhiteKeysUpColor(int anOctave, Color aColor) => SelectedController?.Keys.OctaveSetWhiteKeysUpColor(anOctave, aColor);
	public void OctaveSetWhiteKeysUpColorDefault(int anOctave) => SelectedController?.Keys.OctaveSetWhiteKeysUpColorDefault(anOctave);
	public bool IsKeyDown(int aMidiIdx) => SelectedController.Keys.IsKeyDown(aMidiIdx);
	public KeyType GetKeyType(int aMidiIdx) => SelectedController.Keys.GetKeyType(aMidiIdx);
	public void SetTheorySystem(TheorySystemEnum theorySystem, KeySignatureEnum keySignatureEnum) => SelectedController.Keys.SetTheorySystem(theorySystem, keySignatureEnum);
	public void OnMidiMessageReceived(int aCommand, int aData1, int aData2, int deviceId)
	{
		if (aCommand.IsToneON()) SelectedController.Keys.ColorKeyDown(aData1);
		else if (aCommand.IsToneOFF()) SelectedController.Keys.ColorKeyUp(aData1);
	}
}
