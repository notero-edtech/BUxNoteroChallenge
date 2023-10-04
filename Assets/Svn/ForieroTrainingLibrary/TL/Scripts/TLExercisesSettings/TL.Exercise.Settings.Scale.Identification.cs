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
            public partial class ScaleSettings
            {
                public ScaleIdentificationSettings identificationSettings = new ScaleIdentificationSettings();

                [Serializable]
                public partial class ScaleIdentificationSettings
                {
                    #region general

                    [Tooltip("Pitch Range and Clef that will be displayed and used for exercise generation.")]
                    public Enums.PitchRangeAndClefEnum pitchRangeAndClefEnum = Enums.PitchRangeAndClefEnum.Octave_D3_Db5;
                    [Tooltip("")]
                    public int numberOfQuestions = 10;

                    #endregion

                    #region exercise

                    [Tooltip("")]
                    [EnumFlags]
                    public Enums.Scale.ScaleFlags scaleFlags = Enums.Scale.ScaleFlags.Major;

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
                    public bool playTonicOfKey = false;
                }
            }
        }
    }
}
