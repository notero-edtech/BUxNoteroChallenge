using Notero.Raindrop;
using Notero.Utilities.Pooling;
using System;
using UnityEngine;

namespace BU.NineTails.MidiGameplay.Scripts.Gameplay
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
            var screenEnd = -1280;
            float posY = currentTime * -m_Speed;
            //currentTime -= m_EndScreenTimeInSecond;
            Vector2 pos = new Vector2(m_BarLineRect.anchoredPosition.x, posY);
            m_BarLineRect.anchoredPosition = pos;

            if (posY < -screenEnd)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
