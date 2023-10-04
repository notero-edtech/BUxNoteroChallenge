/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace ForieroEngine.Music.Training
{
	public static partial class TL
	{
		public partial class ExerciseSettings
		{
			public partial class ChordSettings
			{
				public ChordSingingSettings singingSettings = new ChordSingingSettings();

				[Serializable]
				public class ChordSingingSettings
				{
					#region general

					[Tooltip("")]
					public int numberOfQuestions = 10;
					[Tooltip("")]
					public Enums.PitchRangeAndClefEnum pitchRangeAndClefEnum = Enums.PitchRangeAndClefEnum.UserVocalRange;

					#endregion

					#region exercise

					[Tooltip("")]
					[EnumFlags]
					public Enums.Chord.ChordTypeFlags chordTypeFlags = Enums.Chord.ChordTypeFlags.Major | Enums.Chord.ChordTypeFlags.Minor;

					[Tooltip("")]
					public Enums.Chord.ChordVoicingEnum chordVoicingEnum = Enums.Chord.ChordVoicingEnum.OpenVoicing;

					#endregion

					[Tooltip("")]
					[EnumFlags]
					public Enums.PlayModeFlags playModeFlags = Enums.PlayModeFlags.Ascending;


					[Tooltip("")]
					public Enums.PickFromEnum pickFromEnum = Enums.PickFromEnum.Random;
					[Tooltip("")]
					public Enums.RepeatModeEnum repeatModeEnum = Enums.RepeatModeEnum.Random;

					//pick from keys//

					[Tooltip("")]
					[EnumFlags]
					public Enums.KeyFlags keyFlags = Enums.KeyFlags.CMajor;
					[Tooltip("")]
					public Enums.RootOfQuestionEnum rootOfQuestion = Enums.RootOfQuestionEnum.DiatonicMatch;

					//pick from root tones//

					[Tooltip("")]
					[EnumFlags]
					public Enums.ToneFlags toneFlags = Enums.ToneFlags.C | Enums.ToneFlags.E | Enums.ToneFlags.G;

					[Tooltip("")]
					public Enums.PlayTonicLeadEnum playTonicLeadEnum = Enums.PlayTonicLeadEnum.Undefined;


					[Tooltip("Beats per Minute")]
					public int BPM = 90;

				}
			}
		}
	}
}
