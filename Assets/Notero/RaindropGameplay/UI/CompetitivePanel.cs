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
            if(float.IsNaN(accuracy)) return; // Intercept NaN value
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
        }
    }
}