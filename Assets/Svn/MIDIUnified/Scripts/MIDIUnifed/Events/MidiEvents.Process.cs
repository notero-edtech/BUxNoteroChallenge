using UnityEngine;

namespace ForieroEngine.MIDIUnified
{

    public partial class MidiEvents {
			
		public void AddShortMessage(int aCommand, int aData1, int aData2, int aDeviceId = -1)
	    {
			ShortMessageEvent?.Invoke(aCommand, aData1, aData2, aDeviceId);
			ProcessMidiMessage(aCommand, aData1, aData2);
		}
		
		public void AddShortMessage(int aChannel, int aCommand, int aData1, int aData2, int aDeviceId = -1)
		{
			AddShortMessage(aChannel + aCommand, aData1, aData2, aDeviceId);
		}
		
		public void AddNoteOn(int midiIndex, int volume, int channel, int aDeviceId = -1){
			AddShortMessage(channel + (int)CommandEnum.MIDI_NOTE_ON, midiIndex, volume, aDeviceId);
		}
		
		public void AddNoteOff(int midiIndex, int channel, int aDeviceId = -1){
			AddShortMessage(channel + (int)CommandEnum.MIDI_NOTE_OFF, midiIndex, 0, aDeviceId);
		}
			
		private void ProcessMidiMessage(int aCommand, int aData1, int aData2){
		    var channel = aCommand.ToMidiChannel();
	        var command = (CommandEnum)aCommand.ToMidiCommand();
	        if(log && command != CommandEnum.MIDI_CONTROL_CHANGE) OnLog?.Invoke($"CH: {channel} C: {command} D1: {aData1} D2: {aData2}");
	        switch (command)
	        {
	            case CommandEnum.MIDI_NOTE_OFF: NoteOffEvent?.Invoke(aData1, aData2, channel); break;
	            case CommandEnum.MIDI_NOTE_ON:
		            if (aData2 == 0) NoteOffEvent?.Invoke(aData1, aData2, channel);
		            else NoteOnEvent?.Invoke(aData1, aData2, channel);
		            break;
	            case CommandEnum.MIDI_POLY_AFTERTOUCH: 
	                break;
	            case CommandEnum.MIDI_CONTROL_CHANGE: 
	            {
					var ce = aData1.ToControllerEnum();
	                ControllerEvent?.Invoke(ce, aData2, channel);
	                if(log) OnLog?.Invoke($"{ce} {aData2} {channel}");
                    switch (ce)
                    {
                        case ControllerEnum.None: break;
                        case ControllerEnum.BankSelect: BankSelectEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.Modulation: ModulationEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.BreathController: BreathControllerEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.FootController: FootControllerEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.PortamentoTime: PortamentoTimeEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.DataEntry: DataEntryEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.MainVolume: MainVolumeEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.Balance: BalanceEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.Pan: PanEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.ExpressionController: ExpressionControllerEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.EffectControl1: EffectControl1Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.EffectControl2: EffectControl2Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.GeneralPurposeController1: GeneralPurposeController1Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.GeneralPurposeController2: GeneralPurposeController2Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.GeneralPurposeController3: GeneralPurposeController3Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.GeneralPurposeController4: GeneralPurposeController4Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.BankSelectLSB: BankSelectLSBEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.ModulationLSB: ModulationLSBEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.BreathControllerLSB: BreathControllerLSBEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.FootControllerLSB: FootControllerLSBEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.PortamentoTimeLSB: PortamentoTimeLSBEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.DataEntryLSB: DataEntryLSBEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.MainVolumeLSB: MainVolumeLSBEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.BalanceLSB: BalanceLSBEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.PanLSB: PanLSBEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.ExpressionControllerLSB: ExpressionControllerLSBEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.EffectControl1LSB: EffectControl1LSBEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.EffectControl2LSB: EffectControl2LSBEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.DamperPedal:
                            DamperPedalEvent?.Invoke(ce, aData2, channel);
                            if (aData2 > 0) PedalOnEvent?.Invoke(PedalEnum.DamperPedal, aData2, channel); else PedalOffEvent?.Invoke(PedalEnum.DamperPedal, aData2, channel);                                    
                            break;
                        case ControllerEnum.Portamento:
                            PortamentoEvent?.Invoke(ce, aData2, channel);
                            //if (aData2 > 0) PedalOnEvent(PedalEnum.Center, aData2, channel); else PedalOffEvent(PedalEnum.Center, aData2, channel);
                            break;
                        case ControllerEnum.Sostenuto:
                            SostenutoEvent?.Invoke(ce, aData2, channel);
                            if (aData2 > 0) PedalOnEvent?.Invoke(PedalEnum.Sostenuto, aData2, channel); else PedalOffEvent?.Invoke(PedalEnum.Sostenuto, aData2, channel);
                            break;
                        case ControllerEnum.SoftPedal:
                            SoftPedalEvent?.Invoke(ce, aData2, channel);
                            if (aData2 > 0) PedalOnEvent?.Invoke(PedalEnum.SoftPedal, aData2, channel); else PedalOffEvent?.Invoke(PedalEnum.SoftPedal, aData2, channel);
                            break;
                        case ControllerEnum.LegatoFootswitch: LegatoFootswitchEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.Hold2: Hold2Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.SoundController1: SoundController1Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.SoundController2: SoundController2Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.SoundController3: SoundController3Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.SoundController4: SoundController4Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.SoundController5: SoundController5Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.SoundController6: SoundController6Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.SoundController7: SoundController7Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.SoundController8: SoundController8Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.SoundController9: SoundController9Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.SoundController10: SoundController10Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.GeneralPurposeController5: GeneralPurposeController5Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.GeneralPurposeController6: GeneralPurposeController6Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.GeneralPurposeController7: GeneralPurposeController7Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.GeneralPurposeController8: GeneralPurposeController8Event?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.PortamentoControl: PortamentoControlEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.Effects1Depth: Effects1DepthEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.Effects2Depth: Effects2DepthEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.Effects3Depth: Effects3DepthEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.Effects4Depth: Effects4DepthEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.Effects5Depth: Effects5DepthEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.DataIncrement: DataIncrementEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.DataDecrement: DataDecrementEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.NonRegisteredParameteLSB: NonRegisteredParameteLSBEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.NonRegisteredParameteMSB: NonRegisteredParameteMSBEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.RegisteredParameterLSB: RegisteredParameterLSBEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.RegisteredParameterMSB: RegisteredParameterMSBEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.AllSoundOff: AllSoundOffEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.ResetControllers: ResetControllersEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.AllNotesOff: AllNotesOffEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.OmniModeOff: OmniModeOffEvent?.Invoke(ce, aData2, channel); break;
                        case ControllerEnum.OmniModeOn: OmniModeOnEvent?.Invoke(ce, aData2, channel); break;
                    }
                }
                break;
	            case CommandEnum.MIDI_PROGRAM_CHANGE:
	               		ProgramChangedEvent?.Invoke(aData1, aData2, channel);
	                break;
	            case CommandEnum.MIDI_AFTERTOUCH: 
						ChannelAfterTouchEvent?.Invoke(aData1, aData2, channel);
	                break;
	            case CommandEnum.MIDI_PITCH_BEND: //Pitch Bend
						PitchBendEvent?.Invoke(aData1, aData2, channel);
	                break;
				case CommandEnum.MIDI_SYSTEM_RESET:
					Debug.Log("METAMESSAGE");
					switch(aData1){
					case 0x51:
						Debug.Log("TEMPO CHANGE :" + aData2.ToString()); 
					break;
					}
					break;
	            default:
	                return;
            }
        }
    }
}
