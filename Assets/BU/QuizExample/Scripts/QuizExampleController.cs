using DataStore.Quiz;
using Newtonsoft.Json;
using Notero.QuizConnector;
using Notero.QuizConnector.Instructor;
using Notero.QuizConnector.Student;
using Notero.Unity.UI.Quiz;
using Notero.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace BU.QuizExample.Scripts
{
    public class QuizExampleController : MonoBehaviour, IQuizController
    {
        [Header("Instructor Prefabs")]
        [SerializeField]
        private GameObject m_InstructorCountInPrefab;

        [SerializeField]
        private GameObject m_InstructorQuestionPrefab;

        [SerializeField]
        private GameObject m_InstructorPreResultPrefab;

        [SerializeField]
        private GameObject m_InstructorEndPrefab;

        [Header("Student Prefabs")]
        [SerializeField]
        private GameObject m_StudentCountInPrefab;

        [SerializeField]
        private GameObject m_StudentQuestionPrefab;

        [SerializeField]
        private GameObject m_StudentWaitingPrefab;

        [SerializeField]
        private GameObject m_StudentPreResultPrefab;

        [SerializeField]
        private GameObject m_StudentEndPrefab;

        [SerializeField]
        private PianoKeyQuizController m_PianoKeyInput;

        #region Instructor base classes

        private BaseInstructorCountIn m_InstructorCountIn;
        private BaseInstructorQuestion m_InstructorQuestion;
        private BaseInstructorPreResult m_InstructorPreResult;
        private BaseInstructorEnd m_InstructorEnd;

        #endregion

        #region Student base classes

        private BaseStudentCountIn m_StudentCountIn;
        private BaseStudentQuestion m_StudentQuestion;
        private BaseStudentWaiting m_StudentWaiting;
        private BaseStudentPreResult m_StudentPreResult;
        private BaseStudentEnd m_StudentEnd;

        #endregion

        // Container
        private Transform m_Container;

        // Coroutine Variables
        private Coroutine m_SetStudentAnswerCoroutine;
        private int CountdownTime = 3;

        private UnityEvent<byte[]> OnCustomDataUIReceive = new();

        public UnityEvent OnNextStateReceive { get; set; }

        public UnityEvent<string> OnStudentSubmit { get; set; }

        public UnityEvent<byte[]> OnCustomDataReceive { get; set; }

        public bool IsQuizLoaded { get; set; }

        public QuizStore QuizStore { get; set; }

        public void Init(Transform container, QuizStore quizStore)
        {
            m_Container = container;
            QuizStore = quizStore;
        }

        public void OnCustomDataMessageReceive(byte[] data)
        {
            OnCustomDataUIReceive?.Invoke(data);
        }

        public void LoadQuizToQuizStore(string jsonContent)
        {
            if(ApplicationFlagConfig.IsInstructorMode)
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
            else if(ApplicationFlagConfig.IsStudentMode)
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
            }
        }

        private IEnumerator StartCountdownCoroutine(Action<int> tick)
        {
            tick?.Invoke(CountdownTime);
            if(CountdownTime == 0) yield break;

            yield return new WaitForSeconds(1);
            CountdownTime--;

            yield return StartCountdownCoroutine(tick);
        }

        private Texture GenerateQuestionTexture(string path)
        {
            var imagePath = path.Split(".")[0].Trim();
            var texturePath = Path.Combine(imagePath);
            var resource = Resources.Load<Texture2D>(texturePath);
            resource.filterMode = FilterMode.Bilinear;
            resource.anisoLevel = 4;

            return resource;
        }

        private T SpawnPrototype<T>(GameObject gameObj) where T : BaseQuizPanel
        {
            if(gameObj.scene.IsValid())
            {
                Debug.Log("get one");
            }
            else
            {
                Debug.Log("not there");
            }

            var prototype = Instantiate(gameObj, m_Container);
            var quizPanelPrototype = prototype.GetComponent<BaseQuizPanel>();

            OnCustomDataUIReceive.AddListener(OnCustomDataReceived);

            quizPanelPrototype.OnCustomDataReceive(QuizStore.CustomData);
            quizPanelPrototype.OnDestroyed.AddListener(OnPrototypeDestroyed);
            quizPanelPrototype.OnSendCustomData.AddListener(OnSendCustomData);

            return prototype.GetComponent(typeof(T)) as T;

            void OnSendCustomData(byte[] data)
            {
                OnCustomDataReceive?.Invoke(data);
            }

            void OnCustomDataReceived(byte[] message)
            {
                quizPanelPrototype.OnCustomDataReceive(message);
            }

            void OnPrototypeDestroyed()
            {
                quizPanelPrototype.OnSendCustomData.RemoveListener(OnSendCustomData);
                quizPanelPrototype.OnDestroyed.RemoveListener(OnPrototypeDestroyed);

                OnCustomDataUIReceive.RemoveListener(OnCustomDataReceived);
            }
        }

        #region Instructor CountIn state methods

        public void SpawnInstructorCountInStateUI()
        {
            m_InstructorCountIn = SpawnPrototype<BaseInstructorCountIn>(m_InstructorCountInPrefab);

            //Set Countdown panel data
            var quizInfo = QuizStore.QuizInfo;
            m_InstructorCountIn.SetChapter("Chapter 1");
            m_InstructorCountIn.SetMission("Mission Quiz");
            m_InstructorCountIn.SetCurrentPage(quizInfo.CurrentQuizNumber);
            m_InstructorCountIn.SetTotalPage(quizInfo.QuestionAmount);

            // Start countdown
            CountdownTime = 3;
            StartCoroutine(StartCountdownCoroutine((time) => m_InstructorCountIn.OnCountdownSet(time)));
        }

        public void AddInstructorFinishCountInStateEventListener(UnityAction action) => m_InstructorCountIn.OnNextState.AddListener(action);

        public void RemoveInstructorFinishCountInStateEventListener(UnityAction action) => m_InstructorCountIn.OnNextState.RemoveListener(action);

        public void DestroyInstructorCountInStateUI()
        {
            if(m_InstructorCountIn == null) return;

            Destroy(m_InstructorCountIn.gameObject);
            m_InstructorCountIn = null;
        }

        #endregion

        #region Instructor Question state methods

        public void SpawnInstructorQuestionStateUI()
        {
            m_InstructorQuestion = SpawnPrototype<BaseInstructorQuestion>(m_InstructorQuestionPrefab);

            var assetFilePath = QuizStore.CurrentQuestion.AssetFile;
            var texture = GenerateQuestionTexture(assetFilePath);

            //Set question panel data
            var quizInfo = QuizStore.QuizInfo;
            m_InstructorQuestion.SetChapter("Chapter 1");
            m_InstructorQuestion.SetMission("Mission Quiz");
            m_InstructorQuestion.SetCurrentPage(quizInfo.CurrentQuizNumber);
            m_InstructorQuestion.SetTotalPage(quizInfo.QuestionAmount);
            m_InstructorQuestion.SetQuestionTexture(texture);
            m_InstructorQuestion.SetStudentAmount(QuizStore.StudentAmount);

            m_InstructorQuestion.OnNextState = OnNextStateReceive;
        }

        public void DestroyInstructorQuestionStateUI()
        {
            if(m_InstructorQuestion == null) return;

            Destroy(m_InstructorQuestion.gameObject);
            m_InstructorQuestion = null;
        }

        public void OnStudentAnswerReceive(string stationId, string answer, int answerStudentAmount, int studentAmount)
        {
            if(m_InstructorQuestion == null) return;

            m_InstructorQuestion.OnStudentAnswerReceive(answerStudentAmount, studentAmount);
        }

        #endregion

        #region Instructor Pre-Result state methods

        public void SpawnInstructorPreResultStateUI()
        {
            var correctAnswer = QuizState.Default.CurrentQuestion.Answer.CorrectAnswers.ElementAt(0);

            m_InstructorPreResult = SpawnPrototype<BaseInstructorPreResult>(m_InstructorPreResultPrefab);
            QuizStore.SetCorrectAnswer(correctAnswer);

            //Set per-result data
            var assetFilePath = QuizStore.CurrentQuestion.AssetFile;
            var texture = GenerateQuestionTexture(assetFilePath);
            var quizInfo = QuizStore.QuizInfo;
            var studentAmount = QuizStore.StudentAmount;
            var answerAmount = QuizStore.AnswerStudentAmount;
            var AnswerSummaryDic = QuizStore.AnswerSummaryDic;

            m_InstructorPreResult.SetChapter("Chapter 1");
            m_InstructorPreResult.SetMission("Mission Quiz");
            m_InstructorPreResult.SetCurrentPage(quizInfo.CurrentQuizNumber);
            m_InstructorPreResult.SetTotalPage(quizInfo.QuestionAmount);
            m_InstructorPreResult.SetQuestionTexture(texture);
            m_InstructorPreResult.SetCorrectAnswer(correctAnswer);
            m_InstructorPreResult.SetAnswerAmount(answerAmount);
            m_InstructorPreResult.SetStudentAmount(studentAmount);
            m_InstructorPreResult.SetAnswerSummaryDic(AnswerSummaryDic);

            m_InstructorPreResult.OnNextState = OnNextStateReceive;
        }

        public void DestroyInstructorPreResultStateUI()
        {
            if(m_InstructorPreResult == null) return;

            Destroy(m_InstructorPreResult.gameObject);
            m_InstructorPreResult = null;
        }

        #endregion

        #region Instructor End state methods

        public void SpawnInstructorEndStateUI()
        {
            m_InstructorEnd = SpawnPrototype<BaseInstructorEnd>(m_InstructorEndPrefab);
        }

        public void DestroyInstructorEndStateUI()
        {
            if(m_InstructorEnd == null) return;

            Destroy(m_InstructorEnd.gameObject);
            m_InstructorEnd = null;
        }

        #endregion

        #region Instructor Result state nethods

        public void SpawnInstructorResultStateUI(string mode)
        {
            throw new NotImplementedException();
        }

        public void DestroyInstructorResultStateUI(string mode)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Student CountIn state methods

        public void SpawnStudentCountInStateUI()
        {
            m_StudentCountIn = SpawnPrototype<BaseStudentCountIn>(m_StudentCountInPrefab);

            //Set Countdown panel data
            var quizInfo = QuizStore.QuizInfo;
            m_StudentCountIn.SetChapter("Chapter 1");
            m_StudentCountIn.SetMission("Mission Quiz");
            m_StudentCountIn.SetCurrentPage(quizInfo.CurrentQuizNumber);
            m_StudentCountIn.SetTotalPage(quizInfo.QuestionAmount);

            // Start countdown
            CountdownTime = 3;
            StartCoroutine(StartCountdownCoroutine((time) => m_StudentCountIn.OnCountdownSet(time)));
        }

        public void DestroyStudentCountInStateUI()
        {
            if(m_StudentCountIn == null) return;

            Destroy(m_StudentCountIn.gameObject);
            m_StudentCountIn = null;
        }

        #endregion

        #region Student Question state methods

        private string m_Answer;

        public void SpawnStudentQuestionStateUI()
        {
            m_StudentQuestion = SpawnPrototype<BaseStudentQuestion>(m_StudentQuestionPrefab);

            //Set Question panel data
            var quizInfo = QuizStore.QuizInfo;
            var texture = GenerateQuestionTexture(QuizStore.CurrentQuestion.AssetFile);

            m_StudentQuestion.SetChapter("Chapter 1");
            m_StudentQuestion.SetMission("Mission Quiz");
            m_StudentQuestion.SetCurrentPage(quizInfo.CurrentQuizNumber);
            m_StudentQuestion.SetTotalPage(quizInfo.QuestionAmount);
            m_StudentQuestion.SetQuestionTexture(texture);

            // Spawner piano and subscribe event
            m_PianoKeyInput.gameObject.SetActive(true);
            m_PianoKeyInput.PianoKeyQuizSpawner();
            m_PianoKeyInput.OnReleasePiano += OnReleasePiano;
            m_PianoKeyInput.OnPressingPiano += OnPressingPiano;
            m_PianoKeyInput.OnSubmitPiano += OnSubmitPiano;
        }

        public void DestroyStudentQuestionStateUI()
        {
            if(m_PianoKeyInput != null)
            {
                // Destroy piano and unsubscribe event
                m_PianoKeyInput.OnSubmitPiano -= OnSubmitPiano;
                m_PianoKeyInput.OnPressingPiano -= OnPressingPiano;
                m_PianoKeyInput.OnReleasePiano -= OnReleasePiano;
                m_PianoKeyInput.PianoKeyDestroy();
                m_PianoKeyInput.gameObject.SetActive(false);
            }

            if(m_StudentQuestion != null)
            {
                Destroy(m_StudentQuestion.gameObject);
                m_StudentQuestion = null;
            }
        }

        public void SpawnStudentSubmitedStateUI()
        {
            m_StudentWaiting = SpawnPrototype<BaseStudentWaiting>(m_StudentWaitingPrefab);

            var quizInfo = QuizStore.QuizInfo;
            m_StudentWaiting.SetChapter("Chapter 1");
            m_StudentWaiting.SetMission("Mission Quiz");
            m_StudentWaiting.SetCurrentPage(quizInfo.CurrentQuizNumber);
            m_StudentWaiting.SetTotalPage(quizInfo.QuestionAmount);
        }

        public void DestroyStudentSubmitedStateUI()
        {
            if(m_StudentWaiting == null) return;

            Destroy(m_StudentWaiting.gameObject);
            m_StudentWaiting = null;
        }

        public void AddStudentSubmitedQuestionStateEventListener(UnityAction action)
        {
            m_StudentQuestion.OnNextState.AddListener(action);
        }

        public void RemoveStudentSubmitedQuestionStateEventListener(UnityAction action)
        {
            m_StudentQuestion.OnNextState.RemoveListener(action);
        }

        private void OnReleasePiano(int noteIndex, string choice)
        {
            m_StudentQuestion.OnPianoStateReceive(PianoStates.PIANO_WAITFORINPUT, choice);
        }

        private void OnPressingPiano(int noteIndex, string choice)
        {
            m_StudentQuestion.OnPianoStateReceive(PianoStates.PIANO_PRESSING, choice);
        }

        private void OnSubmitPiano(int noteIndex, string choice)
        {
            OnStudentSubmit?.Invoke(choice);
            m_StudentQuestion.OnPianoStateReceive(PianoStates.PIANO_SUBMITTED, choice);
            m_Answer = choice;
        }

        #endregion

        #region Student PreResult state methods

        public void SpawnStudentPreResultStateUI()
        {
            m_StudentPreResult = SpawnPrototype<BaseStudentPreResult>(m_StudentPreResultPrefab);

            //Set PreResult panel data
            var quizInfo = QuizStore.QuizInfo;
            var texture = GenerateQuestionTexture(QuizStore.CurrentQuestion.AssetFile);
            var correctAnswer = QuizState.Default.CurrentQuestion.Answer.CorrectAnswers.ElementAt(0);

            m_StudentPreResult.SetChapter("Chapter 1");
            m_StudentPreResult.SetMission("Mission Quiz");
            m_StudentPreResult.SetCurrentPage(quizInfo.CurrentQuizNumber);
            m_StudentPreResult.SetTotalPage(quizInfo.QuestionAmount);
            m_StudentPreResult.SetAnswer(m_Answer);
            m_StudentPreResult.SetQuestionTexture(texture);
            m_StudentPreResult.SetCorrectAnswer(correctAnswer);
        }

        public void DestroyStudentPreResultStateUI()
        {
            if(m_StudentPreResult == null) return;

            Destroy(m_StudentPreResult.gameObject);
            m_StudentPreResult = null;
        }

        #endregion

        #region Student End state methods

        public void SpawnStudentEndStateUI()
        {
            m_StudentEnd = SpawnPrototype<BaseStudentEnd>(m_StudentEndPrefab);
        }

        public void DestroyStudentEndStateUI()
        {
            if(m_StudentEnd == null) return;

            Destroy(m_StudentEnd.gameObject);
            m_StudentEnd = null;
        }

        #endregion

        #region Student Result state methods

        public void SpawnStudentResultStateUI(string mode)
        {
            throw new NotImplementedException();
        }

        public void DestroyStudentResultStateUI(string mode)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}