using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Notero.Unity.Networking.Example.Countdown
{
    public class CountDownPanel : MonoBehaviour
    {
        [SerializeField]
        private Text m_CountdownText;

        [SerializeField]
        private Image m_Background;

        private bool m_IsCountingDown;

        private void Awake()
        {
            SetText(string.Empty);
        }

        public void Countdown(float delay = 0)
        {
            if(m_IsCountingDown)
                return;

            StartCoroutine(CountdownProcess(delay));
        }

        IEnumerator CountdownProcess(float delay)
        {
            m_IsCountingDown = true;
            float startTime = Time.realtimeSinceStartup;
            float changeInterval = delay;
            float nextTime = startTime + changeInterval;
            yield return new WaitUntil(() => Time.realtimeSinceStartup > nextTime);
            int number = 5;
            while(number >= 0)
            {
                SetText(number.ToString());
                changeInterval += 1;
                nextTime = startTime + changeInterval;
                number--;
                yield return new UnityEngine.WaitUntil(() => Time.realtimeSinceStartup > nextTime);
            }
            SetText(string.Empty);
            m_IsCountingDown = false;
        }

        void SetText(string value)
        {
            m_CountdownText.text = value;
            m_CountdownText.enabled = !string.IsNullOrEmpty(value);
            m_Background.enabled = !string.IsNullOrEmpty(value);
        }
    } 
}
