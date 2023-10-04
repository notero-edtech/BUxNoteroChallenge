namespace Notero.QuizConnector.Model
{
    public class StudentQuizResultInfo
    {
        public string StationId;

        public int FullScore;

        public int CurrentScore;

        public StudentQuizResultInfo(string stationId, int currentScore, int fullScore)
        {
            StationId = stationId;
            FullScore = fullScore;
            CurrentScore = currentScore;
        }
    }
}