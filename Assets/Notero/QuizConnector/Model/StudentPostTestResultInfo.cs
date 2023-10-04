namespace Notero.QuizConnector.Model
{
    public class StudentPostTestResultInfo
    {
        public string StationId;

        public int PreviousFullScore;

        public int CurrentFullScore;

        public int PreviousScore;

        public int CurrentScore;

        public StudentPostTestResultInfo(string stationId, int previousScore, int currentScore, int previousFullScore, int currentFullScore)
        {
            StationId = stationId;
            PreviousFullScore = previousFullScore;
            CurrentFullScore = currentFullScore;
            PreviousScore = previousScore;
            CurrentScore = currentScore;
        }
    }
}