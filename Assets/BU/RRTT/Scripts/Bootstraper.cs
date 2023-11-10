using Notero.Unity.UI;
using Notero.Utilities;
using UnityEngine;

namespace BU.RRTT.QuizExample.Scripts
{
    public class Bootstraper : MonoBehaviour
    {
        [Header("Asset for test scene")]
        [SerializeField]
        private Button m_PopQuizStartButton;

        [SerializeField]
        private GameObject m_PopQuizOption;

        [SerializeField]
        private Button m_PreTestButton;

        [SerializeField]
        private GameObject m_PreTestOption;

        [SerializeField]
        private GameObject m_LoadingBG;

        [SerializeField]
        private TransformableButton m_HideCurrentPanelButton;

        [SerializeField]
        private QuizControllerType m_QuizControllerType;

        [SerializeField]
        private TextAsset m_QuizDataJSON;

        private void Awake()
        {
            ApplicationFlagConfig.InitializeCommandlineArgs();
            if(ApplicationFlagConfig.IsInstructorMode) FakeForInstructorController.Instance.Init(m_QuizControllerType, m_QuizDataJSON.text);
            if(ApplicationFlagConfig.IsStudentMode) FakeForStudentController.Instance.Init("1", m_QuizControllerType, m_QuizDataJSON.text);

            m_PopQuizStartButton.onClick.AddListener(OnPopQuizStartButtonClick);
            m_PreTestButton.onClick.AddListener(OnPostTestStartClick);

            if(m_HideCurrentPanelButton != null)
            {
                SetActiveHideCurrentUIButton(false);
                m_HideCurrentPanelButton.onClick.AddListener(OnHideCurrentPanelClick);
            }
        }

        private void SetActiveLoadingBG(bool isActive) => m_LoadingBG.SetActive(isActive);

        private void SetActivePopQuizOption(bool isActive) => m_PopQuizOption.SetActive(isActive);

        private void SetActivePreTestOption(bool isActive) => m_PreTestOption.SetActive(isActive);

        private void SetActiveHideCurrentUIButton(bool isActive) => m_HideCurrentPanelButton.gameObject.SetActive(isActive);

        private void OnPopQuizStartButtonClick()
        {
            SetActiveLoadingBG(false);
            SetActivePopQuizOption(false);
            SetActivePreTestOption(false);

            if(m_HideCurrentPanelButton != null) SetActiveHideCurrentUIButton(true);

            if(ApplicationFlagConfig.IsInstructorMode) FakeForInstructorController.Instance.SetQuizMode(QuizModes.POPQUIZ);
            if(ApplicationFlagConfig.IsStudentMode) FakeForStudentController.Instance.SetQuizMode(QuizModes.POPQUIZ);

            StartQuiz();
        }

        private void OnPostTestStartClick()
        {
            SetActiveLoadingBG(false);
            SetActivePopQuizOption(false);
            SetActivePreTestOption(false);

            if(m_HideCurrentPanelButton != null) SetActiveHideCurrentUIButton(true);

            if(ApplicationFlagConfig.IsInstructorMode) FakeForInstructorController.Instance.SetQuizMode(QuizModes.POSTTEST);
            if(ApplicationFlagConfig.IsStudentMode) FakeForStudentController.Instance.SetQuizMode(QuizModes.POSTTEST);

            StartQuiz();
        }

        private void OnHideCurrentPanelClick()
        {
            SetActiveLoadingBG(true);
            SetActivePopQuizOption(true);
            SetActivePreTestOption(true);
            SetActiveHideCurrentUIButton(false);

            if(ApplicationFlagConfig.IsInstructorMode) FakeForInstructorController.Instance.DestroyCurrentUI();
            if(ApplicationFlagConfig.IsStudentMode) FakeForStudentController.Instance.DestroyCurrentUI();
        }

        private void StartQuiz()
        {
            SetActivePopQuizOption(false);
            SetActivePreTestOption(false);

            if(ApplicationFlagConfig.IsInstructorMode) FakeForInstructorController.Instance.StarterState();
            if(ApplicationFlagConfig.IsStudentMode) FakeForStudentController.Instance.StarterState();
        }
    }
}