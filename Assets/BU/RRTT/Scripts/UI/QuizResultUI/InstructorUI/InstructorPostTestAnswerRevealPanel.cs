using Notero.QuizConnector.Instructor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = Notero.Unity.UI.Button;

namespace BU.RRTT.QuizExample.Scripts.UI.QuizResultUI.InstructorUI
{
    public class InstructorPostTestAnswerRevealPanel : BaseInstructorPostTestAnswerRevealResult
    {
        [SerializeField]
        private TMP_Text m_Chapter;

        [SerializeField]
        private TMP_Text m_Mission;

        [SerializeField]
        private TMP_Text m_QuizModeText;

        [SerializeField]
        private TMP_Text m_CorrectAnswerText;

        [SerializeField]
        private TMP_Text m_QuestionIndexText;

        [SerializeField]
        private RawImage m_QuestionImage;

        [SerializeField]
        private Button m_BackToAnswerListButton;

        [SerializeField]
        private Button m_NextAnswerButton;

        [SerializeField]
        private Button m_PreviousAnswerButton;

        [SerializeField]
        private Button m_SwapResultButton;

        private const string ChapterIndexFormat = "Chapter: <color=white><font=\"EN_Stylize_Neutral_A\">{0}</font></color>";
        private const string MissionFormat = "Mission: <color=white><font=\"EN_Stylize_Neutral_B\">{0}</font></color>";
        private const string CorrectAnswerFormat = "Correct Answer: <size=18><color=white><font=EN_Stylize_Neutral_A>{0}</font></color></size>";
        private const string QuestionIndexFormat = "<color=#266866>{0}</color> / {1}";

        private void Start()
        {
            SetChapterText(Chapter);
            SetMissionText(Mission);
            SetQuestionImage(QuestionImage);
            SetCorrectAnswerText(CorrectAnswer);
            SetQuestionIndexText(CurrentQuestionIndex + 1, TotalPage);
            SetQuizModeText("Post Test");

            m_SwapResultButton.onClick.AddListener(OnSwapResult);
            m_BackToAnswerListButton.onClick.AddListener(OnShowAnswerListClick);
            m_NextAnswerButton.onClick.AddListener(OnNextQuestionClick);
            m_PreviousAnswerButton.onClick.AddListener(OnPreviousQuestionClick);
        }

        private void OnDestroy()
        {
            m_SwapResultButton.onClick.RemoveListener(OnSwapResult);
            m_BackToAnswerListButton.onClick.RemoveListener(OnShowAnswerListClick);
            m_NextAnswerButton.onClick.RemoveListener(OnNextQuestionClick);
            m_PreviousAnswerButton.onClick.RemoveListener(OnPreviousQuestionClick);
        }

        public override void OnCustomDataReceive(byte[] data)
        {
            Debug.Log($"NPA-data:{data}");
        }

        public override void SetQuestionTexture(Texture texture)
        {
            base.SetQuestionTexture(texture);
            SetQuestionImage(QuestionImage);
        }

        public override void SetCurrentQuestionIndex(int number)
        {
            base.SetCurrentQuestionIndex(number);
            SetQuestionIndexText(number + 1, TotalPage);
        }

        public override void SetCorrectAnswer(string correctAnswer)
        {
            base.SetCorrectAnswer(correctAnswer);
            SetCorrectAnswerText(correctAnswer);
        }

        #region Custom Methods

        private void OnSwapResult()
        {
            OnSwapResultState?.Invoke(QuizMode);
        }

        private void OnShowAnswerListClick()
        {
            OnShowAnswerList?.Invoke(QuizMode);
        }

        private void OnNextQuestionClick()
        {
            OnNextAnswerReveal?.Invoke(CurrentQuestionIndex + 1);
        }

        private void OnPreviousQuestionClick()
        {
            OnPreviousAnswerReveal?.Invoke(CurrentQuestionIndex - 1);
        }

        private void SetChapterText(string text)
        {
            m_Chapter.text = string.Format(ChapterIndexFormat, text);
        }

        private void SetMissionText(string text)
        {
            m_Mission.text = string.Format(MissionFormat, text);
        }

        private void SetQuizModeText(string quizMode)
        {
            m_QuizModeText.text = quizMode;
        }

        private void SetCorrectAnswerText(string correctAnswer)
        {
            m_CorrectAnswerText.text = string.Format(CorrectAnswerFormat, correctAnswer);
        }

        private void SetQuestionIndexText(int currentQuestion, int totalQuestion)
        {
            m_QuestionIndexText.text = string.Format(QuestionIndexFormat, currentQuestion, totalQuestion);
        }

        private void SetQuestionImage(Texture texture)
        {
            m_QuestionImage.texture = texture;
        }

        #endregion
    }
}