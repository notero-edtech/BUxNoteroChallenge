/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using System;
using ForieroEngine.Music.Training.Classes.Pitches;
using UnityEngine;

namespace ForieroEngine.Music.Training
{
	public static partial class TL
	{
		[Serializable]
		public partial class ExerciseSettings
		{
			[Tooltip("")]
			public readonly int instrumentChannel = 0;
			[Tooltip("")]
			public readonly int percussionChannel = 9;

			[Tooltip("")]
			public Enums.MIDI.ProgramEnum instrument = Enums.MIDI.ProgramEnum.AcousticGrandPiano;
			[Tooltip("")]
			public int instrumentAttack = 100;

			[Tooltip("")]
			public Enums.MIDI.PercussionEnum rhythm = Enums.MIDI.PercussionEnum.HandClap;
			[Tooltip("")]
			public int rhythmAttack = 100;

			[Tooltip("")]
			public Enums.MIDI.PercussionEnum metronomHeavy = Enums.MIDI.PercussionEnum.HighWoodBlock;
			[Tooltip("")]
			public int metronomHeavyAttack = 100;

			[Tooltip("")]
			public Enums.MIDI.PercussionEnum metronomLight = Enums.MIDI.PercussionEnum.LowWoodBlock;
			[Tooltip("")]
			public int metronomLightAttack = 100;

			[Tooltip("")]
			public Enums.MIDI.PercussionEnum metronomSubdivision = Enums.MIDI.PercussionEnum.LowConga;
			[Tooltip("")]
			public int metronomSubdivisionAttack = 60;

			[Tooltip("")]
			public PitchRange pitchRange = PitchRange.FromPreset(Enums.PitchRangesPresetEnum.Tenor);
		}
	}
}
