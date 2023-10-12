using Hendrix.Generic.UI.Elements;
using Notero.Utilities;
using TMPro;
using UnityEngine;

namespace Notero.RaindropGameplay.UI
{
    public class TimerPanel : MonoBehaviour
    {
        [SerializeField]
        private SmoothProgressBar m_TimerBar;

        [SerializeField]
        private TMP_Text m_TimerText;

        private float m_MaxTime;

        public void SetActive(bool isActive) => gameObject.SetActive(isActive);

        public void SetTimerBarMaxValue(float value)
        {
            m_TimerBar.maxValue = value;
            m_MaxTime = value;
        }

        public void SetTimerBarCurrentValue(float value) => m_TimerBar.value = value;

        public void SetTimerText(float seconds) => m_TimerText.text = DataFormatValidator.SecondsToTimeFormat(seconds);

        public void OnGameplayTimeUpdate(float seconds)
        {
            var time = m_MaxTime - seconds;

            SetTimerBarCurrentValue(seconds);
            SetTimerText(time);
        }
    }
}