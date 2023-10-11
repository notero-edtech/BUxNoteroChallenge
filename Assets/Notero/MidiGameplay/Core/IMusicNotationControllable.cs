using Notero.Unity.MidiNoteInfo;
using System.Collections.Generic;

namespace Notero.MidiGameplay.Core
{
    public interface IMusicNotationControllable //for game logic
    {
        public int OctaveInputAmount { get; }
        public int MinimumKeyId { get; }
        public float RaindropScrollSpeed { get; }
        public List<float> LanePosList { get; }

        public void CreateActionCue(MidiNoteInfo info);
        public void RemoveActionCue(int cueId);
    }
}
