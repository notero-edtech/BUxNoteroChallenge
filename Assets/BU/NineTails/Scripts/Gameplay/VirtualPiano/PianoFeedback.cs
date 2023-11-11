using Notero.RaindropGameplay.Core.Scoring;
using Notero.Unity.MidiNoteInfo;
using Notero.Unity.UI.VirtualPiano;
using Notero.RaindropGameplay.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BU.NineTails.Scripts.UI.VirtualPiano
{
    public class PianoFeedback : MonoBehaviour
    {
        [SerializeField]
        protected PianoFeedbackUI m_PianoFeedbackUI = default;

        protected Dictionary<int, PianoFeedbackUI> m_PianoFeedbackList = new Dictionary<int, PianoFeedbackUI>();

        private BaseVirtualPiano m_VirtualPiano;
        private List<float> m_LanePosXList;
        private float m_IndicatorPosY;
        private Rect m_WhiteKeyRect;
        private Rect m_BlackKeyRect;

        public virtual void Init(BaseVirtualPiano virtualPianoController, List<float> lanePosX)
        {
            m_VirtualPiano = virtualPianoController;
            m_IndicatorPosY = virtualPianoController.IndicatorPosY;
            m_WhiteKeyRect = virtualPianoController.WhiteKeyRect;
            m_BlackKeyRect = virtualPianoController.BlackKeyRect;
            m_LanePosXList = lanePosX;
        }
        public void UpdateFeedbackBlankKeyPress(int midiId, double time)
        {
            ShowFeedback(midiId, NoteTimingScore.Oops);
        }

        public void UpdateFeedbackBlankKeyRelease(int midiId, double time)
        {
            RemoveFeedback(midiId, NoteTimingScore.Oops);
        }

        public void UpdateFeedbackOnNoteEnd(MidiNoteInfo note)
        {
            if (note.IsPressed)
            {
                UpdateFeedback(note.MidiId, NoteTimingScore.None, ActionState.NoteEnd);
            }
            else
            {
                UpdateFeedback(note.MidiId, NoteTimingScore.Oops, ActionState.NoteEnd);
            }
        }

        public virtual void UpdateFeedback(MidiNoteInfo noteInfo, string resultString, string actionStateString)
        {
            var midiId = 0;
            if (noteInfo != null) midiId = noteInfo.MidiId;

            var result = Enum.Parse<NoteTimingScore>(resultString);
            var actionState = Enum.Parse<ActionState>(actionStateString);

            UpdateFeedback(midiId, result, actionState);
        }

        public virtual void UpdateFeedback(int midiId, NoteTimingScore result, ActionState actionState)
        {
            switch (actionState)
            {
                case ActionState.Press:
                    ShowFeedback(midiId, result);
                    break;
                case ActionState.Release:
                    RemoveFeedback(midiId, result);
                    break;
                case ActionState.NoteEnd:
                    RemoveFeedback(midiId, result);
                    break;
            }
        }

        private void ShowFeedback(int midiId, NoteTimingScore noteScore)
        {
            var index = m_VirtualPiano.GetPianoKeyIndex(midiId);

            if (m_VirtualPiano.IsMidiIdInRange(midiId) && GetPositionXByIndex(index, out var posX))
            {
                var isBlackKey = VirtualPianoHelper.IsBlackKey(index);
                var pianoFeedbackInfo = new PianoFeedbackInfo()
                {
                    PositionX = posX,
                    PositionY = m_IndicatorPosY,
                    Width = isBlackKey ? m_BlackKeyRect.width : m_WhiteKeyRect.width
                };

                SpawnPianoFeedback(midiId, noteScore, pianoFeedbackInfo);
            }
        }

        private void RemoveFeedback(int midiId, NoteTimingScore noteScore)
        {
            if (!m_PianoFeedbackList.TryGetValue(midiId, out var pianoFeedback)) return;

            pianoFeedback.SetRelease(noteScore);
            m_PianoFeedbackList.Remove(midiId);
        }

        private bool GetPositionXByIndex(int index, out float posX)
        {
            var isValid = !index.Equals(-1);
            posX = isValid ? m_LanePosXList[index] : 0;

            return isValid;
        }

        private void SpawnPianoFeedback(int midiId, NoteTimingScore noteScore, PianoFeedbackInfo pianoFeedbackInfo)
        {
            if (m_PianoFeedbackList.ContainsKey(midiId)) return;

            var pianoFeedback = m_PianoFeedbackUI.Rent(transform);
            var posX = pianoFeedbackInfo.PositionX + pianoFeedbackInfo.Width / 2;
            var posY = pianoFeedbackInfo.PositionY;

            pianoFeedback.SetPosition(new Vector2(posX, posY));
            pianoFeedback.SetScale(Vector3.one);
            pianoFeedback.SetActive(noteScore);

            m_PianoFeedbackList.Add(midiId, pianoFeedback);
        }

        protected struct PianoFeedbackInfo
        {
            public float PositionX;
            public float PositionY;
            public float Width;
        }

    }
}
