namespace Notero.QuizConnector.Student
{
    public abstract class BaseStudentPostTestResult : BaseQuizPanel
    {
        protected bool HasPreTestScore { get; set; }
        
        protected int CurrentScore { get; private set; }
        
        protected int PreTestScore { get; private set; }

        protected int QuestionAmount { get; private set; }

        public virtual void SetCurrentScore(int score) => CurrentScore = score;
        
        public virtual void SetPreTestScore(int score, bool hasPreTesResult = true)
        {
            PreTestScore = score;
            HasPreTestScore = hasPreTesResult;
        }

        public virtual void SetQuestionAmount(int questionAmount) => QuestionAmount = questionAmount;

    }
}