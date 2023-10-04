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
			public partial class RhythmSettings
			{
				public RhythmDictationSettings dictationSettings = new RhythmDictationSettings();

				[Serializable]
				public partial class RhythmDictationSettings
				{

					#region general

					[Tooltip("")]
					public int numberOfQuestions = 10;
					[Tooltip("")]
					public int BPM = 90;

					#endregion

					#region exercise

					[Tooltip("")]
					public int numberOfMeasures = 1;

					[Tooltip("")]
					[EnumMask]
					public Enums.BeatsPerMeasureFlags beatsPerMeasure = Enums.BeatsPerMeasureFlags._4;

					[Tooltip("")]
					[EnumMask]
					public Enums.NotePerBeatFlags notesPerBeat = Enums.NotePerBeatFlags._4;

					[Tooltip("")]
					[EnumMask]
					public Enums.NoteAndRestFlags noteFlags = Enums.NoteAndRestFlags.Quarter;

					[Tooltip("")]
					[EnumMask]
					public Enums.NoteAndRestFlags restFlags = Enums.NoteAndRestFlags.Quarter;

					[Tooltip("")]
					[EnumMask]
					public Enums.TupletNoteAndRestFlags tupletNoteFlags;

					[Tooltip("")]
					[EnumMask]
					public Enums.TupletNoteAndRestFlags tupletRestFlags;

					[Tooltip("")]
					[EnumMask]
					public Enums.TimingModifierFlags timeModifierFlags;

					#endregion
				}
			}
		}
	}
}
