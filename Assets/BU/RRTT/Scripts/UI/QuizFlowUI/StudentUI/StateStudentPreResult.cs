using Notero.QuizConnector.Student;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BU.RRTT.QuizExample.Scripts.UI.QuizFlowUI.StudentUI
{
    public class StateStudentPreResult : BaseStudentPreResult
    {
        [SerializeField]
        protected TMP_Text m_Chapter;

        [SerializeField]
        protected TMP_Text m_Mission;

        [SerializeField]
        protected TMP_Text m_QuizInfoText;

        [SerializeField]
        public RawImage m_QuestionImage;

        [SerializeField]
        public TMP_Text m_CorrectAnswerText;

        [SerializeField]
        private GameObject m_CorrectAnswer;

        [SerializeField]
        private GameObject m_WrongAnswer;

        private const string ChapterIndexFormat = "Chapter: <color=white><font=\"EN_Stylize_Neutral_A\">{0}</font></color>";
        private const string MissionFormat = "Mission: <color=white><font=\"EN_Stylize_Neutral_B\">{0}</font></color>";
        private const string QuizInfoFormat = "<color=#14C287>{0}</color> / {1}";

        private string m_Answer;

        private void Start()
        {
            SetChapterText(Chapter);
            SetMissionText(Mission);
            SetQuizInfoText(CurrentPage, TotalPage);
            SetQuestionImage(QuestionImage);
            SetCorrectAnswerText(CorrectAnswer);
            SetIsCorrectAnswer(Answer == CorrectAnswer);
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

        private void SetQuestionImage(Texture texture)
        {
            m_QuestionImage.texture = texture;
        }

        private void SetCorrectAnswerText(string text)
        {
            m_CorrectAnswerText.text = text;
        }

        private void SetIsCorrectAnswer(bool isCorrectAnswer)
        {
            m_CorrectAnswer.SetActive(isCorrectAnswer);
            m_WrongAnswer.SetActive(!isCorrectAnswer);
        }

        #endregion
    }
}