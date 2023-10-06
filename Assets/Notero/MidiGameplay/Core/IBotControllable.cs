using Notero.Unity.MidiNoteInfo;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Notero.MidiGameplay.Bot
{
    public interface IBotControllable
    {
        public UnityEvent<double> UpdateTime { get; }
        public List<MidiNoteInfo> MidiNoteInfos { get; }
        public double GetCurrentTime();
    }
}