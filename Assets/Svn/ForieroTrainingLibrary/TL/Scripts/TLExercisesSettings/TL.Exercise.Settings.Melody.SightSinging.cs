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
				public MelodySightSingingSettings sightSingingSettings = new MelodySightSingingSettings();

				[Serializable]
				public class MelodySightSingingSettings
				{

					#region general

					[Tooltip("")]
					public int numberOfQuestions = 10;
					[Tooltip("")]
					public Enums.PitchRangeAndClefEnum pitchRangeAndClefEnum = Enums.PitchRangeAndClefEnum.UserVocalRange;
					[Tooltip("")]
					public int BPM = 90;

					#endregion

					#region exercise

					[Tooltip("")]
					public Enums.Melody.MelodyEnum melodyEnum = Enums.Melody.MelodyEnum.PitchOnly;
					[Tooltip("")]
					public int numberOfMeasures = 2;

					// pitches //

					[Tooltip("")]
					public Enums.Scale.ScaleFlags scaleEnum = Enums.Scale.ScaleFlags.Major;

					[Tooltip("")]
					public Enums.Interval.IntervalFlags largestIntervalEnum = Enums.Interval.IntervalFlags.Perfect5th;

					[Tooltip("")]
					public Enums.Interval.IntervalFlags ambitusEnum = Enums.Interval.IntervalFlags.Perfect5th;

					[Tooltip("")]
					public Enums.Melody.MelodyLastToneEnum lastToneEnum = Enums.Melody.MelodyLastToneEnum.I;


					// rhythm //

					[Tooltip("")]
					[EnumMask]
					public Enums.BeatsPerMeasureFlags beatsPerMeasure = Enums.BeatsPerMeasureFlags._4;

					[Tooltip("")]
					[EnumMask]
					public Enums.NotePerBeatFlags notesPerBeat = Enums.NotePerBeatFlags._4;

					[Tooltip("")]
					[EnumMask]
					public Enums.Melody.MelodyNoteFlags noteFlags = Enums.Melody.MelodyNoteFlags.Whole | Enums.Melody.MelodyNoteFlags.Quarter | Enums.Melody.MelodyNoteFlags.Item8th;

					#endregion

					[Tooltip("")]
					[EnumMask]
					public Enums.TimingModifierFlags timeModifierFlags;
				}
			}
		}
	}
}
