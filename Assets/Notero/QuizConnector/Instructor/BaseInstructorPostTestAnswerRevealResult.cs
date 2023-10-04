using UnityEngine;
using UnityEngine.Events;

namespace Notero.QuizConnector.Instructor
{
    public abstract class BaseInstructorPostTestAnswerRevealResult : BaseQuizPanel
    {
        public UnityEvent<string> OnShowAnswerList;

        public UnityEvent<string> OnSwapResultState;

        public UnityEvent<int> OnNextAnswerReveal;

        public UnityEvent<int> OnPreviousAnswerReveal;

        protected Texture QuestionImage { get; private set; }

        protected int CurrentQuestionIndex { get; private set; }

        protected string CorrectAnswer { get; private set; }

        public virtual void SetQuestionTexture(Texture texture) => QuestionImage = texture;

        public virtual void SetCurrentQuestionIndex(int number) => CurrentQuestionIndex = number;

        public virtual void SetCorrectAnswer(string correctAnswer) => CorrectAnswer = correctAnswer;
    }
}