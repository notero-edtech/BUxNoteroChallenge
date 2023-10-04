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
			public MelodySettings melodySettings = new MelodySettings();

			[Serializable]
			public partial class MelodySettings
			{
			}
		}
	}
}
