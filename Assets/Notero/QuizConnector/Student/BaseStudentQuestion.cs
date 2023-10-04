using UnityEngine;
using UnityEngine.Events;

namespace Notero.QuizConnector.Student
{
    public abstract class BaseStudentQuestion : BaseQuizPanel
    {
        public UnityEvent OnNextState;

        protected Texture QuestionImage { get; private set; }

        protected PianoStates PianoStates { get; private set; }

        protected string Choice { get; private set; }

        public virtual void OnPianoStateReceive(PianoStates pianoState, string choice)
        {
            PianoStates = pianoState;
            Choice = choice;
        }

        public virtual void SetQuestionTexture(Texture texture) => QuestionImage = texture;
    }

    public enum PianoStates
    {
        PIANO_WAITFORINPUT,
        PIANO_PRESSING,
        PIANO_SUBMITTED
    }
}