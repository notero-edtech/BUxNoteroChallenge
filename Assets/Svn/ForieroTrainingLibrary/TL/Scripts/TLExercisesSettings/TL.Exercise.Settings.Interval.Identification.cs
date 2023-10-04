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
			public partial class IntervalSettings
			{
				public IntervalIdentificationSettings identificationSettings = new IntervalIdentificationSettings();

				[Serializable]
				public partial class IntervalIdentificationSettings
				{
					#region general

					[Tooltip("")]
					public int numberOfQuestions = 10;
					[Tooltip("Pitch Range and Clef that will be displayed and used for exercise generation.")]
					public Enums.PitchRangeAndClefEnum pitchRangeAndClefEnum = Enums.PitchRangeAndClefEnum.UserVocalRange;

					#endregion

					#region exercise

					[Tooltip("")]
					[EnumFlags]
					public Enums.Interval.IntervalFlags intervalFlags1st;

					[Tooltip("")]
					[EnumFlags]
					public Enums.Interval.IntervalFlags intervalFlags2nd;

					[Tooltip("")]
					[EnumFlags]
					public Enums.Interval.IntervalFlags intervalFlags3rd;

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

				}
			}
		}
	}
}
