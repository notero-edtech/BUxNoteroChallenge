/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
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
				public RhythmImitationSettings imitationSettings = new RhythmImitationSettings();

				[Serializable]
				public partial class RhythmImitationSettings
				{
					#region general

					[Tooltip("")]
					public int numberOfQuestions = 10;

					[Tooltip("Beats per minute")]
					public int BPM = 90;

					#endregion

					#region exercise

					[Tooltip("")]
					public Enums.EvaluationEnum evaluationEnum = Enums.EvaluationEnum.Normal;

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

					[Tooltip("Percentage of notes that will be tied")]
					public int tieNotesPercentage = 20;
					#endregion

				}
			}
		}
	}
}
