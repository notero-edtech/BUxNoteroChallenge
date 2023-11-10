using DataStore.Quiz;
using Mirror;
using Notero.Unity.Networking.Mirror;

namespace BU.RRTT.QuizExample.QuizExampleMessages.EventHandler
{
    public class InstructorQuizEventHandler : IEventHandler
    {
        private QuizStore m_Store;

        public InstructorQuizEventHandler(QuizStore store)
        {
            m_Store = store;
        }

        public void Subscribe(IEventSubscribable subscribable)
        {
            subscribable.Subscribe<StudentAnswerMessage>(OnStudentAnswered);
            subscribable.Subscribe<CustomDataMessage>(OnCustomDataReceived);
        }

        public void Unsubscribe(IEventSubscribable subscribable)
        {
            subscribable.Unsubscribe<StudentAnswerMessage>(OnStudentAnswered);
            subscribable.Unsubscribe<CustomDataMessage>(OnCustomDataReceived);
        }

        private void OnStudentAnswered(NetworkConnectionToClient connection, StudentAnswerMessage studentAnswerMessage)
        {
            var stationId = studentAnswerMessage.StationId;
            var answer = studentAnswerMessage.Answer;

            m_Store.SetStudentAnswer(stationId, answer);
        }

        private void OnCustomDataReceived(NetworkConnectionToClient connection, CustomDataMessage customDataMessage)
        {
            m_Store.SetCustomData(customDataMessage.Data);
        }
    }
}