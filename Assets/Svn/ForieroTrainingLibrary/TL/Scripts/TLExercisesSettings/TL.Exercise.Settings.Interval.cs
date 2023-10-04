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
			public IntervalSettings intervalSettings = new IntervalSettings();

			[Serializable]
			public partial class IntervalSettings
			{
				/*                [Tooltip("Needs to be removed")]
								public Enums.KeySignatureEnum keySignature = Enums.KeySignatureEnum.CMaj_AMin;
								[Tooltip("Needs to be removed")]
								public Enums.PlayTonicLeadEnum playTonicLead;

								[Tooltip("Needs to be removed")]
								public Enums.PositionInKeyEnum positionInKey = Enums.PositionInKeyEnum.AllTones;

								[Tooltip("Needs to be removed")]
								public Enums.Interval.IntervalFlags intervalsToInclude = Enums.Interval.IntervalFlags.Major2nd | Enums.Interval.IntervalFlags.Major2nd;

								[Tooltip("Needs to be removed")]
								public Enums.Interval.CommonToneEnum commonTone = Enums.Interval.CommonToneEnum.FirstTone;
								[Tooltip("Needs to be removed")]
								public Enums.Interval.NearByMaxDistanceEnum nearByDistance = Enums.Interval.NearByMaxDistanceEnum.Minor2nd;

								[Tooltip("Needs to be removed")]
								public Enums.PlayModeFlags playMode = Enums.PlayModeFlags.Ascending;
								[Tooltip("Needs to be removed")]
								public bool showIntervalName = false;

								[Tooltip("Needs to be removed")]
								public Enums.RepeatModeEnum repeatMode = Enums.RepeatModeEnum.Random;*/

				[Tooltip("")]
				public readonly float toneGap = 1f;
				[Tooltip("")]
				public readonly float toneDuration = 1f;

				public float ToneGap(float multiplier)
				{
					return multiplier * toneGap;
				}

			}
		}
	}
}
