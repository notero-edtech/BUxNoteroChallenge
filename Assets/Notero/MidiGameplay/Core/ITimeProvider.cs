namespace Notero.MidiGameplay.Core
{
    public interface ITimeProvider
    {
        // The actual time used by game system in milliseconds.
        public float CurrentTime { get; }

        // Reference time, must be most steady, in milliseconds.
        public float RefTimeNow { get; }

        // Observed time, the one we care about, in milliseconds.
        public float ObservedTimeNow { get; }

        public float CurrentObservedTime { get; }
        public float CurrentRefTime {  get; }
        public float ObservedTimeStart { get; }
        public float RefTimeStart { get; }

        public void SetTimeStart();
        public void Reset();

        public void UpdateTime();
    }
}