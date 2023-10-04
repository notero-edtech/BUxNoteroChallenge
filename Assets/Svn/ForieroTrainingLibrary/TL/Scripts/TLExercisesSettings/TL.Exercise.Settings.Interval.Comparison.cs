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
				public IntervalComparisonSettings comparisonSettings = new IntervalComparisonSettings();

				[Serializable]
				public partial class IntervalComparisonSettings
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
					public Enums.Interval.IntervalFlags intervalFlags1st;

					[Tooltip("")]
					[EnumFlags]
					public Enums.Interval.IntervalFlags intervalFlags2nd;

					[Tooltip("")]
					[EnumFlags]
					public Enums.Interval.IntervalFlags intervalFlags3rd;

					[Tooltip("")]
					public Enums.Interval.CommonToneEnum commonToneEnum = Enums.Interval.CommonToneEnum.Undefined;
					[Tooltip("")]
					public Enums.Interval.NearByMaxDistanceEnum nearByMaxDistanceEnum = Enums.Interval.NearByMaxDistanceEnum.Minor2nd;

					[Tooltip("")]
					public bool allowEqual = false;

					[Tooltip("")]
					public bool showIntervalNames = false;

					#endregion

					[Tooltip("")]
					[EnumFlags]
					public Enums.PlayModeFlags playModeFlags = Enums.PlayModeFlags.Ascending;
				}
			}
		}
	}
}
