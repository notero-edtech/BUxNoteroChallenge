using FileHelpers;

namespace Notero.Report
{
    [DelimitedRecord(",")]
    public class GameplayRecord
    {
        public string Station;
        public float Accuracy;
        public int Perfect;
        public int Good;
        public int Oops;
        public int Score;
        public bool Finished;
    }
}
