using BU.RRTT.QuizExample.Scripts.BossSystem;
using Notero.QuizConnector.Student;
using TMPro;
using UnityEngine;

namespace BU.RRTT.QuizExample.Scripts.UI.QuizFlowUI.StudentUI
{
    public class StateStudentWaiting : BaseStudentWaiting
    {
        [SerializeField]
        private TMP_Text m_Chapter;

        [SerializeField]
        private TMP_Text m_Mission;

        [SerializeField]
        private TMP_Text m_QuizInfoText;

        [SerializeField]
        private Animator m_WaitingCircleAnimator;

        private const string ChapterIndexFormat = "Chapter: <color=white><font=\"EN_Stylize_Neutral_A\">{0}</font></color>";
        private const string MissionFormat = "Mission: <color=white><font=\"EN_Stylize_Neutral_B\">{0}</font></color>";
        private const string QuizInfoFormat = "<color=#14C287>{0}</color> / {1}";
        
        // RRTT Variables
        [SerializeField]
        private Transform bossPosition;

        [SerializeField]
        private Transform countDownFrame;
        
        [SerializeField]
        private GameObject bossReference;

        private BossList bossList;

        private void Start()
        {
            SetChapterText(Chapter);
            SetMissionText(Mission);
            SetQuizInfoText(CurrentPage, TotalPage);
            TriggerWaitingCircleAnimation();
        }

        public override void OnCustomDataReceive(byte[] data)
        {
            bossList = bossReference.GetComponent<BossList>();
            GameObject boss = Instantiate(bossList.bossPrefabs[data[0]].gameObject, bossPosition);
            boss.transform.SetParent(countDownFrame);
            boss.transform.SetParent(bossPosition);
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

        private void TriggerWaitingCircleAnimation()
        {
            m_WaitingCircleAnimator.SetTrigger("WaitingCircle");
        }

        #endregion
    }
}