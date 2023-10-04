using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static System.Threading.Tasks.Task;

namespace Notero.Unity.Networking.Common
{
    public class WaitUntil
    {
        private Func<bool> m_Predicate;
        private int m_TimeOut;
        private bool m_IsTimeOutMode;

        public bool IsTimeOut
        {
            get; private set;
        }

        public bool IsConditionMet
        {
            get; private set;
        }

        /// <summary>
        /// Wait time in milliseconds
        /// </summary>
        private const int m_DelayTime = 25;

        public WaitUntil(Func<bool> predicate)
        {
            m_Predicate = predicate;
            m_IsTimeOutMode = false;
        }

        public WaitUntil(Func<bool> predicate, int timeOut)
        {
            m_Predicate = predicate;
            m_TimeOut = timeOut;
            m_IsTimeOutMode = true;
        }

        public async Task Run()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            while(!m_Predicate())
            {
                if(m_IsTimeOutMode)
                {
                    IsTimeOut = timer.Elapsed.TotalMilliseconds >= m_TimeOut;
                    if(IsTimeOut)
                        break;
                }

                await Delay(m_DelayTime);
            }

            IsConditionMet = m_Predicate();
            timer.Stop();
        }
    } 
}