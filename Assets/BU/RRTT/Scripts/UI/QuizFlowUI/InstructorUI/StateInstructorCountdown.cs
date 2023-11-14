using BU.RRTT.QuizExample.Scripts.BossSystem;
using Notero.QuizConnector.Instructor;
using TMPro;
using UnityEngine;

namespace BU.RRTT.QuizExample.Scripts.UI.QuizFlowUI.InstructorUI
{
    public class StateInstructorCountdown : BaseInstructorCountIn
    {
        [SerializeField]
        protected TMP_Text m_CountdownText;

        [SerializeField]
        protected Animator m_CircleTimerAnimator;

        [SerializeField]
        protected TMP_Text m_Chapter;

        [SerializeField]
        protected TMP_Text m_Mission;

        [SerializeField]
        protected TMP_Text m_QuizInfoText;

        private const string ChapterIndexFormat = "Chapter: <color=white><font=\"EN_Stylize_Neutral_A\">{0}</font></color>";
        private const string MissionFormat = "Mission: <color=white><font=\"EN_Stylize_Neutral_B\">{0}</font></color>";
        private const string QuizInfoFormat = "<color=#14C287>{0}</color> / {1}";
        
        // RRTT Variables
        [SerializeField]
        private Transform bossPosition;

        [SerializeField]
        private Transform countDownFrame;
        
        [SerializeField]
        private GameObject bossReference;

        private BossList bossList;

        private void Start()
        {
            SetChapterText(Chapter);
            SetMissionText(Mission);
            SetQuizInfoText(CurrentPage, TotalPage);
        }

        public override void OnCustomDataReceive(byte[] data)
        {
            bossList = bossReference.GetComponent<BossList>();
            GameObject boss = Instantiate(bossList.bossPrefabs[data[0]].gameObject, bossPosition);
            boss.transform.SetParent(countDownFrame);
            boss.transform.SetParent(bossPosition);
        }

        /// <summary>
        /// Trigger this method each second
        /// </summary>
        /// <param name="count"></param>
        public override void OnCountdownSet(int count)
        {
            m_CircleTimerAnimator.SetTrigger("TimerCountdown");
            m_CountdownText.text = $"In {count} ...";

            if(count < 1)
            {
                // Run animation
                // ...

                // Run animation finish
                OnNextState?.Invoke();
            }
        }

        #region Custom function

        private void SetChapterText(string text)
        {
            m_Chapter.text = string.Format(ChapterIndexFormat, text);
        }

        private void SetMissionText(string text)
        {
            m_Mission.text = string.Format(MissionFormat, text);
        }

        private void SetQuizInfoText(int currentQuestionIndex, int questionAmount)
        {
            m_QuizInfoText.text = string.Format(QuizInfoFormat, currentQuestionIndex, questionAmount);
        }

        #endregion
    }
}