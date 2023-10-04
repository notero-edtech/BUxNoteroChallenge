/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using System;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public partial class ExerciseOptions
        {
            public partial class ChordOptions
            {
                public ChordProgressionsOptions progressionsOptions = new ChordProgressionsOptions();

                [Serializable]
                public class ChordProgressionsOptions
                {
                    public GeneralOptions generalOptions = new GeneralOptions();
                    public AnswerOptions answerOptions = new AnswerOptions();
                    public QuestionOptions questionOptions = new QuestionOptions();
                    public ExerciseOptions exerciseOptions = new ExerciseOptions();

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

                    [Serializable]
                    public class ExerciseOptions
                    {

                    }
                }
            }
        }
    }
}
