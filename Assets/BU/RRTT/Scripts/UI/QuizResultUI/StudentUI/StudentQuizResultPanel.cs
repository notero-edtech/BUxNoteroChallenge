
using BU.RRTT.Scripts.BossSystem;
using Notero.QuizConnector.Student;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BU.RRTT.Scripts.UI.QuizResultUI.StudentUI
{
    public class StudentQuizResultPanel : BaseStudentPopQuizResult
    {
        [SerializeField]
        protected TMP_Text m_ChapterIndexText;

        [SerializeField]
        protected TMP_Text m_MissionText;

        [SerializeField]
        protected TMP_Text m_QuizModeText;

        [SerializeField]
        protected TMP_Text m_QuestionAmountText;

        [SerializeField]
        protected TMP_Text m_QuizScoreText;

        private const string ChapterIndexFormat = "Chapter: <color=white><font=EN_Stylize_Neutral_A>{0}</font><color=#9F1A1A>";
        private const string MissionFormat = "Mission: <color=white><font=EN_Stylize_Neutral_B>{0}</font></color>";
        private const string QuizModeFormat = "Type: <color=white><font=EN_Stylize_Neutral_A>{0}</font></color>";
        private const string QuestionAmountFormat = "Amount of Questions: <color=white><font=EN_Stylize_Neutral_A>{0} Questions</font></color>";
        private const string ScoreColor = "#14C287";
        private const string QuizScoreFormat = "Your Quiz Score\n<color=" + ScoreColor + ">{0}</color>/{1}";
        
        // RRTT Variables
        [SerializeField]
        private Transform bossPosition;

        [SerializeField]
        private GameObject bossReference;

        private BossList bossList;
        
        private Vector3 scale = new Vector3( 4.5f,4.5f,4.5f);
        
        [SerializeField]
        private Image heartFiller;
        
        private float heart;
        
        private Animator animator;

        private void Start()
        {
            SetChapterText(Chapter);
            SetMissionText(Mission);
            SetQuizModeText(QuizMode);
            SetQuestionAmountText(QuestionAmount);
            SetQuizScoreText(CurrentScore, QuestionAmount);
            Debug.Log($"Questions : {QuestionAmount}");
            Debug.Log($"Heart : {heart}");
            if (heart < (0.5 * QuestionAmount))
            {
                Debug.Log("Negative!!!");
                animator.SetBool("ResultNeg", true);
            }
            if (heart >= (0.5 * QuestionAmount))
            {
                Debug.Log("Positive!!!");
                animator.SetBool("ResultPos", true);
            }
        }
        
        private void Update()
        {
            heartFiller.fillAmount = Mathf.MoveTowards(heartFiller.fillAmount, heart/QuestionAmount, 0.5f * Time.deltaTime);
        }

        public override void OnCustomDataReceive(byte[] data)
        {
            heart = data[1];
            //เขียนดักไว้เพราะว่าไม่มี Data สามารถลบได้เลยครับ ตอน Sync
            if (heart == null)
            {
                heart = 0;
            }
            //เขียนดักไว้เพราะว่าไม่มี Data สามารถลบได้เลยครับ ตอน Sync
            bossList = bossReference.GetComponent<BossList>();
            GameObject boss = Instantiate(bossList.bossPrefabs[data[0]].gameObject, bossPosition);
            animator = boss.GetComponent<Animator>();
            animator.SetBool("Positive", false);
            animator.SetBool("Negative", false);
            animator.SetBool("Question", false);
            boss.transform.localScale = scale;
            boss.transform.SetParent(bossPosition);
        }

        #region Custom Methods

        private void SetChapterText(string chapterText)
        {
            m_ChapterIndexText.text = string.Format(ChapterIndexFormat, chapterText);
        }

        private void SetMissionText(string mission)
        {
            m_MissionText.text = string.Format(MissionFormat, mission);
        }

        private void SetQuizModeText(string mode)
        {
            m_QuizModeText.text = string.Format(QuizModeFormat, mode);
        }

        private void SetQuestionAmountText(int questionAmount)
        {
            m_QuestionAmountText.text = string.Format(QuestionAmountFormat, questionAmount);
        }

        public virtual void SetQuizScoreText(int score, int questionAmount)
        {
            m_QuizScoreText.text = string.Format(QuizScoreFormat, score, questionAmount);
        }

        #endregion
    }
}