using Notero.QuizConnector.Instructor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BU.QuizExample.Scripts.UI.QuizFlowUI.InstructorUI
{
    public class StateInstructorQuestion : BaseInstructorQuestion
    {
        [SerializeField]
        protected TMP_Text m_Chapter;

        [SerializeField]
        protected TMP_Text m_Mission;

        [SerializeField]
        protected TMP_Text m_QuizInfoText;

        [SerializeField]
        protected TMP_Text m_StudentAmountText;

        [SerializeField]
        protected RawImage m_QuestionRawImage;

        [SerializeField]
        private HUDController m_NextButtonUI;
        
        // RRTT Variables
        [SerializeField]
        private Transform bossPosition;
        
        [SerializeField]
        private Transform ContentFrame;
        
        private GameObject bossReference;

        private BossList bossList;
        

        private const string ChapterIndexFormat = "Chapter: <color=white><font=\"EN_Stylize_Neutral_A\">{0}</font></color>";
        private const string MissionFormat = "Mission: <color=white><font=\"EN_Stylize_Neutral_B\">{0}</font></color>";
        private const string QuizInfoFormat = "<color=#14C287>{0}</color> / {1}";

        private void Start()
        {
            SetChapterText(Chapter);
            SetMissionText(Mission);
            SetQuizInfoText(CurrentPage, TotalPage);
            SetQuestionImage(QuestionImage);
            SetStudentAmountText(0, StudentAmount);

            m_NextButtonUI.OnNextClick.AddListener(OnNextStateReceive);
        }

        public override void OnCustomDataReceive(byte[] data)
        {
            Debug.Log($"NPA-data:{data[0]}");
            // RRTT Additional
            bossReference = GameObject.FindGameObjectWithTag("BossReference");
            bossList = bossReference.GetComponent<BossList>();
            GameObject boss = Instantiate(bossList.bossPrefabs[data[0]].gameObject, bossPosition);
            boss.transform.localScale = new Vector3(4, 4, 4);
            boss.transform.SetParent(ContentFrame);
            boss.transform.SetParent(bossPosition);
            if (boss != null)
            {
                Debug.Log("Spawn!");
            }
        }

        public override void OnStudentAnswerReceive(int studentAnswer, int studentAmount)
        {
            SetStudentAmountText(studentAnswer, studentAmount);
        }

        #region Custom functions

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

        private void SetStudentAmountText(int commitedStudent, int totalStudent)
        {
            m_StudentAmountText.text = string.Format("<color=#14C287>{0}</color> / {1}", commitedStudent, totalStudent);
        }

        private void SetQuestionImage(Texture texture)
        {
            m_QuestionRawImage.texture = texture;
        }

        #endregion
    }
}