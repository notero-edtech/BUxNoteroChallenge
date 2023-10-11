using Notero.Unity.MidiNoteInfo;
using Notero.Utilities.Pooling;
using TMPro;
using UnityEngine;

namespace Notero.Raindrop
{
    public class RaindropNote : PoolObject<RaindropNote>
    {
        [SerializeField]
        protected GameObject m_DefaultRaindropGO;

        [SerializeField]
        protected GameObject[] m_SubRaindropEffects;

        [SerializeField]
        protected RectTransform m_NoteRect;

        [SerializeField]
        protected TMP_Text m_NoteNameText;

        [SerializeField]
        protected TMP_Text m_AccidentalText;

        public float NoteOnTime { get; protected set; }

        public MidiNoteInfo MidiNoteInfo { get; protected set; }

        //Raindrop Sub-Effect indices.
        private const int m_CorrectEffectId = 0;
        private const int m_MissEffectId = 1;

        protected float m_Speed;
        protected float m_Length => (float)MidiNoteInfo.GetNoteDurationInMilliseconds() / 1000f * m_Speed;

        public void Remove()
        {
            Return();
        }

        public void SetDefault()
        {
            m_DefaultRaindropGO.SetActive(true);

            foreach(GameObject effect in m_SubRaindropEffects)
            {
                effect.SetActive(false);
            }
        }

        public void SetNoteNameText(string noteName) => m_NoteNameText.text = noteName;

        public void SetAccidentalText(string accidental) => m_AccidentalText.text = accidental;

        public void SetActiveKeySignatureText(bool isActive)
        {
            m_NoteNameText.gameObject.SetActive(isActive);
            m_AccidentalText.gameObject.SetActive(isActive);
        }

        public void SetActiveAccidentalText(bool isActive) => m_AccidentalText.gameObject.SetActive(isActive);

        public virtual void SetCorrect()
        {
            SetFeedbackEffect(true);
        }

        public virtual void SetMiss()
        {
            SetFeedbackEffect(false);
        }

        //TODO: remove if hold on mode is confirm visual
        public void SetHide()
        {
            m_DefaultRaindropGO.SetActive(false);

            foreach(GameObject effect in m_SubRaindropEffects)
            {
                effect.SetActive(false);
            }
        }

        public void Init(float speed, float noteOnTime)
        {
            m_Speed = speed;
            NoteOnTime = noteOnTime;
            SetNoteRectSize();
        }

        public void SetMidiInfo(MidiNoteInfo info)
        {
            MidiNoteInfo = info;
        }

        /// <summary>
        /// Set Note body vertical length.
        /// </summary>
        public virtual void SetNoteRectSize()
        {
            RectTransform rect = (RectTransform)transform;
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_Length);
        }

        public virtual void UpdatePosition(float currentTime)
        {
            currentTime -= NoteOnTime;
            float posY = currentTime * -m_Speed;
            Vector2 pos = new(m_NoteRect.anchoredPosition.x, posY);
            m_NoteRect.anchoredPosition = pos;
        }

        protected virtual void SetFeedbackEffect(bool isCorrect)
        {
            m_SubRaindropEffects[m_CorrectEffectId].SetActive(isCorrect);
            m_SubRaindropEffects[m_MissEffectId].SetActive(!isCorrect);
        }
    }
}