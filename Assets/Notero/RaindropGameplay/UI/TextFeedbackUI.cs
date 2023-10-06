using Hendrix.Gameplay.Core.Scoring;
using Notero.Utilities.Pooling;
using UnityEngine;

namespace Hendrix.Gameplay.UI
{
    public class TextFeedbackUI : PoolObject<TextFeedbackUI>, IFeedbackDisplayable
    {
        [SerializeField]
        private RectTransform m_RectTransform;

        [SerializeField]
        private GameObject m_PerfacFeedback;

        [SerializeField]
        private GameObject m_GoodFeedback;

        [SerializeField]
        private GameObject m_OopsFeedback;

        [SerializeField]
        private float m_AutoReturnDuration = 1;

        private float m_ReturnTime;

        public void SetPosition(Vector2 position) => m_RectTransform.anchoredPosition = position;

        public void SetScale(Vector3 scale) => m_RectTransform.localScale = scale;

        public void SetRotation(Quaternion rotation) => m_RectTransform.rotation = rotation;

        public void SetActive(NoteTimingScore noteScore)
        {
            m_PerfacFeedback.SetActive(noteScore == NoteTimingScore.Perfect);
            m_GoodFeedback.SetActive(noteScore == NoteTimingScore.Good);
            m_OopsFeedback.SetActive(noteScore == NoteTimingScore.Oops);
        }

        protected override void OnRented()
        {
            base.OnRented();
            m_ReturnTime = Time.time + m_AutoReturnDuration;
        }

        protected override void Update()
        {
            base.Update();

            if(Time.time >= m_ReturnTime)
            {
                Return();
            }
        }
    }
}