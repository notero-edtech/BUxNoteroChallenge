using Notero.MidiGameplay.Core;
using Notero.Unity.MidiNoteInfo;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace Notero.RaindropGameplay.Core.Scoring
{
    public abstract class BaseScoringProcessor
    {
        public UnityEvent<MidiNoteInfo, string, string> OnNoteTimingProcessed = new();
        public UnityEvent<SelfResultInfo> OnScoreUpdated = new();

        protected int m_PerfectScore;
        protected int m_GoodScore;
        protected int m_ThreeStarAccuracy;
        protected int m_TwoStarAccuracy;
        protected int m_OneStarAccuracy;

        protected List<MidiNoteInfo> m_TableCondition;
        protected int m_TotalNote;
        protected int[] m_TimingScoreList;
        protected int m_PerfectTimeMs;
        protected int m_GoodTimeMs;
        protected float m_MinimumTimeMs = 309.88f;
        protected double m_NoteStartTimeOffset;

        public BaseScoringProcessor(List<MidiNoteInfo> tableCondition)
        {
            m_TimingScoreList = new int[3];
            m_TableCondition = tableCondition.OrderBy(info => info.NoteOnTime).ToList();
            m_TotalNote = tableCondition.Count * 2;
        }

        public abstract void ProcessNoteTiming(MidiNoteInfo note, double actionTime, string actionState);
        public abstract void ProcessNoteTiming(int noteId, double actionTime, ActionState actionState);
        public abstract SelfResultInfo GetScoringInfo();
        public abstract NoteTimingScore CalculateTimingScore(MidiNoteInfo note, double actionTime, bool isPress);

        public void SetGameplayConfig(GameplayConfig config)
        {
            m_GoodTimeMs = config.GoodTimeMs;
            m_PerfectTimeMs = config.PerfectTimeMs;
            m_PerfectScore = config.PerfectScore;
            m_GoodScore = config.GoodScore;
            m_ThreeStarAccuracy = config.ThreeStarAccuracy;
            m_TwoStarAccuracy = config.TwoStarAccuracy;
            m_OneStarAccuracy = config.OneStarAccuracy;
        }

        public void SetNoteStartTimeOffset(double noteStartTimeOffset) => m_NoteStartTimeOffset = noteStartTimeOffset;
        public void SetMinimumTime(float minimumTime) => m_MinimumTimeMs = minimumTime;

        public abstract void ProcessNotePressTiming(MidiNoteInfo note, double actionTime);

        public abstract void ProcessNoteReleaseTiming(MidiNoteInfo note, double actionTime);

        public abstract void ProcessBlankKeyPress(int midiId, double time);

        public abstract void ProcessBlankKeyRelease(int midiId, double time);
    }
}
