using Notero.QuizConnector.Student;
using TMPro;
using UnityEngine;

namespace BU.RRTT.QuizExample.Scripts.UI.QuizFlowUI.StudentUI
{
    public class StateStudentEnd : BaseStudentEnd
    {
        [SerializeField]
        private TMP_Text m_EndText;

        private void Start()
        {
            var endText = $"This is the end of quiz mode";

            SetEndText(endText);
        }

        public override void OnCustomDataReceive(byte[] data)
        {
            Debug.Log($"NPA-data:{data}");
        }

        #region Custom function

        private void SetEndText(string endText)
        {
            m_EndText.text = endText;
        }

        #endregion
    }
}