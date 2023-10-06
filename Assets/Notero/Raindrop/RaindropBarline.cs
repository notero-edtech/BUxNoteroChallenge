using Notero.Utilities.Pooling;
using System;
using UnityEngine;

namespace Notero.Raindrop
{
    public class RaindropBarline : PoolObject<RaindropBarline>
    {
        [SerializeField]
        protected RectTransform m_BarLineRect;

        [SerializeField]
        protected float m_Speed;

        [SerializeField]
        protected float m_EndScreenTimeInSecond = 0;

        public float EndScreenTimeInSecond => m_EndScreenTimeInSecond;

        public Action OnBarlineEndTime;

        public virtual void SetupBarLine(float speed, float endScreenTimeInSecond)
        {
            m_Speed = speed;
            m_EndScreenTimeInSecond = endScreenTimeInSecond;
        }

        public virtual void UpdatePosition(float currentTime)
        {
            currentTime -= m_EndScreenTimeInSecond;
            float posY = currentTime * -m_Speed;
            Vector2 pos = new Vector2(m_BarLineRect.anchoredPosition.x, posY);
            m_BarLineRect.anchoredPosition = pos;
            var screenEnd = 720;

            if(posY < -screenEnd)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
