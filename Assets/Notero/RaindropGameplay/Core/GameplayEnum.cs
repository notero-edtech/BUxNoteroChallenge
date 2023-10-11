namespace Hendrix.Gameplay.Core.Scoring
{
    public enum NoteTimingScore
    {
        Perfect = 2,
        Good = 1,
        Oops = 0,
        None = -1
    }

    public enum ActionState
    {
        Press,
        Release,
        NoteStart,
        NoteEnd
    }
}