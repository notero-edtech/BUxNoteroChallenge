using Notero.QuizConnector.Student;
using TMPro;
using UnityEngine;

namespace BU.RRTT.QuizExample.Scripts.UI.QuizFlowUI.StudentUI
{
    public class StateStudentCountdown : BaseStudentCountIn
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

        private void Start()
        {
            SetChapterText(Chapter);
            SetMissionText(Mission);
            SetQuizInfoText(CurrentPage, TotalPage);
        }

        /// <summary>
        /// Trigger this method each second
        /// </summary>
        /// <param name="count"></param>
        public override void OnCountdownSet(int count)
        {
            m_CircleTimerAnimator.SetTrigger("TimerCountdown");
            m_CountdownText.text = $"In {count} ...";
        }

        public override void OnCustomDataReceive(byte[] data)
        {
            Debug.Log($"NPA-data:{data}");
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