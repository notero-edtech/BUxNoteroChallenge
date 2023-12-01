using Hendrix.Generic.UI.Elements;
using Notero.MidiGameplay.Core;
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
            Debug.Log(time);
            if (time <= 10 && RunOneTime == 0 && GameLogicController.CheckforRun == true)
            {
                Animetion();
                RunOneTime = 1;
            }
        }

        //public GameObject idel;
        public int RunOneTime = 0;
        public GameObject bad;
        public GameObject notbad;
        public GameObject good;
        public GameObject great;
        public GameObject perfect;
        public GameObject TrunoffP;
        public GameObject TrunoffB;

        public void Animetion()
        {
            if (CompetitivePanel.CountBridge == 0 || CompetitivePanel.CountBridge == 1)
            {
                // idel.SetActive(false);
                bad.SetActive(true);
                TrunoffP.SetActive(false);
                TrunoffB.SetActive(false);
            }
            if (CompetitivePanel.CountBridge == 2)
            {
                // idel.SetActive(false);
                notbad.SetActive(true);
                TrunoffP.SetActive(false);
                TrunoffB.SetActive(false);
            }
            if (CompetitivePanel.CountBridge == 3)
            {
                // idel.SetActive(false);
                good.SetActive(true);
                TrunoffP.SetActive(false);
                TrunoffB.SetActive(false);
            }
            if (CompetitivePanel.CountBridge == 4)
            {
                //idel.SetActive(false);
                great.SetActive(true);
                TrunoffP.SetActive(false);
                TrunoffB.SetActive(false);
            }
            if (CompetitivePanel.CountBridge == 5)
            {
                // idel.SetActive(false);
                perfect.SetActive(true);
                TrunoffP.SetActive(false);
                TrunoffB.SetActive(false);
            }
        }
    }
}