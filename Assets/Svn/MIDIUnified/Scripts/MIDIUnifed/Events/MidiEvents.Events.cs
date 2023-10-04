namespace ForieroEngine.MIDIUnified
{
    public partial class MidiEvents {
		
		public delegate void NoteEventHandler(int aMidiId, int aValue, int aChannel);
		public delegate void PedalEventHandler(PedalEnum aPedal, int aValue, int aChannel);
		public delegate void ControllerEventHandler(ControllerEnum aControllerCommand, int aValue, int aChannel);
		
		public event ShortMessageEventHandler ShortMessageEvent;
		public event NoteEventHandler NoteOnEvent;
		public event NoteEventHandler NoteOffEvent;
		public event NoteEventHandler NoteAfterTouchEvent;
		public event NoteEventHandler ProgramChangedEvent;
		public event NoteEventHandler ChannelAfterTouchEvent;
		public event NoteEventHandler PitchBendEvent;
		
        #region CONTROLLER EVENTS

        public event ControllerEventHandler ControllerEvent;

        public event PedalEventHandler PedalOnEvent;
        public event PedalEventHandler PedalOffEvent;

        public event ControllerEventHandler BankSelectEvent;
        public event ControllerEventHandler ModulationEvent;
        public event ControllerEventHandler BreathControllerEvent;
        public event ControllerEventHandler FootControllerEvent;
        public event ControllerEventHandler PortamentoTimeEvent;
        public event ControllerEventHandler DataEntryEvent;
        public event ControllerEventHandler MainVolumeEvent;
        public event ControllerEventHandler BalanceEvent;
        public event ControllerEventHandler PanEvent;
        public event ControllerEventHandler ExpressionControllerEvent;
        public event ControllerEventHandler EffectControl1Event;
        public event ControllerEventHandler EffectControl2Event;
        public event ControllerEventHandler GeneralPurposeController1Event;
        public event ControllerEventHandler GeneralPurposeController2Event;
        public event ControllerEventHandler GeneralPurposeController3Event;
        public event ControllerEventHandler GeneralPurposeController4Event;
        public event ControllerEventHandler BankSelectLSBEvent;
        public event ControllerEventHandler ModulationLSBEvent;
        public event ControllerEventHandler BreathControllerLSBEvent;
        public event ControllerEventHandler FootControllerLSBEvent;
        public event ControllerEventHandler PortamentoTimeLSBEvent;
        public event ControllerEventHandler DataEntryLSBEvent;
        public event ControllerEventHandler MainVolumeLSBEvent;
        public event ControllerEventHandler BalanceLSBEvent;
        public event ControllerEventHandler PanLSBEvent;
        public event ControllerEventHandler ExpressionControllerLSBEvent;
        public event ControllerEventHandler EffectControl1LSBEvent;
        public event ControllerEventHandler EffectControl2LSBEvent;
        public event ControllerEventHandler DamperPedalEvent;
        public event ControllerEventHandler PortamentoEvent;
        public event ControllerEventHandler SostenutoEvent;
        public event ControllerEventHandler SoftPedalEvent;
        public event ControllerEventHandler LegatoFootswitchEvent;
        public event ControllerEventHandler Hold2Event;
        public event ControllerEventHandler SoundController1Event;
        public event ControllerEventHandler SoundController2Event;
        public event ControllerEventHandler SoundController3Event;
        public event ControllerEventHandler SoundController4Event;
        public event ControllerEventHandler SoundController5Event;
        public event ControllerEventHandler SoundController6Event;
        public event ControllerEventHandler SoundController7Event;
        public event ControllerEventHandler SoundController8Event;
        public event ControllerEventHandler SoundController9Event;
        public event ControllerEventHandler SoundController10Event;
        public event ControllerEventHandler GeneralPurposeController5Event;
        public event ControllerEventHandler GeneralPurposeController6Event;
        public event ControllerEventHandler GeneralPurposeController7Event;
        public event ControllerEventHandler GeneralPurposeController8Event;
        public event ControllerEventHandler PortamentoControlEvent;
        public event ControllerEventHandler Effects1DepthEvent;
        public event ControllerEventHandler Effects2DepthEvent;
        public event ControllerEventHandler Effects3DepthEvent;
        public event ControllerEventHandler Effects4DepthEvent;
        public event ControllerEventHandler Effects5DepthEvent;
        public event ControllerEventHandler DataIncrementEvent;
        public event ControllerEventHandler DataDecrementEvent;
        public event ControllerEventHandler NonRegisteredParameteLSBEvent;
        public event ControllerEventHandler NonRegisteredParameteMSBEvent;
        public event ControllerEventHandler RegisteredParameterLSBEvent;
        public event ControllerEventHandler RegisteredParameterMSBEvent;
        public event ControllerEventHandler AllSoundOffEvent;
        public event ControllerEventHandler ResetControllersEvent;
        public event ControllerEventHandler AllNotesOffEvent;
        public event ControllerEventHandler OmniModeOffEvent;
        public event ControllerEventHandler OmniModeOnEvent;

        #endregion

        
		
	}
}
