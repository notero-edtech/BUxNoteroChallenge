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
            public ScaleSettings scaleSettings = new ScaleSettings();

            [Serializable]
            public partial class ScaleSettings
            {
                [Tooltip("")]
                public readonly float toneGap = 1f;
                [Tooltip("")]
                public readonly float toneDuration = 1f;

            }
        }
    }
}
