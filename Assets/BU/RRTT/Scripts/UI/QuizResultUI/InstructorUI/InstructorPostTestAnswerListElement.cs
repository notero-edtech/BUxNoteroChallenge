using Notero.QuizConnector.Instructor;
using Notero.Unity.UI;
using TMPro;
using UnityEngine;

namespace BU.RRTT.QuizExample.Scripts.UI.QuizResultUI.InstructorUI
{
    public class InstructorPostTestAnswerListElement : BaseAnswerListItem<Notero.QuizConnector.Model.PostTestAnswer>
    {
        [SerializeField]
        private TMP_Text m_AnswerInfoText;

        [SerializeField]
        private Button m_AnswerRevealButton;

        private const string AnswerInfoFormat = "Q. <font=EN_Stylize_Neutral_A><color=white><size=24>{0}  </size></color></font>Correct answer is <font=EN_Stylize_Positive><color=white><size=24>{1}  </size></color></font>";

        private void Start()
        {
            m_AnswerRevealButton.onClick.AddListener(OnAnswerRevealClick);
        }

        public override void OnCustomDataReceive(byte[] data)
        {
            Debug.Log($"NPA-data:{data}");
        }

        public override void SetElementInfo(Notero.QuizConnector.Model.PostTestAnswer info)
        {
            base.SetElementInfo(info);

            SetAnswerInfo(info);
        }

        #region Custom Methods

        private void OnAnswerRevealClick()
        {
            OnClicked?.Invoke(ElementInfo);
        }

        private void SetAnswerInfo(Notero.QuizConnector.Model.PostTestAnswer question)
        {
            m_AnswerInfoText.text = string.Format(AnswerInfoFormat, question.QuestionIndex + 1, question.QuestionAnswer);
        }

        #endregion
    }
}