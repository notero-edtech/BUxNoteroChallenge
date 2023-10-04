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
			public partial class MelodySettings
			{
				public MelodyImitationSettings imitationSettings = new MelodyImitationSettings();

				[Serializable]
				public class MelodyImitationSettings
				{
					#region general

					[Tooltip("")]
					public int numberOfQuestions = 10;
					[Tooltip("")]
					public Enums.PitchRangeAndClefEnum pitchRangeAndClefEnum = Enums.PitchRangeAndClefEnum.UserVocalRange;

					[Tooltip("Beats per minute")]
					public int BPM = 90;

					#endregion

					#region exercise

					[Tooltip("")]
					public Enums.Melody.MelodyEnum melodyEnum = Enums.Melody.MelodyEnum.PitchOnly;
					[Tooltip("")]
					public int numberOfTones = 5;

					// pitches //

					[Tooltip("")]
					public Enums.Scale.ScaleFlags scaleEnum = Enums.Scale.ScaleFlags.Major;

					[Tooltip("")]
					public Enums.Interval.IntervalFlags largestIntervalEnum = Enums.Interval.IntervalFlags.Perfect5th;

					[Tooltip("")]
					public Enums.Interval.IntervalFlags ambitusEnum = Enums.Interval.IntervalFlags.Perfect5th;

					[Tooltip("")]
					public Enums.Melody.MelodyLastToneEnum lastToneEnum = Enums.Melody.MelodyLastToneEnum.I;

					#endregion

					[Tooltip("")]
					public Enums.Melody.MelodyPickFrom pickFromEnum = Enums.Melody.MelodyPickFrom.Random;
					[Tooltip("")]
					public Enums.RepeatModeEnum repeatModeEnum = Enums.RepeatModeEnum.Random;

					// pick from keys //

					[Tooltip("")]
					[EnumFlags]
					public Enums.KeyFlags keyFlags = Enums.KeyFlags.CMajor;

					//pick from root tones//

					[Tooltip("")]
					[EnumFlags]
					public Enums.ToneFlags toneFlags = Enums.ToneFlags.C | Enums.ToneFlags.E | Enums.ToneFlags.G;

					[Tooltip("")]
					public Enums.PlayTonicLeadEnum playTonicLeadEnum = Enums.PlayTonicLeadEnum.KeyNotes;


				}
			}
		}
	}
}
