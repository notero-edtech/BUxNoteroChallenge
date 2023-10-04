namespace Notero.QuizConnector.Student
{
    public abstract class BaseStudentPopQuizResult : BaseQuizPanel
    {
        protected int CurrentScore { get; private set; }

        protected int QuestionAmount { get; private set; }
        
        public virtual void SetCurrentScore(int score) => CurrentScore = score;

        public virtual void SetQuestionAmount(int questionAmount) => QuestionAmount = questionAmount;
    }
}