using Hendrix.Gameplay.Core;
using Hendrix.Gameplay.Core.Scoring;
using Notero.Unity.MidiNoteInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace BU.Gameplay.Scoring
{
    public class DemoScoringController : BaseScoringProcessor
    {
        private int m_CurrentScore = 0;

        public DemoScoringController(List<MidiNoteInfo> tableCondition) : base(tableCondition) { }

        public override void ProcessNotePressTiming(MidiNoteInfo note, double actionTime)
        {
            ProcessNoteTiming(note, actionTime, "Press");
        }

        public override void ProcessNoteReleaseTiming(MidiNoteInfo note, double actionTime)
        {
            ProcessNoteTiming(note, actionTime, "Release");
        }

        public override void ProcessBlankKeyPress(int midiId, double time)
        {
            var result = NoteTimingScore.Oops;
            UpdateScore(result);
        }

        public override void ProcessBlankKeyRelease(int midiId, double time) { }


        public override void ProcessNoteTiming(MidiNoteInfo noteInfo, double actionTime, string actionStateString)
        {
            var actionState = Enum.Parse<ActionState>(actionStateString);

            var result = NoteTimingScore.Oops;
            var isScoreUpdate = true;

            var isPress = actionState == ActionState.Press;
            var isRelease = actionState == ActionState.Release;
            var isNoteEnd = actionState == ActionState.NoteEnd;

            if(TryGetNoteInfo(noteInfo.MidiId, actionTime, actionState, out var note))
            {
                if(isPress) note.SetIsPressed(true);
                if(isRelease) note.SetIsPlayed(true);

                if(note.IsPressed)
                {
                    result = CalculateTimingScore(note, actionTime, isPress);
                    isScoreUpdate = !isNoteEnd;
                }
                else if(isNoteEnd)
                {
                    note.SetIsPlayed(true);
                }

                OnNoteTimingProcessed?.Invoke(note, result.ToString(), actionState.ToString());
            }
            else
            {
                if(isNoteEnd) isScoreUpdate = false;
            }

            if(isScoreUpdate) UpdateScore(result);
        }

        public override void ProcessNoteTiming(int noteId, double actionTime, ActionState actionState)
        {
            var result = NoteTimingScore.Oops;
            var isScoreUpdate = true;

            var isPress = actionState == ActionState.Press;
            var isRelease = actionState == ActionState.Release;
            var isNoteEnd = actionState == ActionState.NoteEnd;

            if(TryGetNoteInfo(noteId, actionTime, actionState, out var note))
            {
                if(isPress) note.SetIsPressed(true);
                if(isRelease) note.SetIsPlayed(true);

                switch(note.IsPressed)
                {
                    case true:
                        result = CalculateTimingScore(note, actionTime, isPress);
                        isScoreUpdate = !isNoteEnd;
                        break;
                    case false when isNoteEnd:
                        note.SetIsPlayed(true);
                        break;
                }
            }
            else
            {
                if(actionState == ActionState.NoteEnd) isScoreUpdate = false;
            }

            //OnSetNoteResult?.Invoke(noteId, result, actionState);
            if(isScoreUpdate) UpdateScore(result);
        }

        private int CalculateNoteScore(NoteTimingScore timingScore)
        {
            int score = 0;

            switch(timingScore)
            {
                case NoteTimingScore.None: score = 0; break;
                case NoteTimingScore.Oops: score = 0; break;
                case NoteTimingScore.Good: score = m_GoodScore; break;
                case NoteTimingScore.Perfect: score = m_PerfectScore; break;
            }

            return score;
        }

        private void UpdateScore(NoteTimingScore score)
        {
            m_CurrentScore += CalculateNoteScore(score);
            m_TimingScoreList[(int)score]++;
            var result = CreateStudentResultInfo();
            OnScoreUpdated?.Invoke(result);
        }

        public override SelfResultInfo GetScoringInfo() => CreateStudentResultInfo();

        private SelfResultInfo CreateStudentResultInfo()
        {
            int countPerfect = CountResult(NoteTimingScore.Perfect);
            int countGood = CountResult(NoteTimingScore.Good);
            int countOops = CountResult(NoteTimingScore.Oops);
            int maximunResult = Math.Max(m_TotalNote, countPerfect + countGood + countOops);
            countOops += Math.Abs(maximunResult - (countPerfect + countGood + countOops));
            float accuracyPercent = CalculateAccuracy(countPerfect, countGood, countOops);
            int starCount = CalculateStarCount(accuracyPercent);

            return new SelfResultInfo
            {
                PerfectCount = countPerfect,
                GoodCount = countGood,
                OopsCount = countOops,
                StarCount = starCount,
                AccuracyPercent = accuracyPercent,
                StudentCurrentScore = m_CurrentScore
            };
        }

        private float CalculateAccuracy(int countPerfect, int countGood, int countOops)
        {
            int diffTotal = Math.Abs(m_TotalNote - (countPerfect + countGood + countOops));
            int totalScore = (m_TotalNote + diffTotal) * m_PerfectScore;

            return m_CurrentScore * 100f / totalScore;
        }

        private int CalculateStarCount(float accuracyPercent)
        {
            if(accuracyPercent >= m_ThreeStarAccuracy) return 3;
            if(accuracyPercent >= m_TwoStarAccuracy) return 2;
            if(accuracyPercent >= m_OneStarAccuracy) return 1;

            return 0;
        }

        #region Timing Score
        public override NoteTimingScore CalculateTimingScore(MidiNoteInfo note, double actionTime, bool isPress)
        {
            var noteActionTime = isPress ? note.NoteOnTime : note.NoteOffTime;
            double noteEventTime = noteActionTime + m_NoteStartTimeOffset;

            return CalculateTimingScore(noteEventTime, actionTime);
        }

        private NoteTimingScore CalculateTimingScore(double noteEventTime, double actionTime)
        {
            if(IsTimingPerfect(noteEventTime, actionTime)) return NoteTimingScore.Perfect;
            if(IsTimingGood(noteEventTime, actionTime)) return NoteTimingScore.Good;

            return NoteTimingScore.Oops;
        }

        private bool IsTimingPerfect(double noteEventTime, double actionTime)
        {
            var marginTime = Math.Max(m_MinimumTimeMs, m_PerfectTimeMs);

            var perfectTimeStart = noteEventTime - marginTime;
            var perfectTimeStop = noteEventTime + marginTime;

            return perfectTimeStart <= actionTime && perfectTimeStop >= actionTime;
        }

        private bool IsTimingGood(double noteEventTime, double actionTime)
        {
            var marginTime = Math.Max(m_MinimumTimeMs, m_GoodTimeMs);

            var goodTimeStart = noteEventTime - marginTime;
            var goodTimeStop = noteEventTime + marginTime;

            return goodTimeStart <= actionTime && goodTimeStop >= actionTime;
        }

        private int CountResult(NoteTimingScore noteScore)
        {
            return m_TimingScoreList[(int)noteScore];
        }
        #endregion

        //Auxilary Method
        /// <summary>
        /// Try get note that is spawned but has not been interacted with.
        /// </summary>
        /// <param name="midiId"></param>
        /// <param name="actionTime"></param>
        /// <param name="actionState"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        protected bool TryGetNoteInfo(int midiId, double actionTime, ActionState actionState, out MidiNoteInfo note)
        {
            var marginTime = Mathf.Max(m_MinimumTimeMs, m_PerfectTimeMs, m_GoodTimeMs);
            var isPress = actionState == ActionState.Press;
            var isRelease = actionState == ActionState.Release;
            var isNoteEnd = actionState == ActionState.NoteEnd;

            actionTime -= m_NoteStartTimeOffset;

            note = m_TableCondition.FirstOrDefault(noteInfo =>
            {

                bool noteStartRange = actionTime >= noteInfo.NoteOnTime - marginTime &&
                                      actionTime <= noteInfo.NoteOnTime + marginTime;

                bool noteEndRange = actionTime >= noteInfo.NoteOffTime - marginTime &&
                                      actionTime <= noteInfo.NoteOffTime + marginTime;

                bool findPressNote = noteStartRange && isPress;
                bool findEndNote = noteEndRange && isRelease || noteEndRange && isNoteEnd;

                bool foundNote = findPressNote || findEndNote;

                return foundNote && noteInfo.MidiId == midiId && !noteInfo.IsPlayed;
            });
            return note != default;
        }
    }
}
