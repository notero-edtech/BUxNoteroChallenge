using Notero.QuizConnector.Instructor;
using Notero.QuizConnector.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BU.RRTT.QuizExample.Scripts.UI.QuizResultUI.InstructorUI
{
    public class InstructorPostTestStudentListElement : BasePostTestStudentListItem<StudentPostTestResultInfo>
    {
        [SerializeField]
        private TMP_Text m_PlayerInfoText;

        [SerializeField]
        private TMP_Text m_PreTestAnswerSummaryText;

        [SerializeField]
        private TMP_Text m_PostTestAnswerSummaryText;

        [Header("Compare Icon")]
        [SerializeField]
        private RawImage m_CompareIcon;

        [SerializeField]
        private Texture m_EqualIcon;

        [SerializeField]
        private Texture m_LessIcon;

        [SerializeField]
        private Texture m_MoreIcon;

        private const string AnswerDefaultColor = "#14C287";
        private const string AnswerLessColor = "#EC0F47";

        private const string PlayerInfoFormat = "Seat <color=#F68800>{0}</color>";

        private const string AnswerSummaryFormat = "<color={0}>{1}</color> / {2}";

        public override void OnCustomDataReceive(byte[] data)
        {
            Debug.Log($"NPA-data:{data}");
        }

        public override void SetElementInfo(StudentPostTestResultInfo info)
        {
            base.SetElementInfo(info);

            var preTestScoreText = info.PreviousScore == -1 ? "-" : info.PreviousScore.ToString();

            SetSeatNumber(info.StationId);
            SetCompareIcon(info.PreviousScore, info.CurrentScore);
            SetCorrectAnswerAmount(preTestScoreText, info.CurrentScore, info.PreviousFullScore, info.CurrentFullScore);
        }

        #region Custom Methods

        private void SetCompareIcon(int preTestScore, int postScore)
        {
            if(preTestScore > postScore)
            {
                m_CompareIcon.texture = m_LessIcon;
            }
            else if(preTestScore < postScore)
            {
                m_CompareIcon.texture = m_MoreIcon;
            }
            else
            {
                m_CompareIcon.texture = m_EqualIcon;
            }
        }

        private void SetCorrectAnswerAmount(string preCorrectAnswerAmount, int postCorrectAnswerAmount, int preTestFullScore, int postTestFullScore)
        {
            if(int.TryParse(preCorrectAnswerAmount, out var preTestNumber))
            {
                var postTestScoreText = preTestNumber > postCorrectAnswerAmount ? string.Format(AnswerSummaryFormat, AnswerLessColor, postCorrectAnswerAmount, postTestFullScore) : string.Format(AnswerSummaryFormat, AnswerDefaultColor, postCorrectAnswerAmount, postTestFullScore);
                m_PostTestAnswerSummaryText.text = postTestScoreText;
            }
            else
            {
                m_PostTestAnswerSummaryText.text = string.Format(AnswerSummaryFormat, AnswerDefaultColor, postCorrectAnswerAmount, postTestFullScore);
            }

            m_PreTestAnswerSummaryText.text = string.Format(AnswerSummaryFormat, AnswerDefaultColor, preCorrectAnswerAmount, preTestFullScore);
        }

        private void SetSeatNumber(string stationId)
        {
            var seatNumber = ValidatePositiveStringToInt(stationId);

            m_PlayerInfoText.text = string.Format(PlayerInfoFormat, NumberToZeroFrontString(seatNumber));
        }

        private static int ValidatePositiveStringToInt(string info)
        {
            if(!int.TryParse(info, out int num))
            {
                num = -1;
            }

            return num;
        }

        private static string NumberToZeroFrontString(int num, int expectedDigits = 2)
        {
            if(num <= 0)
            {
                return "None";
            }

            string numStr = num.ToString();
            string zero = new string('0', expectedDigits - numStr.Length);

            return $"{zero}{numStr}";
        }

        #endregion
    }
}