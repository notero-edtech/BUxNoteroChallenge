using System.Collections.Generic;
using Notero.QuizConnector.Instructor;
using Notero.QuizConnector.Model;
using TMPro;
using UnityEngine;

namespace BU.RRTT.QuizExample.Scripts.UI.QuizResultUI.InstructorUI
{
    public class InstructorPopQuizResultPanel : BaseInstructorPopQuizResult<StudentQuizResultInfo>
    {
        [Header("Panel Text")]
        [SerializeField]
        private TMP_Text m_Chapter;

        [SerializeField]
        private TMP_Text m_Mission;

        [SerializeField]
        private TMP_Text m_QuizModeText;

        [SerializeField]
        private TMP_Text m_NoStudentText;

        [Header("Panel List Element")]
        [SerializeField]
        protected BaseStudentQuizResultListItem<StudentQuizResultInfo> m_ListItemPrototype;

        [SerializeField]
        protected Transform m_ListElementContainer;

        private const string ChapterIndexFormat = "Chapter: <color=white><font=\"EN_Stylize_Neutral_A\">{0}</font></color>";
        private const string MissionFormat = "Mission: <color=white><font=\"EN_Stylize_Neutral_B\">{0}</font></color>";

        private void Start()
        {
            SetChapterText(Chapter);
            SetMissionText(Mission);
            SetQuizModeText("Pop Quiz");
        }

        public override void OnCustomDataReceive(byte[] data)
        {
            Debug.Log($"NPA-data:{data}");
        }

        public override void SetElementListInfo(List<StudentQuizResultInfo> list)
        {
            base.SetElementListInfo(list);

            if(list == null || list.Count == 0)
            {
                SetNoStudentText(true);
                SetToBlankElementList();
            }
            else
            {
                SetNoStudentText(false);
                SetElementList(list);
            }
        }

        #region Custom Methods

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

        private void SetNoStudentText(bool isActive)
        {
            m_NoStudentText.gameObject.SetActive(isActive);
        }

        private void SetElementList(List<StudentQuizResultInfo> elementInfos)
        {
            if(elementInfos.Count <= 0)
            {
                SetToBlankElementList();
                return;
            }

            foreach(var info in elementInfos)
            {
                BaseStudentQuizResultListItem<StudentQuizResultInfo> element = Instantiate(m_ListItemPrototype, m_ListElementContainer);
                element.SetElementInfo(info);
            }
        }

        private void SetToBlankElementList()
        {
            foreach(Transform element in m_ListElementContainer) Destroy(element.gameObject);
        }

        #endregion
    }
}