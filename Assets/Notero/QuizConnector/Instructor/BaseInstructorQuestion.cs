using UnityEngine;
using UnityEngine.Events;

namespace Notero.QuizConnector.Instructor
{
    public abstract class BaseInstructorQuestion : BaseQuizPanel
    {
        public UnityEvent OnNextState;

        protected Texture QuestionImage { get; private set; }

        protected int StudentAnswer { get; private set; }

        protected int StudentAmount { get; private set; }

        public virtual void SetQuestionTexture(Texture texture) => QuestionImage = texture;

        public virtual void SetStudentAmount(int amount) => StudentAmount = amount;

        public virtual void OnStudentAnswerReceive(int studentAnswer, int studentAmount)
        {
            StudentAnswer = studentAnswer;
            StudentAmount = studentAmount;
        }
    }
}