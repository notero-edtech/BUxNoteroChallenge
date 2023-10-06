using UnityEngine;

namespace Notero.Utilities.Pooling.Samples
{
    public class SamplePoolObject : PoolObject<SamplePoolObject>
    {
        [SerializeField]
        private float m_AutoReturnDuration = 1;

        private float m_ReturnTime;

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
