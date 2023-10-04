using UnityEngine;

namespace Notero.QuizConnector.Student
{
    public abstract class BaseStudentPreResult : BaseQuizPanel
    {
        protected Texture QuestionImage { get; private set; }

        protected string Answer { get; private set; }

        protected string CorrectAnswer { get; private set; }

        public virtual void SetQuestionTexture(Texture texture) => QuestionImage = texture;

        public virtual void SetAnswer(string answer) => Answer = answer;

        public virtual void SetCorrectAnswer(string correctAnswer) => CorrectAnswer = correctAnswer;
    }
}