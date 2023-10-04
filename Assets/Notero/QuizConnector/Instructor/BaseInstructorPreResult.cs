using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Notero.QuizConnector.Instructor
{
    public abstract class BaseInstructorPreResult : BaseQuizPanel
    {
        public UnityEvent OnNextState;

        protected Texture QuestionImage { get; private set; }

        protected string CorrectAnswer { get; private set; }

        protected int AnswerAmount { get; private set; }

        protected int StudentAmount { get; private set; }

        protected Dictionary<string, int> AnswerSummaryDic { get; private set; }

        public virtual void SetQuestionTexture(Texture texture) => QuestionImage = texture;

        public virtual void SetCorrectAnswer(string correctAnswer) => CorrectAnswer = correctAnswer;

        public virtual void SetAnswerAmount(int studentAmount) => AnswerAmount = studentAmount;

        public virtual void SetStudentAmount(int studentAmount) => StudentAmount = studentAmount;

        public virtual void SetAnswerSummaryDic(Dictionary<string, int> answerSummaryDic) => AnswerSummaryDic = answerSummaryDic;
    }
}