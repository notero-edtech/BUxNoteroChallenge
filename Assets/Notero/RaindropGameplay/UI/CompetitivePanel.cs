using Notero.MidiGameplay.Core;
using Notero.RaindropGameplay.Core;
using UnityEngine;

namespace Notero.RaindropGameplay.UI
{
    public class CompetitivePanel : MonoBehaviour
    {
        [SerializeField]
        private TimerPanel m_TimerPanel;

        [SerializeField]
        private AccuracyMeterBarPanel m_AccuracyMeterBar;

        [SerializeField]
        private ScorePanel m_ScorePanel;

        public void SetActive(bool isActive) => gameObject.SetActive(isActive);

        public void SetupTimerDisplay(float duration)
        {
            m_TimerPanel.SetTimerBarMaxValue(duration);
            m_TimerPanel.SetTimerText(duration);
            m_TimerPanel.SetTimerBarCurrentValue(0);
        }

        public void UpdateAccuracyMeterBar(float accuracy, int startCount)
        {
            if (float.IsNaN(accuracy)) return; // Intercept NaN value
            m_AccuracyMeterBar.SetProgressBarValue(accuracy);
            m_AccuracyMeterBar.SetAccuracyPercentText(accuracy);
            m_AccuracyMeterBar.SetStar(startCount);
        }

        public void UpdateScore(float studentCurrentScore) => m_ScorePanel.SetScoreText(studentCurrentScore);

        public void SetTimerActive(bool isActive)
        {
            m_TimerPanel.SetActive(isActive);
        }

        public void OnGameplayTimeUpdate(float seconds)
        {
            m_TimerPanel.OnGameplayTimeUpdate(seconds);
        }

        public void SetupAccuracyMeterBar(GameplayConfig config = null) => m_AccuracyMeterBar.SetStarsPosition(config);

        public void SetAccuracyMeterActive(bool isActive) => m_AccuracyMeterBar.SetActive(isActive);

        public void SetScoreActive(bool isActive) => m_ScorePanel.SetActive(isActive);

        public void OnScoreUpdated(SelfResultInfo studentResultInfo)
        {
            UpdateAccuracyMeterBar(studentResultInfo.AccuracyPercent, studentResultInfo.StarCount);
            UpdateScore(studentResultInfo.StudentCurrentScore);
            SetSpawnBridge(studentResultInfo.AccuracyPercent);
        }


        public GameObject Bridge1;
        public GameObject Bridge2;
        public GameObject Bridge3;
        public GameObject Bridge4;
        public GameObject Bridge5;
        public static int CountBridge = 0;
        public void SetSpawnBridge(float Percent)
        {
            if (Percent >= 0 && CountBridge == 0) //ความคีบหน้าเมื่อถึง 19 สะพานอันที่ 1
            {
                CountBridge++;
                Bridge1.SetActive(true);
            }
            else if (Percent >= 20 && CountBridge == 1) //ความคีบหน้าเมื่อถึง 39 สะพานอันที่ 2
            {
                CountBridge++;
                Bridge2.SetActive(true);
            }
            else if (Percent >= 40 && CountBridge == 2) //ความคีบหน้าเมื่อถึง 59 สะพานอันที่ 3
            {
                CountBridge++;
                Bridge3.SetActive(true);
            }
            else if (Percent >= 60 && CountBridge == 3) //ความคีบหน้าเมื่อถึง 79 สะพานอันที่ 4
            {
                CountBridge++;
                Bridge4.SetActive(true);
            }
            else if (Percent >= 80 && CountBridge == 4) //ความคีบหน้าเมื่อถึง 100 สะพานอันที่ 5
            {
                CountBridge++;
                Bridge5.SetActive(true);
            }
            //check = 1;
        }
    }
}