/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using System;
using ForieroEngine.Music.Training.Classes.Pitches;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        [Serializable]
        public partial class ExerciseOptions
        {
            public GeneralOptions generalOptions = new GeneralOptions();
            public AnswerOptions answerOptions = new AnswerOptions();
            public QuestionOptions questionOptions = new QuestionOptions();

            [Serializable]
            public class GeneralOptions
            {

            }

            [Serializable]
            public class AnswerOptions
            {

            }

            [Serializable]
            public class QuestionOptions
            {

            }
        }
    }
}
