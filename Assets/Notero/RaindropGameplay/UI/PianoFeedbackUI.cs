using Notero.RaindropGameplay.Core.Scoring;
using Notero.Utilities.Pooling;
using UnityEngine;

namespace Notero.RaindropGameplay.UI
{
    public class PianoFeedbackUI : PoolObject<PianoFeedbackUI>, IFeedbackDisplayable
    {
        [SerializeField]
        private RectTransform m_RectTransform;

        [SerializeField]
        private GameObject m_HitFeedback;

        [SerializeField]
        private GameObject m_MissFeedback;

        [SerializeField]
        private float m_AutoReturnDuration = 1;

        private float m_ReturnTime;

        private bool m_IsPress;

        public void SetPosition(Vector2 position) => m_RectTransform.anchoredPosition = position;
        public void SetRotation(Quaternion rotation) => m_RectTransform.rotation = rotation;
        public void SetPosition(Vector2 position, Quaternion quaternion)
        {
            SetPosition(position);
            SetRotation(quaternion);
        }

        public void SetScale(Vector3 scale) => m_RectTransform.localScale = scale;


        public virtual void SetActive(NoteTimingScore noteScore)
        {
            m_IsPress = true;
            m_ReturnTime = Time.time + m_AutoReturnDuration;
            m_HitFeedback.SetActive(noteScore == NoteTimingScore.Perfect || noteScore == NoteTimingScore.Good);
            m_MissFeedback.SetActive(noteScore == NoteTimingScore.Oops);

            if(noteScore != NoteTimingScore.Oops) return;
            m_IsPress = false;
        }

        public virtual void SetRelease(NoteTimingScore noteScore)
        {
            m_IsPress = false;
            m_ReturnTime = Time.time + m_AutoReturnDuration;
            m_HitFeedback.SetActive(false);
            m_MissFeedback.SetActive(noteScore == NoteTimingScore.Oops || noteScore == NoteTimingScore.Good);
        }

        protected override void Update()
        {
            base.Update();

            if(Time.time >= m_ReturnTime && !m_IsPress) Return();
        }
    }
}