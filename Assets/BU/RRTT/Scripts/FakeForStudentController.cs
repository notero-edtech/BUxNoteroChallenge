using BU.QuizExample.QuizExampleMessages;
using BU.QuizExample.QuizExampleMessages.EventHandler;
using DataStore;
using DataStore.Quiz;
using Mirror;
using Notero.QuizConnector;
using Notero.Unity.Networking.Mirror;
using Notero.Utilities;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BU.RRTT.QuizExample.Scripts
{
    public class FakeForStudentController : MonoSingleton<FakeForStudentController>
    {
        private QuizControllerType m_QuizControllerType { get; set; }

        private string m_StationId { get; set; }

        private QuizModes QuizMode { get; set; }

        private QuizStore m_QuizStore => Store.QuizStore;

        private IEventHandler[] m_EventHandler;
        private EventBus m_EventBus;
        private Store Store;
        private string m_JsonContent;

        public void Init(string stationId, QuizControllerType quizControllerType, string jsonContent)
        {
            Store = Store.Default;
            m_EventBus = EventBus.Default;
            m_StationId = stationId;
            m_QuizControllerType = quizControllerType;
            m_JsonContent = jsonContent;

            m_EventHandler = new IEventHandler[]
            {
                new InstructorQuizEventHandler(m_QuizStore),
                new StudentQuizEventHandler(m_QuizStore),
            };

            SubscribeStudentEvent();
        }

        public void SetQuizMode(QuizModes quizMode) => QuizMode = quizMode;

        private void OnDisable()
        {
            UnsubscribeStudentEvent();
        }

        public void DestroyCurrentUI()
        {
            Destroy(QuizConnectorController.Instance.CurrentUI);
        }

        private void InstructorNextQuestion()
        {
            var quizState = QuizState.Default;

            if(quizState.GetNextQuestionIfExist(out var quiz))
            {
                SendQuizInfoToStudent(quizState.CurrentQuestionNumber, quizState.QuestionAmount);
                SendCurrentQuestionToStudent(quiz.Id);
                SendCorrectAnswerToStudent("");

                SendQuizStateToStudent(((int)QuizStates.COUNTIN));
            }
            else
            {
                SendQuizStateToStudent(((int)QuizStates.END));
            }
        }

        private IEnumerator InstructorFakeAction(int delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }

        #region Send event message to Instructor

        private void SendAnswerToInstructor(string answer)
        {
            if(m_EventBus == null) return;
            m_EventBus.Publish(new LocalConnectionToClient(), new StudentAnswerMessage() { StationId = m_StationId, Answer = answer });
        }

        #endregion

        #region Send event message to Student

        private void SendCustomDataToStudent(byte[] data)
        {
            if(m_EventBus == null) return;
            m_EventBus.Publish(new CustomDataMessage()
            {
                Data = data
            });
        }

        private void SendQuizStateToStudent(int nextState)
        {
            if(m_EventBus == null) return;
            m_EventBus.Publish(new StateChangeMessage()
            {
                NextQuizState = nextState
            });
        }

        private void SendQuizResultToStudent(int correctAnswerAmount)
        {
            if(m_EventBus == null) return;
            m_EventBus.Publish(new QuizResultMessage()
            {
                CorrectAnswerAmount = correctAnswerAmount
            });
        }

        private void SendPreTestResultToStudent(int score, int fullScore)
        {
            if(m_EventBus == null) return;
            m_EventBus.Publish(new PreTestResultMessage()
            {
                HasScore = true,
                Score = score,
                FullScore = fullScore
            });
        }

        private void SendCorrectAnswerToStudent(string answer)
        {
            if(m_EventBus == null) return;
            m_EventBus.Publish(new AnswerCorrectMessage()
            {
                Answer = answer
            });
        }

        private void SendQuizInfoToStudent(int currentQuizNumber, int questionAmount)
        {
            if(m_EventBus == null) return;
            m_EventBus.Publish(new QuizInfoMessage()
            {
                QuestionAmount = questionAmount,
                CurrentQuizNumber = currentQuizNumber
            });
        }

        private void SendCurrentQuestionToStudent(string id)
        {
            if(m_EventBus == null) return;
            if(m_QuizStore.QuizList.TryGetValue(id, out var question))
            {
                m_EventBus.Publish(new CurrentQuestionMessage()
                {
                    Id = question.Id,
                    AssetFile = question.AssetFile,
                    AssetAnswerFile = question.Answer.AssetAnswerFile,
                    QuestionAssetType = question.QuestionAssetType
                });
            }
        }

        #endregion

        private void SubscribeStudentEvent()
        {
            foreach(var eventHandler in m_EventHandler) eventHandler.Subscribe(m_EventBus);

            // Instructor
            m_EventBus.Subscribe<StudentAnswerMessage>(OnStudentAnswerReceived);

            // Student
            m_EventBus.Subscribe<StateChangeMessage>(OnStateChangeReceived);
            m_EventBus.Subscribe<CustomDataMessage>(OnStudentCustomDataReceived);
        }

        private void UnsubscribeStudentEvent()
        {
            // Instructor
            m_EventBus.Unsubscribe<StudentAnswerMessage>(OnStudentAnswerReceived);

            // Student
            m_EventBus.Unsubscribe<StateChangeMessage>(OnStateChangeReceived);
            m_EventBus.Unsubscribe<CustomDataMessage>(OnStudentCustomDataReceived);

            foreach(var eventHandler in m_EventHandler) eventHandler.Unsubscribe(m_EventBus);
        }

        private void OnStateChangeReceived(StateChangeMessage message)
        {
            StudentGoToNextState(message);
        }

        private void OnStudentAnswerReceived(NetworkConnectionToClient connection, StudentAnswerMessage message)
        {
            var answer = QuizState.Default.CurrentQuestion.Answer.CorrectAnswers.ElementAt(0);
            SendCorrectAnswerToStudent(answer);
        }

        private void OnStudentCustomDataReceived(CustomDataMessage message)
        {
            QuizConnectorController.Instance.OnCustomDataMessageReceive(message.Data);
        }

        private void StudentGoToNextState(StateChangeMessage message)
        {
            switch((QuizStates)message.NextQuizState)
            {
                case QuizStates.STARTER:
                    StarterState();
                    break;
                case QuizStates.LOADING:
                    LoadingState();
                    break;
                case QuizStates.COUNTIN:
                    CountInState();
                    break;
                case QuizStates.QUESTION:
                    QuestionState();
                    break;
                case QuizStates.PRERESULT:
                    PreResultState();
                    break;
                case QuizStates.END:
                    EndState();
                    break;
            }
        }

        #region StarterState

        public void StarterState()
        {
            QuizConnectorController.Instance.Init(m_QuizStore, "A");
            QuizConnectorController.Instance.OnStudentSubmit.AddListener(SendAnswerToInstructor);

            LoadingState();
        }

        #endregion

        #region LoadingState

        private void LoadingState()
        {
            QuizConnectorController.Instance.LoadQuizToQuizStore(m_JsonContent);

            // Example:send custom data to student
            //SendCustomDataToStudent(new byte[] { 0, 1, 2 });

            if(m_QuizControllerType == QuizControllerType.FLOW)
            {
                InstructorNextQuestion();
            }
            else if(m_QuizControllerType == QuizControllerType.RESULT)
            {
                // [Instructor] Mock result data and send from instructor to student side
                var questionAmount = QuizState.Default.QuestionAmount;
                var currentScore = Random.Range(1, m_QuizStore.QuizInfo.QuestionAmount + 1);

                SendQuizInfoToStudent(questionAmount, questionAmount);
                SendQuizResultToStudent(currentScore);

                if(QuizMode == QuizModes.POSTTEST)
                {
                    var preTestScore = Random.Range(1, m_QuizStore.QuizInfo.QuestionAmount + 1);

                    SendPreTestResultToStudent(preTestScore, m_QuizStore.QuizInfo.QuestionAmount);
                }

                ResultState();
            }
        }

        #endregion

        #region CountInState

        private int CountdownTime;

        private void CountInState()
        {
            QuizConnectorController.Instance.SpawnStudentCountInStateUI();

            CountdownTime = 3;
            StartCoroutine(StartCountdownCoroutine((time) =>
            {
                if(time < 1) SendQuizStateToStudent((int)QuizStates.QUESTION);
            }));
        }

        private IEnumerator StartCountdownCoroutine(Action<int> tick)
        {
            tick?.Invoke(CountdownTime);
            if(CountdownTime == 0) yield break;

            yield return new WaitForSeconds(1);
            CountdownTime--;

            yield return StartCountdownCoroutine(tick);
        }

        #endregion

        #region QuestionState

        private void QuestionState()
        {
            QuizConnectorController.Instance.DestroyStudentCountInStateUI();
            QuizConnectorController.Instance.SpawnStudentQuestionStateUI();
            QuizConnectorController.Instance.AddStudentSubmitedQuestionStateEventListener(OnNextState);
        }

        private void OnNextState()
        {
            QuizConnectorController.Instance.RemoveStudentSubmitedQuestionStateEventListener(OnNextState);
            QuizConnectorController.Instance.DestroyStudentQuestionStateUI();
            QuizConnectorController.Instance.SpawnStudentSubmitedStateUI();

            if(QuizMode == QuizModes.POPQUIZ)
            {
                // Instructor processing answer and send result to student
                CoroutineHelper.Instance.StartCoroutine(InstructorFakeAction(3, () => { SendQuizStateToStudent((int)QuizStates.PRERESULT); }));
                return;
            }

            // Instructor call function NextQuestion is fake
            CoroutineHelper.Instance.StartCoroutine(InstructorFakeAction(3, () =>
            {
                InstructorNextQuestion();
                QuizConnectorController.Instance.DestroyStudentSubmitedStateUI();
            }));
        }

        #endregion

        #region PreResult state

        private void PreResultState()
        {
            QuizConnectorController.Instance.DestroyStudentSubmitedStateUI();
            QuizConnectorController.Instance.SpawnStudentPreResultStateUI();
            CoroutineHelper.Instance.StartCoroutine(InstructorFakeAction(3, () =>
            {
                QuizConnectorController.Instance.DestroyStudentPreResultStateUI();
                InstructorNextQuestion();
            }));
        }

        #endregion

        #region EndState

        private void EndState()
        {
            QuizConnectorController.Instance.SpawnStudentEndStateUI();
        }

        #endregion

        #region ResultState

        private void ResultState()
        {
            QuizConnectorController.Instance.SpawnStudentResultStateUI(QuizMode.ToString());
        }

        #endregion
    }
}