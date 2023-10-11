using Notero.MidiGameplay.Core;
using UnityEngine;

namespace Notero.MidiGameplay
{
    public class SimpleTimeProvider : MonoBehaviour, ITimeProvider
    {
        public virtual float CurrentTime
        {
            get
            {
                return CurrentRefTime;
            }
        }

        public virtual float RefTimeNow
        {
            get
            {
                return (float)Time.realtimeSinceStartup * 1000;
            }
        }

        public virtual float ObservedTimeNow
        {
            get
            {
                return (float)AudioSettings.dspTime * 1000;
            }
        }

        public virtual float ObservedTimeStart { get; private set; } = 0;

        public virtual float RefTimeStart { get; private set; } = 0;

        public virtual float CurrentObservedTime => ObservedTimeNow - ObservedTimeStart;

        public virtual float CurrentRefTime => RefTimeNow - RefTimeStart;

        public virtual void Reset()
        {
            ObservedTimeStart = 0;
            RefTimeStart = 0;
        }

        public virtual void SetTimeStart()
        {
            ObservedTimeStart = ObservedTimeNow;
            RefTimeStart = RefTimeNow;
        }

        public virtual void UpdateTime()
        {
        }
    }
}
