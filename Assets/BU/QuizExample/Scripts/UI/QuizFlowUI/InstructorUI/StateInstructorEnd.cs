using Notero.QuizConnector.Instructor;
using Notero.Unity.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BU.QuizExample.Scripts.UI.QuizFlowUI.InstructorUI
{
    public class StateInstructorEnd : BaseInstructorEnd
    {
        [SerializeField]
        private TMP_Text m_EndText;

        [SerializeField]
        private Button m_RestartButton;

        private void Awake()
        {
            m_RestartButton.onClick.AddListener(OnRestartClick);
        }

        private void OnDestroy()
        {
            m_RestartButton.onClick.RemoveListener(OnRestartClick);
        }

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

        private void OnRestartClick()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        #endregion
    }
}