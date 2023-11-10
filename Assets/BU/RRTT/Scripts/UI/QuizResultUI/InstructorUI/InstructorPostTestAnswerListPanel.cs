using System.Collections.Generic;
using Notero.QuizConnector.Instructor;
using Notero.Unity.UI;
using TMPro;
using UnityEngine;

namespace BU.RRTT.QuizExample.Scripts.UI.QuizResultUI.InstructorUI
{
    public class InstructorPostTestAnswerListPanel : BaseInstructorPostTestAnswerListResult<Notero.QuizConnector.Model.PostTestAnswer>
    {
        [SerializeField]
        private TMP_Text m_Chapter;

        [SerializeField]
        private TMP_Text m_Mission;

        [SerializeField]
        private TMP_Text m_QuizModeText;

        [SerializeField]
        private TMP_Text m_QuestionAmountText;

        [SerializeField]
        private Button m_SwapResultButton;

        [SerializeField]
        protected BaseAnswerListItem<Notero.QuizConnector.Model.PostTestAnswer> m_ListItemPrototype;

        [SerializeField]
        protected Transform m_ListElementContainer;

        private const string ChapterIndexFormat = "Chapter: <color=white><font=\"EN_Stylize_Neutral_A\">{0}</font></color>";
        private const string MissionFormat = "Mission: <color=white><font=\"EN_Stylize_Neutral_B\">{0}</font></color>";
        private const string QuestionAmountFormat = "Amount of Question: <size=24><color=white><font=EN_Stylize_Neutral_A>{0} Questions</font></color></size>";

        private void Start()
        {
            SetChapterText(Chapter);
            SetMissionText(Mission);
            SetQuizModeText("Post Test");
            SetQuestionAmountText(TotalPage);

            if(m_SwapResultButton != null) m_SwapResultButton.onClick.AddListener(OnSwapResult);
        }

        private void OnDestroy()
        {
            if(m_SwapResultButton != null) m_SwapResultButton.onClick.RemoveListener(OnSwapResult);
            RemoveListenerFromListElement();
        }

        public override void OnCustomDataReceive(byte[] data)
        {
            Debug.Log($"NPA-data:{data}");
        }

        public override void SetElementListInfo(List<Notero.QuizConnector.Model.PostTestAnswer> list)
        {
            base.SetElementListInfo(list);
            SetElementList(list);
        }

        #region Custom Methods

        private void OnSwapResult()
        {
            OnSwapResultState?.Invoke(QuizMode);
        }

        private void OnShowAnswerRevealClick(Notero.QuizConnector.Model.PostTestAnswer info)
        {
            OnClicked?.Invoke(info);
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

        private void SetQuestionAmountText(int questionAmount)
        {
            m_QuestionAmountText.text = string.Format(QuestionAmountFormat, questionAmount);
        }

        private void SetElementList(List<Notero.QuizConnector.Model.PostTestAnswer> elementInfos)
        {
            if(elementInfos.Count <= 0)
            {
                SetToBlankElementList();
                return;
            }

            foreach(var info in elementInfos)
            {
                BaseAnswerListItem<Notero.QuizConnector.Model.PostTestAnswer> element = Instantiate(m_ListItemPrototype, m_ListElementContainer);
                element.SetElementInfo(info);
                element.OnClicked.AddListener(OnShowAnswerRevealClick);
            }
        }

        private void RemoveListenerFromListElement()
        {
            foreach(Transform element in m_ListElementContainer)
            {
                element.GetComponent<BaseAnswerListItem<Notero.QuizConnector.Model.PostTestAnswer>>().OnClicked.RemoveListener(OnShowAnswerRevealClick);
            }
        }

        private void SetToBlankElementList()
        {
            foreach(Transform element in m_ListElementContainer) Destroy(element.gameObject);
        }

        #endregion
    }
}