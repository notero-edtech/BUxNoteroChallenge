using BU.RRTT.QuizExample.Scripts.BossSystem;
using Notero.QuizConnector.Instructor;
using Notero.Unity.UI.Quiz;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BU.RRTT.QuizExample.Scripts.UI.QuizFlowUI.InstructorUI
{
    public class StateInstructorPreResult : BaseInstructorPreResult
    {
        [SerializeField]
        protected TMP_Text m_Chapter;

        [SerializeField]
        protected TMP_Text m_Mission;

        [SerializeField]
        protected TMP_Text m_QuizInfoText;

        [SerializeField]
        private HUDController m_NextButtonUI;

        [SerializeField]
        public RawImage m_QuestionImage;

        [SerializeField]
        public ChoiceUI m_Choice1;

        [SerializeField]
        public ChoiceUI m_Choice2;

        [SerializeField]
        public ChoiceUI m_Choice3;

        [SerializeField]
        public ChoiceUI m_Choice4;

        [SerializeField]
        public ChartUI m_ChartBar1;

        [SerializeField]
        public ChartUI m_ChartBar2;

        [SerializeField]
        public ChartUI m_ChartBar3;

        [SerializeField]
        public ChartUI m_ChartBar4;

        [SerializeField]
        public TMP_Text m_CorrectAnswerText;

        [SerializeField]
        public TMP_Text m_CorrectAnswerAmountText;

        [SerializeField]
        public TMP_Text m_WrongAnswerAmountText;

        [SerializeField]
        public TMP_Text m_NotAnswerAmountText;

        [SerializeField]
        public TMP_Text m_StudentAnswerAmountText;

        private const string ChapterIndexFormat = "Chapter: <color=white><font=\"EN_Stylize_Neutral_A\">{0}</font></color>";
        private const string MissionFormat = "Mission: <color=white><font=\"EN_Stylize_Neutral_B\">{0}</font></color>";
        private const string QuizInfoFormat = "<color=#14C287>{0}</color> / {1}";
        private const string StudentAnswerAmountFormat = "<color=#14C287>{0}</color> / {1}";
        
        // RRTT Variables
        [SerializeField]
        private Transform bossPosition;

        [SerializeField]
        private Transform contentFrame;
        
        [SerializeField]
        private GameObject bossReference;

        private BossList bossList;

        private Vector3 scale = new Vector3( 8.35f,8.35f,8.35f);
        
        private void Start()
        {
            SetChapterText(Chapter);
            SetMissionText(Mission);
            SetQuizInfoText(CurrentPage, TotalPage);
            SetQuestionImage(QuestionImage);
            SetPreResultUI();

            m_NextButtonUI.OnNextClick.AddListener(OnNextStateReceive);
        }

        public override void OnCustomDataReceive(byte[] data)
        {
            bossList = bossReference.GetComponent<BossList>();
            GameObject boss = Instantiate(bossList.bossPrefabs[data[0]].gameObject, bossPosition);
            boss.transform.localScale = scale;
            boss.transform.SetParent(contentFrame);
            boss.transform.SetParent(bossPosition);
        }

        #region Custom function

        private void OnNextStateReceive()
        {
            OnNextState?.Invoke();
        }

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

        private void SetCorrectAnswerAmountText(string text)
        {
            m_CorrectAnswerAmountText.text = text;
        }

        private void SetWrongAnswerAmountText(string text)
        {
            m_WrongAnswerAmountText.text = text;
        }

        private void SetNotAnswerAmountText(string text)
        {
            m_NotAnswerAmountText.text = text;
        }

        private void SetStudentAnswerAmountText(int studentAnswer, int studenAmount)
        {
            m_StudentAnswerAmountText.text = string.Format(StudentAnswerAmountFormat, studentAnswer, studenAmount);
        }

        private void SetChartBar(float choice1, float choice2, float choice3, float choice4)
        {
            m_ChartBar1.SetChartBar(choice1);
            m_ChartBar2.SetChartBar(choice2);
            m_ChartBar3.SetChartBar(choice3);
            m_ChartBar4.SetChartBar(choice4);
        }

        private void SetAnswerAmount(int choice1, int choice2, int choice3, int choice4)
        {
            m_ChartBar1.SetAnswerAmount(choice1);
            m_ChartBar2.SetAnswerAmount(choice2);
            m_ChartBar3.SetAnswerAmount(choice3);
            m_ChartBar4.SetAnswerAmount(choice4);
        }

        private void SetChartBarState(bool choice1, bool choice2, bool choice3, bool choice4)
        {
            m_ChartBar1.SetIsCorrectCharBar(choice1);
            m_ChartBar2.SetIsCorrectCharBar(choice2);
            m_ChartBar3.SetIsCorrectCharBar(choice3);
            m_ChartBar4.SetIsCorrectCharBar(choice4);
        }

        private void SetChoiceState(bool choice1, bool choice2, bool choice3, bool choice4)
        {
            m_Choice1.SetIsCorrectChoice(choice1);
            m_Choice2.SetIsCorrectChoice(choice2);
            m_Choice3.SetIsCorrectChoice(choice3);
            m_Choice4.SetIsCorrectChoice(choice4);
        }

        private void SetPreResultUI()
        {
            var studentAmount = StudentAmount;
            var choiceAmount1 = AnswerSummaryDic.TryGetValue("1", out var choiceAmount1Value) ? choiceAmount1Value : 0;
            var choiceAmount2 = AnswerSummaryDic.TryGetValue("2", out var choiceAmount2Value) ? choiceAmount2Value : 0;
            var choiceAmount3 = AnswerSummaryDic.TryGetValue("3", out var choiceAmount3Value) ? choiceAmount3Value : 0;
            var choiceAmount4 = AnswerSummaryDic.TryGetValue("4", out var choiceAmount4Value) ? choiceAmount4Value : 0;

            var chartBarChoice1 = choiceAmount1 == 0 ? 0 : choiceAmount1 / (float)AnswerAmount * 100;
            var chartBarChoice2 = choiceAmount2 == 0 ? 0 : choiceAmount2 / (float)AnswerAmount * 100;
            var chartBarChoice3 = choiceAmount3 == 0 ? 0 : choiceAmount3 / (float)AnswerAmount * 100;
            var chartBarChoice4 = choiceAmount4 == 0 ? 0 : choiceAmount4 / (float)AnswerAmount * 100;

            var IsCorrectAnswer1 = CorrectAnswer == "1";
            var IsCorrectAnswer2 = CorrectAnswer == "2";
            var IsCorrectAnswer3 = CorrectAnswer == "3";
            var IsCorrectAnswer4 = CorrectAnswer == "4";

            var answerCorrectAmount = AnswerSummaryDic.TryGetValue(CorrectAnswer, out var amount) ? amount : 0;
            var notAnswerAmount = studentAmount;

            var answerWrongAmount = AnswerAmount - answerCorrectAmount;
            notAnswerAmount -= answerCorrectAmount + answerWrongAmount;

            SetChartBarState(IsCorrectAnswer1, IsCorrectAnswer2, IsCorrectAnswer3, IsCorrectAnswer4);
            SetChoiceState(IsCorrectAnswer1, IsCorrectAnswer2, IsCorrectAnswer3, IsCorrectAnswer4);
            SetChartBar(chartBarChoice1, chartBarChoice2, chartBarChoice3, chartBarChoice4);
            SetAnswerAmount(choiceAmount1, choiceAmount2, choiceAmount3, choiceAmount4);

            SetCorrectAnswerText(CorrectAnswer);
            SetCorrectAnswerAmountText(answerCorrectAmount.ToString());
            SetWrongAnswerAmountText(answerWrongAmount.ToString());
            SetNotAnswerAmountText(notAnswerAmount.ToString());
            SetStudentAnswerAmountText(AnswerAmount, studentAmount);
        }

        #endregion
    }
}