using Notero.QuizConnector.Instructor;
using Notero.Unity.UI;
using TMPro;
using UnityEngine;

namespace BU.QuizExample.Scripts.UI.QuizFlowUI.InstructorUI
{
    public class StateInstructorQuestion : BaseInstructorQuestion
    {
        [SerializeField]
        protected TMP_Text m_Chapter;

        [SerializeField]
        protected TMP_Text m_Mission;

        [SerializeField]
        protected TMP_Text m_QuizInfoText;

        [SerializeField]
        protected TMP_Text m_StudentAmountText;

        [SerializeField]
        protected MediaPanel m_MediaPanel;

        private const string ChapterIndexFormat = "Chapter: <color=white><font=\"EN_Stylize_Neutral_A\">{0}</font></color>";
        private const string MissionFormat = "Mission: <color=white><font=\"EN_Stylize_Neutral_B\">{0}</font></color>";
        private const string QuizInfoFormat = "<color=#14C287>{0}</color> / {1}";

        private void Start()
        {
            SetChapterText(Chapter);
            SetMissionText(Mission);
            SetQuizInfoText(CurrentPage, TotalPage);
            SetStudentAmountText(0, StudentAmount);
        }

        public override void SetQuestionImage(Texture texture)
        {
            base.SetQuestionImage(texture);

            m_MediaPanel.ShowImage(texture);
        }

        public override void SetQuestionVideo(string url)
        {
            base.SetQuestionVideo(url);

            m_MediaPanel.ShowVideo(url);
        }

        public override void OnStudentAnswerReceive(int studentAnswer, int studentAmount)
        {
            SetStudentAmountText(studentAnswer, studentAmount);
        }

        #region Custom functions

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

        private void SetStudentAmountText(int commitedStudent, int totalStudent)
        {
            m_StudentAmountText.text = string.Format("<color=#14C287>{0}</color> / {1}", commitedStudent, totalStudent);
        }

        #endregion
    }
}
