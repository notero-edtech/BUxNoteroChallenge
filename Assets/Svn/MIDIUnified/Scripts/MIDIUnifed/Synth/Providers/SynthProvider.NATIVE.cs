using System.Runtime.InteropServices;
using UnityEngine;

namespace ForieroEngine.MIDIUnified.Synthesizer
{
    public static partial class Synth
    {
        class NATIVESynthProvider : SynthProvider
        {
            public static Synth.SynthSettingsIOS.SoundBankEnum soundBank = Synth.SynthSettingsIOS.SoundBankEnum.sf2;

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX

            public override int Start(Settings settings) => Plugin.Start();
            public override int Stop() => Plugin.Stop();
            public override int SendShortMessage(int Command, int Data1, int Data2) => Plugin.SendMessage(Command, Data1, Data2);

            static class Plugin
            {
                [DllImport("osxsynth")]
                public static extern int Start();

                [DllImport("osxsynth")]
                public static extern int Stop();

                [DllImport("osxsynth")]
                public static extern int SendMessage(int Command, int Data1, int Data2);
            }

#elif UNITY_IOS

		public override int Start(Settings settings) => Plugin.StartSoftSynth(settings.sampleRate, settings.channels, (int)soundBank);						
		public override int Stop() => Plugin.StopSoftSynth();				
		public override int SendShortMessage(int Command, int Data1, int Data2) =>  Plugin.SendSynthMessage(Command, Data1, Data2);
		
        static class Plugin{
		    [DllImport ("__Internal")]
		    public static extern int StartSoftSynth(int sampleRate, int channelCount, int mode);
		
		    [DllImport ("__Internal")]
		    public static extern int StopSoftSynth();
		
		    [DllImport ("__Internal")]
		    public static extern int SendSynthMessage(int Command, int Data1, int Data2);
        }

#elif UNITY_ANDROID

		public override int Start(Settings settings) => Plugin.Start(settings.sampleRate, settings.channels);
		public override int Stop() => Plugin.Stop();
		public override int SendShortMessage(int Command, int Data1, int Data2) => Plugin.SendMidiMessage(Command, Data1, Data2);
		
		static class Plugin
		{
			[DllImport ("androidsynth")]
			public static extern int Start (int x, int y);

			[DllImport ("androidsynth")]
			public static extern int SendMidiMessage (int command, int data1, int data2);

			[DllImport ("androidsynth")]
			public static extern int Stop ();
		}

#else

		public override int Start(Settings settings){
			Debug.LogError ("NATIVE Synthesizer not supported!");
			return 0;
		}

		public override int Stop(){
			Debug.LogError ("NATIVE Synthesizer not supported!");
			return 0;
		}

		public override int SendShortMessage(int Command, int Data1, int Data2){
			Debug.LogError ("NATIVE Synthesizer not supported!");
			return 0;
		}

#endif
        }
    }
}
