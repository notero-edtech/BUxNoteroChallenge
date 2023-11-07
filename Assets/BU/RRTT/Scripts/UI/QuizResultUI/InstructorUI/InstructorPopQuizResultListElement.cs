using Notero.QuizConnector.Instructor;
using Notero.QuizConnector.Model;
using TMPro;
using UnityEngine;

namespace BU.RRTT.QuizExample.Scripts.UI.QuizResultUI.InstructorUI
{
    public class InstructorPopQuizResultListElement : BaseStudentQuizResultListItem<StudentQuizResultInfo>
    {
        [SerializeField]
        private TMP_Text m_PlayerInfoText;

        [SerializeField]
        private TMP_Text m_AnswerSummaryText;

        private const string PlayerInfoFormat = "Seat <color=#F68800>{0}</color>";

        private const string AnswerSummaryFormat = "<color=#14C287>{0}</color> / {1}";

        public override void SetElementInfo(StudentQuizResultInfo info)
        {
            base.SetElementInfo(info);

            SetSeatNumber(info.StationId);
            SetCorrentAnswerAmount(info.CurrentScore, info.FullScore);
        }

        public override void OnCustomDataReceive(byte[] data)
        {
            Debug.Log($"NPA-data:{data}");
        }

        #region Custom methods

        private void SetSeatNumber(string stationId)
        {
            var seatNumber = ValidatePositiveStringToInt(stationId);

            m_PlayerInfoText.text = string.Format(PlayerInfoFormat, NumberToZeroFrontString(seatNumber));
        }

        private void SetCorrentAnswerAmount(int correctAnswerAmount, int questionAmount)
        {
            m_AnswerSummaryText.text = string.Format(AnswerSummaryFormat, correctAnswerAmount, questionAmount);
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