using Notero.QuizConnector.Student;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BU.RRTT.QuizExample.Scripts.UI.QuizFlowUI.StudentUI
{
    public class StateStudentQuestion : BaseStudentQuestion
    {
        [SerializeField]
        protected TMP_Text m_Chapter;

        [SerializeField]
        protected TMP_Text m_Mission;

        [SerializeField]
        protected TMP_Text m_QuizInfoText;

        [SerializeField]
        protected RawImage m_QuestionRawImage;

        private const string ChapterIndexFormat = "Chapter: <color=white><font=\"EN_Stylize_Neutral_A\">{0}</font></color>";
        private const string MissionFormat = "Mission: <color=white><font=\"EN_Stylize_Neutral_B\">{0}</font></color>";
        private const string QuizInfoFormat = "<color=#14C287>{0}</color> / {1}";

        private void Start()
        {
            SetChapterText(Chapter);
            SetMissionText(Mission);
            SetQuizInfoText(CurrentPage, TotalPage);
            SetQuestionImage(QuestionImage);
        }

        public override void OnCustomDataReceive(byte[] data)
        {
            Debug.Log($"NPA-data:{data}");
        }

        public override void OnPianoStateReceive(PianoStates pianoState, string choice)
        {
            base.OnPianoStateReceive(pianoState, choice);

            if(pianoState != PianoStates.PIANO_SUBMITTED) return;
            // Run animation

            // Run animation finish
            OnNextState?.Invoke();
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

        private void SetQuestionImage(Texture texture)
        {
            m_QuestionRawImage.texture = texture;
        }

        #endregion
    }
}