using DataStore.Quiz;
using Newtonsoft.Json;
using Notero.QuizConnector;
using Notero.QuizConnector.Instructor;
using Notero.QuizConnector.Model;
using Notero.QuizConnector.Student;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace BU.QuizExample.Scripts
{
    public class QuizResultExampleController : MonoBehaviour, IQuizController
    {
        [Header("Instructor Prefabs")]
        [SerializeField]
        private GameObject m_InstructorPopQuizResultPrefab;

        [SerializeField]
        private GameObject m_InstructorPostTestStudentListResultPrefab;

        [SerializeField]
        private GameObject m_InstructorPostTestAnswerListResultPrefab;

        [SerializeField]
        private GameObject m_InstructorPostTestAnswerRevealResultPrefab;

        [Header("Student Prefabs")]
        [SerializeField]
        private GameObject m_StudentPopQuizResultPrefab;

        [SerializeField]
        private GameObject m_StudentPostTestResultPrefab;

        #region Instructor base classes

        private BaseInstructorPopQuizResult<StudentQuizResultInfo> m_InstructorPopQuizResult;
        private BaseInstructorPostTestStudentListResult<StudentPostTestResultInfo> m_InstructorPostTestStudentListResult;
        private BaseInstructorPostTestAnswerListResult<PostTestAnswer> m_InstructorPostTestAnswerListResult;
        private BaseInstructorPostTestAnswerRevealResult m_InstructorPostTestAnswerRevealResult;

        #endregion

        #region Student base classes

        private BaseStudentPopQuizResult m_StudentPopQuizResult;
        private BaseStudentPostTestResult m_StudentPostTestResult;

        #endregion

        // Container
        private Transform m_Container;

        public bool IsQuizLoaded { get; set; }

        public QuizStore QuizStore { get; set; }

        public UnityEvent OnNextStateReceive { get; set; }

        public UnityEvent<string> OnStudentSubmit { get; set; }

        public UnityEvent<byte[]> OnCustomDataReceive { get; set; }

        private string QuizMode;

        public void Init(Transform container, QuizStore quizStore)
        {
            m_Container = container;
            QuizStore = quizStore;
        }

        private T SpawnQuizPrototype<T>(GameObject gameObj) where T : BaseQuizPanel
        {
            var prototype = Instantiate(gameObj, m_Container);

            QuizConnectorController.Instance.SetCurrentUI(prototype);
            prototype.GetComponent<BaseQuizPanel>().OnCustomDataReceive(QuizStore.CustomData);

            return prototype.GetComponent(typeof(T)) as T;
        }

        private static Texture GenerateQuestionTexture(string path)
        {
            var imagePath = path.Split(".")[0].Trim();
            var texturePath = Path.Combine(imagePath);
            var resource = Resources.Load<Texture2D>(texturePath);

            resource.filterMode = FilterMode.Bilinear;
            resource.anisoLevel = 4;

            return resource;
        }

        public void LoadQuizToQuizStore(string jsonContent)
        {
            var questionJson = JsonConvert.DeserializeObject<SchemaQuiz>(jsonContent);

            QuizState.Default.ResetQuestionIndex();
            QuizState.Default.SetQuizList(questionJson.Data.ToList());

            var list = questionJson.Data.Select(question => new Question(
                id: question.Id,
                assetFile: question.AssetFile,
                answer: question.Answer,
                questionAssetType: question.QuestionAssetType
            )).ToList();

            QuizStore.SetQuizList(list);

            // Example: Set custom data
            //QuizStore.SetCustomData(new byte[] { 0, 1, 2 });
        }

        #region Instructor Result state nethods

        public void SpawnInstructorResultStateUI(string mode)
        {
            var quizInfo = QuizStore.QuizInfo;

            switch(mode)
            {
                case "POPQUIZ":
                case "PRETEST":
                    var studentQuizResultList = QuizSequenceHelper.GenerateStudentQuizResultInfoList(QuizStore.StudentAnswers);

                    m_InstructorPopQuizResult = SpawnQuizPrototype<BaseInstructorPopQuizResult<StudentQuizResultInfo>>(m_InstructorPopQuizResultPrefab);
                    m_InstructorPopQuizResult.SetChapter("Chapter 1");
                    m_InstructorPopQuizResult.SetMission("Mission Quiz");
                    m_InstructorPopQuizResult.SetQuizMode(mode);
                    m_InstructorPopQuizResult.SetCurrentPage(quizInfo.CurrentQuizNumber);
                    m_InstructorPopQuizResult.SetTotalPage(quizInfo.QuestionAmount);
                    m_InstructorPopQuizResult.SetElementListInfo(studentQuizResultList);
                    break;
                case "POSTTEST":
                    SwapToStudentList(mode);
                    break;
            }
        }

        public void DestroyInstructorResultStateUI(string mode)
        {
            switch(mode)
            {
                case "POPQUIZ":
                case "PRETEST":
                    if(m_InstructorPopQuizResult == null) return;

                    Destroy(m_InstructorPopQuizResult.gameObject);
                    m_InstructorPopQuizResult = null;
                    break;
                case "POSTTEST":
                    if(m_InstructorPostTestStudentListResult == null) return;

                    m_InstructorPostTestStudentListResult.OnSwapResultState.RemoveListener(SwapToAnswerList);

                    Destroy(m_InstructorPostTestStudentListResult.gameObject);
                    m_InstructorPostTestStudentListResult = null;
                    break;
            }
        }

        private void SwapToStudentList(string mode)
        {
            var quizInfo = QuizStore.QuizInfo;
            var studentPreTestResult = QuizStore.StudentPreTestResult;
            var studentQuizResultInfo = QuizSequenceHelper.GenerateStudentPostTestResultInfoList(QuizStore.StudentAnswers, studentPreTestResult, QuizStore.QuizInfo.QuestionAmount);

            Destroy(QuizConnectorController.Instance.CurrentUI);

            m_InstructorPostTestStudentListResult = SpawnQuizPrototype<BaseInstructorPostTestStudentListResult<StudentPostTestResultInfo>>(m_InstructorPostTestStudentListResultPrefab);
            m_InstructorPostTestStudentListResult.SetChapter("Chapter 1");
            m_InstructorPostTestStudentListResult.SetMission("Mission Quiz");
            m_InstructorPostTestStudentListResult.SetQuizMode(mode);
            m_InstructorPostTestStudentListResult.SetCurrentPage(quizInfo.CurrentQuizNumber);
            m_InstructorPostTestStudentListResult.SetTotalPage(quizInfo.QuestionAmount);
            m_InstructorPostTestStudentListResult.SetElementListInfo(studentQuizResultInfo);

            m_InstructorPostTestStudentListResult.OnSwapResultState.AddListener(SwapToAnswerList);
            m_InstructorPostTestStudentListResult.OnDestroyed.AddListener(OnDestroyed);

            return;

            void OnDestroyed()
            {
                m_InstructorPostTestStudentListResult.OnSwapResultState.RemoveListener(SwapToAnswerList);
                m_InstructorPostTestStudentListResult.OnDestroyed.RemoveListener(OnDestroyed);
                m_InstructorPostTestStudentListResult = null;
            }
        }

        private void SwapToAnswerList(string mode)
        {
            var quizInfo = QuizStore.QuizInfo;
            var postTestAnswer = QuizSequenceHelper.GeneratePostTestAnswerList(QuizStore);

            Destroy(QuizConnectorController.Instance.CurrentUI);

            m_InstructorPostTestAnswerListResult = SpawnQuizPrototype<BaseInstructorPostTestAnswerListResult<PostTestAnswer>>(m_InstructorPostTestAnswerListResultPrefab);
            m_InstructorPostTestAnswerListResult.SetChapter("Chapter 1");
            m_InstructorPostTestAnswerListResult.SetMission("Mission Quiz");
            m_InstructorPostTestAnswerListResult.SetQuizMode(mode);
            m_InstructorPostTestAnswerListResult.SetCurrentPage(quizInfo.CurrentQuizNumber);
            m_InstructorPostTestAnswerListResult.SetTotalPage(quizInfo.QuestionAmount);
            m_InstructorPostTestAnswerListResult.SetElementListInfo(postTestAnswer);

            m_InstructorPostTestAnswerListResult.OnClicked.AddListener(OnSwapToAnswerReveal);
            m_InstructorPostTestAnswerListResult.OnSwapResultState.AddListener(SwapToStudentList);
            m_InstructorPostTestAnswerListResult.OnDestroyed.AddListener(OnDestroyed);

            return;

            void OnDestroyed()
            {
                m_InstructorPostTestAnswerListResult.OnClicked.RemoveListener(OnSwapToAnswerReveal);
                m_InstructorPostTestAnswerListResult.OnSwapResultState.RemoveListener(SwapToStudentList);
                m_InstructorPostTestAnswerListResult.OnDestroyed.RemoveListener(OnDestroyed);
                m_InstructorPostTestAnswerListResult = null;
            }

            void OnSwapToAnswerReveal(PostTestAnswer answer) => SwapToAnswerReveal(answer, mode);
        }

        private void SwapToAnswerReveal(PostTestAnswer postTestAnswer, string mode)
        {
            var quizInfo = QuizStore.QuizInfo;

            Destroy(QuizConnectorController.Instance.CurrentUI);

            m_InstructorPostTestAnswerRevealResult = SpawnQuizPrototype<BaseInstructorPostTestAnswerRevealResult>(m_InstructorPostTestAnswerRevealResultPrefab);
            m_InstructorPostTestAnswerRevealResult.SetChapter("Chapter 1");
            m_InstructorPostTestAnswerRevealResult.SetMission("Mission Quiz");
            m_InstructorPostTestAnswerRevealResult.SetQuizMode(mode);
            m_InstructorPostTestAnswerRevealResult.SetCurrentPage(quizInfo.CurrentQuizNumber);
            m_InstructorPostTestAnswerRevealResult.SetTotalPage(quizInfo.QuestionAmount);
            m_InstructorPostTestAnswerRevealResult.SetCurrentQuestionIndex(postTestAnswer.QuestionIndex);
            m_InstructorPostTestAnswerRevealResult.SetQuestionTexture(GenerateQuestionTexture(postTestAnswer.QuestionImagePath));
            m_InstructorPostTestAnswerRevealResult.SetCorrectAnswer(postTestAnswer.QuestionAnswer);

            m_InstructorPostTestAnswerRevealResult.OnShowAnswerList.AddListener(SwapToAnswerList);
            m_InstructorPostTestAnswerRevealResult.OnSwapResultState.AddListener(SwapToStudentList);
            m_InstructorPostTestAnswerRevealResult.OnNextAnswerReveal.AddListener(OnAnswerRevealChange);
            m_InstructorPostTestAnswerRevealResult.OnPreviousAnswerReveal.AddListener(OnAnswerRevealChange);
            m_InstructorPostTestAnswerRevealResult.OnDestroyed.AddListener(OnDestroyed);

            return;

            void OnDestroyed()
            {
                m_InstructorPostTestAnswerRevealResult.OnShowAnswerList.RemoveListener(SwapToAnswerList);
                m_InstructorPostTestAnswerRevealResult.OnSwapResultState.RemoveListener(SwapToStudentList);
                m_InstructorPostTestAnswerRevealResult.OnNextAnswerReveal.RemoveListener(OnAnswerRevealChange);
                m_InstructorPostTestAnswerRevealResult.OnPreviousAnswerReveal.RemoveListener(OnAnswerRevealChange);
                m_InstructorPostTestAnswerRevealResult.OnDestroyed.RemoveListener(OnDestroyed);
                m_InstructorPostTestAnswerRevealResult = null;
            }
        }

        private void OnAnswerRevealChange(int index)
        {
            var postTestAnswerList = QuizSequenceHelper.GeneratePostTestAnswerList(QuizStore);
            var max = postTestAnswerList.Max(item => item.QuestionIndex);

            index = index < 0 ? 0 : index;
            index = index > max ? max : index;

            var postTestAnswer = postTestAnswerList.Find(item => item.QuestionIndex == index);

            if(m_InstructorPostTestAnswerRevealResult == null) return;

            m_InstructorPostTestAnswerRevealResult.SetCurrentQuestionIndex(postTestAnswer.QuestionIndex);
            m_InstructorPostTestAnswerRevealResult.SetQuestionTexture(GenerateQuestionTexture(postTestAnswer.QuestionImagePath));
            m_InstructorPostTestAnswerRevealResult.SetCorrectAnswer(postTestAnswer.QuestionAnswer);
        }

        #endregion

        #region Student Result state methods

        public void SpawnStudentResultStateUI(string mode)
        {
            var quizInfo = QuizStore.QuizInfo;

            switch(mode)
            {
                case "POPQUIZ":
                case "PRETEST":
                    m_StudentPopQuizResult = SpawnQuizPrototype<BaseStudentPopQuizResult>(m_StudentPopQuizResultPrefab);
                    m_StudentPopQuizResult.SetChapter("Chapter 1");
                    m_StudentPopQuizResult.SetMission("Mission Quiz");
                    m_StudentPopQuizResult.SetQuizMode(mode);
                    m_StudentPopQuizResult.SetCurrentPage(quizInfo.CurrentQuizNumber);
                    m_StudentPopQuizResult.SetTotalPage(quizInfo.QuestionAmount);
                    m_StudentPopQuizResult.SetQuestionAmount(quizInfo.QuestionAmount);
                    m_StudentPopQuizResult.SetCurrentScore(QuizStore.CorrectAnswerAmount);
                    break;
                case "POSTTEST":
                    var preTestResult = QuizStore.PreTestResult;

                    m_StudentPostTestResult = SpawnQuizPrototype<BaseStudentPostTestResult>(m_StudentPostTestResultPrefab);
                    m_StudentPostTestResult.SetChapter("Chapter 1");
                    m_StudentPostTestResult.SetMission("Mission Quiz");
                    m_StudentPostTestResult.SetQuizMode(mode);
                    m_StudentPostTestResult.SetCurrentPage(quizInfo.CurrentQuizNumber);
                    m_StudentPostTestResult.SetTotalPage(quizInfo.QuestionAmount);
                    m_StudentPostTestResult.SetQuestionAmount(quizInfo.QuestionAmount);
                    m_StudentPostTestResult.SetCurrentScore(QuizStore.CorrectAnswerAmount);
                    m_StudentPostTestResult.SetPreTestScore(preTestResult.Score, preTestResult.HasScore);
                    break;
            }
        }

        public void DestroyStudentResultStateUI(string mode)
        {
            switch(mode)
            {
                case "POPQUIZ":
                case "PRETEST":
                    if(m_StudentPopQuizResult == null) return;

                    Destroy(m_StudentPopQuizResult.gameObject);
                    m_StudentPopQuizResult = null;
                    break;
                case "POSTTEST":
                    if(m_StudentPostTestResult == null) return;

                    Destroy(m_StudentPostTestResult.gameObject);
                    m_StudentPostTestResult = null;
                    break;
            }
        }

        #endregion

        public void OnStudentAnswerReceive(string stationId, string answer, int answerStudentAmount, int studentAmount)
        {
            throw new NotImplementedException();
        }

        public void OnCustomDataMessageReceive(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void SpawnInstructorCountInStateUI()
        {
            throw new NotImplementedException();
        }

        public void DestroyInstructorCountInStateUI()
        {
            throw new NotImplementedException();
        }

        public void AddInstructorFinishCountInStateEventListener(UnityAction action)
        {
            throw new NotImplementedException();
        }

        public void RemoveInstructorFinishCountInStateEventListener(UnityAction action)
        {
            throw new NotImplementedException();
        }

        public void SpawnInstructorQuestionStateUI()
        {
            throw new NotImplementedException();
        }

        public void DestroyInstructorQuestionStateUI()
        {
            throw new NotImplementedException();
        }

        public void SpawnInstructorPreResultStateUI()
        {
            throw new NotImplementedException();
        }

        public void DestroyInstructorPreResultStateUI()
        {
            throw new NotImplementedException();
        }

        public void SpawnInstructorEndStateUI()
        {
            throw new NotImplementedException();
        }

        public void DestroyInstructorEndStateUI()
        {
            throw new NotImplementedException();
        }

        public void SpawnStudentCountInStateUI()
        {
            throw new NotImplementedException();
        }

        public void DestroyStudentCountInStateUI()
        {
            throw new NotImplementedException();
        }

        public void SpawnStudentQuestionStateUI()
        {
            throw new NotImplementedException();
        }

        public void DestroyStudentQuestionStateUI()
        {
            throw new NotImplementedException();
        }

        public void SpawnStudentSubmitedStateUI()
        {
            throw new NotImplementedException();
        }

        public void DestroyStudentSubmitedStateUI()
        {
            throw new NotImplementedException();
        }

        public void AddStudentSubmitedQuestionStateEventListener(UnityAction action)
        {
            throw new NotImplementedException();
        }

        public void RemoveStudentSubmitedQuestionStateEventListener(UnityAction action)
        {
            throw new NotImplementedException();
        }

        public void SpawnStudentPreResultStateUI()
        {
            throw new NotImplementedException();
        }

        public void DestroyStudentPreResultStateUI()
        {
            throw new NotImplementedException();
        }

        public void SpawnStudentEndStateUI()
        {
            throw new NotImplementedException();
        }

        public void DestroyStudentEndStateUI()
        {
            throw new NotImplementedException();
        }
    }
}