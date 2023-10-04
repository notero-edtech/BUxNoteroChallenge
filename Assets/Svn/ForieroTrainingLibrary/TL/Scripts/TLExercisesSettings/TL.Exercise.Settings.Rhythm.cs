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
			public RhythmSettings rhythmSettings = new RhythmSettings();

			[Serializable]
			public partial class RhythmSettings
			{
				//[Tooltip("Determines the precision / strictness of the evaluation")]
				[Tooltip("Needs to be removed")]
				public Enums.EvaluationEnum evaluation = Enums.EvaluationEnum.Normal;
			}
		}
	}
}
