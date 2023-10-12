namespace Notero.RaindropGameplay.Core
{
    public class SelfResultInfo
    {
        public EditorInfo EditorInfo;
        public StudentResultStatus StudentResultStatus;

        // TODO: This is for full version
        //public string ID;
        //public string Name;
        public string StationNo;

        public int PerfectCount;
        public int GoodCount;
        public int OopsCount;

        public float AccuracyPercent;

        public int Rank;
        public int StarCount;
        //star will be based AccuracyPercent. the amount and range is still being discussed.

        public int AttemptedCount;
        public float StudentCurrentScore;
        public float StudentHighScore;

        public float ClassCurrentHighScore;
        public float ClassAllTimeHighScore;

        public override string ToString()
        {
            string text =
                $"Perfect Counts: {PerfectCount}\n" +
                $"Good Counts: {GoodCount}\n" +
                $"Oops Counts: {OopsCount}\n" +
                $"Accuracy %: {AccuracyPercent}\n" +
                $"Total Score: {StudentCurrentScore}";

            return text;
        }
    }

    public enum StudentResultStatus
    {
        StateDefault,
        StateDisconnect,
        StateHoldOnPlaying,
        StateHoldOnDone
    }

    public enum EditorInfo
    {
        Instructor,
        Student
    }
}