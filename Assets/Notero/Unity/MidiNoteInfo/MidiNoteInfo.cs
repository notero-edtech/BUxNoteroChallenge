namespace Notero.Unity.MidiNoteInfo
{
    public class MidiNoteInfo
    {
        /// <summary>
        /// Music note id, for piano should be 21-108
        /// </summary>
        public readonly int MidiId;

        public readonly float AccidentalNumber;

        public readonly float Scale;

        /// <summary>
        /// Track 0 is left hand, track 1 is right hand
        /// </summary>
        public readonly int TrackIndex;

        /// <summary>
        /// Time when midi note start triggering in milliseconds
        /// </summary>
        public readonly double NoteOnTime;

        /// <summary>
        /// Time when midi note end triggering in milliseconds
        /// </summary>
        public readonly double NoteOffTime;

        public readonly float BPM;

        /// <summary>
        /// This is a unique id that set when assign note data to the specific raindrop GO
        /// </summary>
        public int RaindropNoteId { get; private set; }

        /// <summary>
        /// True when this note is already scored
        /// </summary>
        public bool IsPlayed { get; private set; }

        /// <summary>
        /// True when this note is already pressed
        /// </summary>
        public bool IsPressed { get; private set; }

        /// <summary>
        /// True when this note is already start to play,
        /// piano change to raindrop in color
        /// </summary>
        public bool IsStart { get; private set; }

        public bool IsUpdatedScore { get; private set; }

        public MidiNoteInfo(int trackIndex, int midiId, double noteOnTime, double noteOffTime, float accidentalNumber, float scale, float bpm)
        {
            TrackIndex = trackIndex;
            MidiId = midiId;
            NoteOnTime = noteOnTime;
            NoteOffTime = noteOffTime;
            AccidentalNumber = accidentalNumber;
            Scale = scale;
            BPM = bpm;
        }

        /// <summary>
        /// Get note duration in milliseconds
        /// </summary>
        /// <returns></returns>
        public double GetNoteDurationInMilliseconds() => NoteOffTime - NoteOnTime;

        /// <summary>
        /// This is a unique id that set when assign note data to the specific raindrop GO
        /// </summary>
        public void SetRaindropNoteId(int id) => RaindropNoteId = id;

        /// <summary>
        /// Set to true when this note is already scored
        /// </summary>
        public void SetIsPlayed(bool isPlayed) => IsPlayed = isPlayed;

        /// <summary>
        /// Set to true when this note is already pressed
        /// </summary>
        public void SetIsPressed(bool isPressed) => IsPressed = isPressed;

        /// <summary>
        /// Set to true when this note is already start to play,
        /// piano change to raindrop in color
        /// </summary>
        /// <param name="isStart"></param>
        public void SetIsStart(bool isStart) => IsStart = isStart;
        public void SetIsUpdatedScore(bool isUpdatedScore) => IsUpdatedScore = isUpdatedScore;
    }
}