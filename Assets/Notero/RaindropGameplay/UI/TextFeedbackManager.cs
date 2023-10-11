using Hendrix.Gameplay.Core.Scoring;
using Notero.Unity.MidiNoteInfo;
using System;
using UnityEngine;

namespace Hendrix.Gameplay.UI
{
    public class TextFeedbackManager : MonoBehaviour
    {
        [SerializeField]
        protected TextFeedbackUI m_TextFeedbackUI = default;

        public void UpdateFeedbackBlankKeyPress(int midiId, double time)
        {

        }

        public void UpdateFeedbackBlankKeyRelease(int midiId, double time)
        {
            ShowFeedback(NoteTimingScore.Oops);
        }

        public void UpdateFeedbackOnNoteEnd(MidiNoteInfo note)
        {
            if(note.IsPressed)
            {
                UpdateFeedback(note.MidiId, NoteTimingScore.None, ActionState.NoteEnd);
            }
            else
            {
                UpdateFeedback(note.MidiId, NoteTimingScore.Oops, ActionState.NoteEnd);
            }
        }

        public virtual void UpdateFeedback(int midiId, NoteTimingScore result, ActionState actionState)
        {
            if(actionState is ActionState.Release or ActionState.NoteEnd && result != NoteTimingScore.None)
            {
                ShowFeedback(result);
            }
        }

        public virtual void UpdateFeedback(MidiNoteInfo noteInfo, string resultString, string actionStateString)
        {
            var midiId = noteInfo.MidiId;

            var result = Enum.Parse<NoteTimingScore>(resultString);
            var actionState = Enum.Parse<ActionState>(actionStateString);

            UpdateFeedback(midiId, result, actionState);
        }

        private void ShowFeedback(NoteTimingScore noteScore)
        {
            var textFeedback = m_TextFeedbackUI.Rent(transform);
            const int posY = -50;

            textFeedback.SetPosition(new Vector2(0, posY));
            textFeedback.SetScale(Vector3.one);
            textFeedback.SetActive(noteScore);
        }
    }
}
