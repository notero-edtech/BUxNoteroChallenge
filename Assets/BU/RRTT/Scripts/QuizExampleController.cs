using DataStore.Quiz;
using Notero.QuizConnector;
using Notero.QuizConnector.Instructor;
using Notero.QuizConnector.Model;
using Notero.QuizConnector.Student;
using Notero.Unity.UI.Quiz;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace BU.RRTT.Scripts
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
        private GameObject m_StudentPopQuizResultPrefab;

        [SerializeField]
        private GameObject m_StudentPostTestResultPrefab;

        [SerializeField]
        private PianoKeyQuizController m_PianoKeyInput;

        // Instructor
        private BaseInstructorCountIn m_InstructorCountIn;
        private BaseInstructorQuestion m_InstructorQuestion;
        private BaseInstructorPreResult m_InstructorPreResult;
        private BaseInstructorEnd m_InstructorEnd;
        private BaseInstructorPopQuizResult<StudentQuizResultInfo> m_InstructorPopQuizResult;
        private BaseInstructorPostTestStudentListResult<StudentPostTestResultInfo> m_InstructorPostTestStudentListResult;
        private BaseInstructorPostTestAnswerListResult<PostTestAnswer> m_InstructorPostTestAnswerListResult;
        private BaseInstructorPostTestAnswerRevealResult m_InstructorPostTestAnswerRevealResult;

        // Student
        private BaseStudentCountIn m_StudentCountIn;
        private BaseStudentQuestion m_StudentQuestion;
        private BaseStudentWaiting m_StudentWaiting;
        private BaseStudentPreResult m_StudentPreResult;
        private BaseStudentEnd m_StudentEnd;
        private BaseStudentPopQuizResult m_StudentPopQuizResult;
        private BaseStudentPostTestResult m_StudentPostTestResult;

        public bool IsQuizLoaded { get; set; }

        public QuizStore QuizStore { get; set; }

        public UnityEvent OnNextStateReceive { get; set; }

        public UnityEvent<string> OnStudentSubmit { get; set; }

        public UnityEvent<byte[]> OnCustomDataReceive { get; set; }

        private Transform m_Container;
        private int m_CountdownTime;
        private int m_ChapterIndex;
        private string m_Chapter;
        private string m_Mission;
        private string m_Answer;
        private string m_RootDirectory;
        private Coroutine m_CountdownCoroutine;

        private event Action<byte[]> OnCustomDataUIReceive;
        private Func<string, string, Texture> m_GenerateTextureLogicDefault;

        //RRTT Variables
        private byte bossIndex;
        private int m_CurrentQuiz;

        public void Init(Transform container, QuizStore quizStore, Func<string, string, Texture> logic)
        {
            m_Container = container;
            QuizStore = quizStore;
            m_GenerateTextureLogicDefault = logic;
        }

        public void SetChapterIndex(int chapterIndex)
        {
            m_ChapterIndex = chapterIndex;
        }

        public void SetRootDirectory(string rootDirectory)
        {
            m_RootDirectory = rootDirectory;
        }

        public void SetChapter(string chapter)
        {
            m_Chapter = chapter;
        }

        public void SetMission(string mission)
        {
            m_Mission = mission;
        }

        public void OnCustomDataMessageReceive(byte[] data)
        {
            OnCustomDataUIReceive?.Invoke(data);
        }

        public void SetGenerateTextureLogic(Func<string, string, Texture> logic)
        {
            m_GenerateTextureLogicDefault = logic;
        }

        public void LoadQuizToQuizStore(string quizJson)
        {
            bossIndex = (byte)Random.Range(0, 5);

            QuizState.Default.Load(
                assetJson: quizJson,
                onFinished: quizList =>
                {
                    var list = quizList.Select(question => new Question
                    {
                        Id = question.Id,
                        AssetFile = question.AssetFile,
                        Answer = question.Answer,
                        QuestionAssetType = question.QuestionAssetType
                    }).ToList();

                    QuizStore.SetQuizList(list);
                    QuizStore.SetCustomData(new byte[] { bossIndex, 0 });
                    OnCustomDataReceive?.Invoke(new byte[] { bossIndex, 0 });

                    IsQuizLoaded = true;
                },
                onFailed: _ => { Debug.LogError("Error: Loader json file failed"); });
        }

        public void SpawnInstructorCountInStateUI()
        {
            m_InstructorCountIn = SpawnPrototype<BaseInstructorCountIn>(m_InstructorCountInPrefab);

            // Start countdown
            m_CountdownTime = 3;

            if(m_CountdownCoroutine != null) return;

            m_CountdownCoroutine = StartCoroutine(StartCountdownCoroutine((time) =>
            {
                if(m_InstructorCountIn == null) return;

                m_InstructorCountIn.OnCountdownSet(time);
            }, () => m_CountdownCoroutine = null));
        }

        public void DestroyInstructorCountInStateUI()
        {
            if(m_InstructorCountIn == null) return;

            Destroy(m_InstructorCountIn.gameObject);
            m_CountdownCoroutine = null;
            m_InstructorCountIn = null;
        }

        public void AddInstructorFinishCountInStateEventListener(UnityAction action)
        {
            if(m_InstructorCountIn == null) return;

            m_InstructorCountIn.OnNextState.AddListener(action);
        }

        public void RemoveInstructorFinishCountInStateEventListener(UnityAction action)
        {
            if(m_InstructorCountIn == null) return;

            m_InstructorCountIn.OnNextState.RemoveListener(action);
        }

        public void SpawnInstructorQuestionStateUI()
        {
            var assetType = QuizStore.CurrentQuestion.QuestionAssetType;
            var assetFilePath = QuizStore.CurrentQuestion.AssetFile;

            m_InstructorQuestion = SpawnPrototype<BaseInstructorQuestion>(m_InstructorQuestionPrefab);
            m_CurrentQuiz++;

            switch(assetType)
            {
                case QuestionAssetType.IMAGE:
                {
                    var texture = GenerateQuestionTexture(assetFilePath, m_RootDirectory);
                    m_InstructorQuestion.SetQuestionImage(texture);
                    break;
                }
                case QuestionAssetType.VIDEO:
                {
                    var url = Path.Combine(m_RootDirectory, assetFilePath);
                    m_InstructorQuestion.SetQuestionVideo(url);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            m_InstructorQuestion.SetStudentAmount(QuizStore.StudentAmount);

            m_InstructorQuestion.OnNextState = OnNextStateReceive;
            m_InstructorQuestion.OnDestroyed.AddListener(OnDestroyed);
            QuizStore.OnStudentAmountChange += OnStudentAmountChange;

            return;

            void OnStudentAmountChange(int amount)
            {
                if(m_InstructorQuestion == null) return;

                m_InstructorQuestion.SetStudentAmount(amount);
            }

            void OnDestroyed()
            {
                QuizStore.OnStudentAmountChange -= OnStudentAmountChange;

                if(m_InstructorQuestion == null) return;

                m_InstructorQuestion.OnDestroyed.RemoveListener(OnDestroyed);
            }
        }

        public void DestroyInstructorQuestionStateUI()
        {
            //Boss Heart System
            var quizList = QuizStore.QuizList;
            var studentAnswer = QuizStore.StudentAnswers;
            byte heart = 0;

            foreach(var question in quizList.Values)
            {
                if(!studentAnswer.TryGetValue(question.Id, out var studentList)) continue;

                var score = studentList.Count(student => student.Answer == question.Answer.CorrectAnswers.ElementAt(0));

                if(score >= (0.25f * studentList.Count)) heart++;

                QuizStore.SetCustomData(new[] { bossIndex, heart });
                OnCustomDataReceive?.Invoke(new[] { bossIndex, heart });
            }

            if(m_InstructorQuestion == null) return;

            Destroy(m_InstructorQuestion.gameObject);
            m_InstructorQuestion = null;
        }

        public void SetFullScreen(bool isFull)
        {
            if(m_InstructorQuestion == null) return;

            m_InstructorQuestion.SetFullScreen(isFull);
        }

        public void OnStudentAnswerReceive(string stationId, string answer, int answerStudentAmount, int studentAmount)
        {
            if(m_InstructorQuestion == null) return;

            m_InstructorQuestion.OnStudentAnswerReceive(answerStudentAmount, studentAmount);
        }

        public void SpawnInstructorPreResultStateUI()
        {
            //Boss Heart System
            var quiz = QuizStore.QuizInfo;
            var quizList = QuizStore.QuizList;
            var studentAnswer = QuizStore.StudentAnswers;
            byte currentHeart = 0;
            byte heart = 0;

            foreach(var question in quizList.Values)
            {
                if(!studentAnswer.TryGetValue(question.Id, out var studentList))
                {
                    continue;
                }
                if(m_CurrentQuiz == quiz.CurrentQuizNumber)
                {
                    var score = 0;

                    score += studentList.Count(student => student.Answer == question.Answer.CorrectAnswers.ElementAt(0));

                    if(score >= (0.25f * studentList.Count))
                    {
                        currentHeart++;
                    }
                    QuizStore.SetCustomData(new[] { bossIndex, currentHeart, heart });
                    heart = currentHeart;
                }
            }
            
            //Set per-result data
            var assetFilePath = QuizStore.CurrentQuestion.Answer.AssetAnswerFile;
            var correctAnswer = QuizStore.CorrectAnswer;
            var studentAmount = QuizStore.StudentAmount;
            var answerAmount = QuizStore.AnswerStudentAmount;
            var AnswerSummaryDic = QuizStore.AnswerSummaryDic;
            var texture = GenerateQuestionTexture(assetFilePath, m_RootDirectory);

            m_InstructorPreResult = SpawnPrototype<BaseInstructorPreResult>(m_InstructorPreResultPrefab);
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

        public void SpawnInstructorResultStateUI(string mode)
        {
            switch(mode)
            {
                case "POP_QUIZ":
                case "PRE_TEST":
                    var studentQuizResultList = QuizSequenceHelper.GenerateStudentQuizResultInfoList(QuizStore.StudentAnswers);
                    var modeText = getModeText();

                    m_InstructorPopQuizResult = SpawnPrototype<BaseInstructorPopQuizResult<StudentQuizResultInfo>>(m_InstructorPopQuizResultPrefab);
                    m_InstructorPopQuizResult.SetQuizMode(modeText);
                    m_InstructorPopQuizResult.SetElementListInfo(studentQuizResultList);
                    break;
                case "POST_TEST":
                    SwapToStudentList(mode);
                    break;
            }

            return;

            string getModeText()
            {
                return mode switch
                {
                    "POP_QUIZ" => "Pop Quiz",
                    "PRE_TEST" => "Pre Test",
                    "POST_TEST" => "Post Test",
                    _ => mode
                };
            }
        }

        public void DestroyInstructorResultStateUI(string mode)
        {
            switch(mode)
            {
                case "POP_QUIZ":
                case "PRE_TEST":
                    if(m_InstructorPopQuizResult == null) return;

                    Destroy(m_InstructorPopQuizResult.gameObject);
                    m_InstructorPopQuizResult = null;
                    break;
                case "POST_TEST":
                    if(m_InstructorPostTestStudentListResult == null) return;

                    m_InstructorPostTestStudentListResult.OnSwapResultState.RemoveListener(SwapToAnswerList);

                    Destroy(m_InstructorPostTestStudentListResult.gameObject);
                    m_InstructorPostTestStudentListResult = null;
                    break;
            }
        }

        private void SwapToStudentList(string mode)
        {
            var studentQuizResultInfo = QuizSequenceHelper.GenerateStudentPostTestResultInfoList(QuizStore.StudentAnswers, QuizStore.StudentPreTestResult, QuizStore.QuizInfo.QuestionAmount);
            var modeText = getModeText();

            Destroy(QuizConnectorController.Instance.CurrentUI);

            m_InstructorPostTestStudentListResult = SpawnPrototype<BaseInstructorPostTestStudentListResult<StudentPostTestResultInfo>>(m_InstructorPostTestStudentListResultPrefab);
            m_InstructorPostTestStudentListResult.SetQuizMode(modeText);
            m_InstructorPostTestStudentListResult.SetElementListInfo(studentQuizResultInfo);

            m_InstructorPostTestStudentListResult.OnSwapResultState.AddListener(SwapToAnswerList);
            m_InstructorPostTestStudentListResult.OnDestroyed.AddListener(OnDestroyed);

            return;

            string getModeText()
            {
                return mode switch
                {
                    "POP_QUIZ" => "Pop Quiz",
                    "PRE_TEST" => "Pre Test",
                    "POST_TEST" => "Post Test",
                    _ => mode
                };
            }

            void OnDestroyed()
            {
                m_InstructorPostTestStudentListResult.OnSwapResultState.RemoveListener(SwapToAnswerList);
                m_InstructorPostTestStudentListResult.OnDestroyed.RemoveListener(OnDestroyed);
                m_InstructorPostTestStudentListResult = null;
            }
        }

        private void SwapToAnswerList(string mode)
        {
            var postTestAnswer = QuizSequenceHelper.GeneratePostTestAnswerList(QuizStore);
            var modeText = getModeText();

            Destroy(QuizConnectorController.Instance.CurrentUI);

            m_InstructorPostTestAnswerListResult = SpawnPrototype<BaseInstructorPostTestAnswerListResult<PostTestAnswer>>(m_InstructorPostTestAnswerListResultPrefab);
            m_InstructorPostTestAnswerListResult.SetQuizMode(modeText);
            m_InstructorPostTestAnswerListResult.SetElementListInfo(postTestAnswer);

            m_InstructorPostTestAnswerListResult.OnClicked.AddListener(OnSwapToAnswerReveal);
            m_InstructorPostTestAnswerListResult.OnSwapResultState.AddListener(SwapToStudentList);
            m_InstructorPostTestAnswerListResult.OnDestroyed.AddListener(OnDestroyed);

            return;

            string getModeText()
            {
                return mode switch
                {
                    "POP_QUIZ" => "Pop Quiz",
                    "PRE_TEST" => "Pre Test",
                    "POST_TEST" => "Post Test",
                    _ => mode
                };
            }

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
            Destroy(QuizConnectorController.Instance.CurrentUI);

            var modeText = getModeText();

            m_InstructorPostTestAnswerRevealResult = SpawnPrototype<BaseInstructorPostTestAnswerRevealResult>(m_InstructorPostTestAnswerRevealResultPrefab);
            m_InstructorPostTestAnswerRevealResult.SetQuizMode(modeText);
            m_InstructorPostTestAnswerRevealResult.SetCurrentQuestionIndex(postTestAnswer.QuestionIndex);
            m_InstructorPostTestAnswerRevealResult.SetQuestionTexture(GenerateQuestionTexture(postTestAnswer.QuestionImagePath, m_RootDirectory));
            m_InstructorPostTestAnswerRevealResult.SetCorrectAnswer(postTestAnswer.QuestionAnswer);

            m_InstructorPostTestAnswerRevealResult.OnShowAnswerList.AddListener(SwapToAnswerList);
            m_InstructorPostTestAnswerRevealResult.OnSwapResultState.AddListener(SwapToStudentList);
            m_InstructorPostTestAnswerRevealResult.OnNextAnswerReveal.AddListener(OnAnswerRevealChange);
            m_InstructorPostTestAnswerRevealResult.OnPreviousAnswerReveal.AddListener(OnAnswerRevealChange);
            m_InstructorPostTestAnswerRevealResult.OnDestroyed.AddListener(OnDestroyed);

            return;

            string getModeText()
            {
                return mode switch
                {
                    "POP_QUIZ" => "Pop Quiz",
                    "PRE_TEST" => "Pre Test",
                    "POST_TEST" => "Post Test",
                    _ => mode
                };
            }

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
            m_InstructorPostTestAnswerRevealResult.SetQuestionTexture(GenerateQuestionTexture(postTestAnswer.QuestionImagePath, m_RootDirectory));
            m_InstructorPostTestAnswerRevealResult.SetCorrectAnswer(postTestAnswer.QuestionAnswer);
        }

        public void SpawnStudentCountInStateUI()
        {
            m_StudentCountIn = SpawnPrototype<BaseStudentCountIn>(m_StudentCountInPrefab);

            // Start countdown
            m_CountdownTime = 3;
            StartCoroutine(StartCountdownCoroutine((time) => m_StudentCountIn.OnCountdownSet(time)));
        }

        public void DestroyStudentCountInStateUI()
        {
            if(m_StudentCountIn == null) return;

            Destroy(m_StudentCountIn.gameObject);
            m_StudentCountIn = null;
        }

        public void SpawnStudentQuestionStateUI()
        {
            var assetType = QuizStore.CurrentQuestion.QuestionAssetType;

            m_StudentQuestion = SpawnPrototype<BaseStudentQuestion>(m_StudentQuestionPrefab);

            switch(assetType)
            {
                case QuestionAssetType.IMAGE:
                    var texture = GenerateQuestionTexture(QuizStore.CurrentQuestion.AssetFile, m_RootDirectory);

                    m_StudentQuestion.SetQuestionTexture(texture);

                    break;
                case QuestionAssetType.VIDEO:
                    m_StudentQuestion.ShowWatchInstructorScreen();

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Spawner piano and subscribe event
            m_PianoKeyInput.gameObject.SetActive(true);
            m_PianoKeyInput.PianoKeyQuizSpawner();
            m_PianoKeyInput.OnReleasePiano += OnReleasePiano;
            m_PianoKeyInput.OnPressingPiano += OnPressingPiano;
            m_PianoKeyInput.OnSubmitPiano += OnSubmitPiano;

            m_Answer = "";
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

        private void OnReleasePiano(int noteIndex, string choice)
        {
            if(m_StudentQuestion == null) return;

            m_StudentQuestion.OnPianoStateReceive(PianoStates.PIANO_WAITFORINPUT, choice);
        }

        private void OnPressingPiano(int noteIndex, string choice)
        {
            if(m_StudentQuestion == null) return;

            m_StudentQuestion.OnPianoStateReceive(PianoStates.PIANO_PRESSING, choice);
        }

        private void OnSubmitPiano(int noteIndex, string choice)
        {
            m_Answer = choice;
            OnStudentSubmit?.Invoke(choice);

            if(m_StudentQuestion == null) return;

            m_StudentQuestion.OnPianoStateReceive(PianoStates.PIANO_SUBMITTED, choice);
        }

        public void SpawnStudentSubmitedStateUI()
        {
            m_StudentWaiting = SpawnPrototype<BaseStudentWaiting>(m_StudentWaitingPrefab);
        }

        public void DestroyStudentSubmitedStateUI()
        {
            if(m_StudentWaiting == null) return;

            Destroy(m_StudentWaiting.gameObject);
            m_StudentWaiting = null;
        }

        public void AddStudentSubmitedQuestionStateEventListener(UnityAction action)
        {
            if(m_StudentQuestion == null) return;

            m_StudentQuestion.OnNextState.AddListener(action);
        }

        public void RemoveStudentSubmitedQuestionStateEventListener(UnityAction action)
        {
            if(m_StudentQuestion == null) return;

            m_StudentQuestion.OnNextState.RemoveListener(action);
        }

        public void SpawnStudentPreResultStateUI()
        {
            var correctAnswer = QuizStore.CorrectAnswer;
            var texture = GenerateQuestionTexture(QuizStore.CurrentQuestion.Answer.AssetAnswerFile, m_RootDirectory);

            m_StudentPreResult = SpawnPrototype<BaseStudentPreResult>(m_StudentPreResultPrefab);
            m_StudentPreResult.SetAnswer(m_Answer);
            m_StudentPreResult.SetCorrectAnswer(correctAnswer);
            m_StudentPreResult.SetQuestionTexture(texture);
        }

        public void DestroyStudentPreResultStateUI()
        {
            if(m_StudentPreResult == null) return;

            Destroy(m_StudentPreResult.gameObject);
            m_StudentPreResult = null;
        }

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

        public void SpawnStudentResultStateUI(string mode)
        {
            var quizInfo = QuizStore.QuizInfo;

            switch(mode)
            {
                case "POP_QUIZ":
                case "PRE_TEST":
                    m_StudentPopQuizResult = SpawnPrototype<BaseStudentPopQuizResult>(m_StudentPopQuizResultPrefab);
                    m_StudentPopQuizResult.SetQuizMode(mode);
                    m_StudentPopQuizResult.SetQuestionAmount(quizInfo.QuestionAmount);
                    m_StudentPopQuizResult.SetCurrentScore(QuizStore.CorrectAnswerAmount);
                    break;
                case "POST_TEST":
                    var preTestResult = QuizStore.PreTestResult;

                    m_StudentPostTestResult = SpawnPrototype<BaseStudentPostTestResult>(m_StudentPostTestResultPrefab);
                    m_StudentPostTestResult.SetQuizMode(mode);
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
                case "POP_QUIZ":
                case "PRE_TEST":
                    if(m_StudentPopQuizResult == null) return;

                    Destroy(m_StudentPopQuizResult.gameObject);
                    m_StudentPopQuizResult = null;
                    break;
                case "POST_TEST":
                    if(m_StudentPostTestResult == null) return;

                    Destroy(m_StudentPostTestResult.gameObject);
                    m_StudentPostTestResult = null;
                    break;
            }
        }

        private T SpawnPrototype<T>(GameObject gameObj) where T : BaseQuizPanel
        {
            var prototype = Instantiate(gameObj, m_Container);
            var baseQuizPanelPrototype = prototype.GetComponent<BaseQuizPanel>();
            var quizInfo = QuizStore.QuizInfo;

            baseQuizPanelPrototype.SetChapterIndex(m_ChapterIndex);
            baseQuizPanelPrototype.SetChapter(m_Chapter);
            baseQuizPanelPrototype.SetMission(m_Mission);
            baseQuizPanelPrototype.SetCurrentPage(quizInfo.CurrentQuizNumber);
            baseQuizPanelPrototype.SetTotalPage(quizInfo.QuestionAmount);
            baseQuizPanelPrototype.OnCustomDataReceive(QuizStore.CustomData);

            baseQuizPanelPrototype.OnDestroyed.AddListener(OnPrototypeDestroyed);
            baseQuizPanelPrototype.OnSendCustomData.AddListener(OnSendCustomData);

            OnCustomDataUIReceive += OnCustomDataReceived;

            QuizConnectorController.Instance.SetCurrentUI(prototype);

            return prototype.GetComponent(typeof(T)) as T;

            void OnSendCustomData(byte[] data)
            {
                OnCustomDataReceive?.Invoke(data);
            }

            void OnCustomDataReceived(byte[] message)
            {
                baseQuizPanelPrototype.OnCustomDataReceive(message);
            }

            void OnPrototypeDestroyed()
            {
                baseQuizPanelPrototype.OnSendCustomData.RemoveListener(OnSendCustomData);
                baseQuizPanelPrototype.OnDestroyed.RemoveListener(OnPrototypeDestroyed);

                OnCustomDataUIReceive -= OnCustomDataReceived;
            }
        }

        private Texture GenerateQuestionTexture(string path, string rootDir = "")
        {
            return m_GenerateTextureLogicDefault.Invoke(path, rootDir);
        }

        private IEnumerator StartCountdownCoroutine(Action<int> tick, Action onFinished = null)
        {
            tick?.Invoke(m_CountdownTime);
            if(m_CountdownTime == 0)
            {
                onFinished?.Invoke();
                yield break;
            }

            yield return new WaitForSeconds(1);
            m_CountdownTime--;

            yield return StartCountdownCoroutine(tick, onFinished);
        }
    }
}