using UnityEngine.Events;

namespace Notero.QuizConnector.Instructor
{
    public abstract class BaseInstructorCountIn : BaseQuizPanel
    {
        public UnityEvent OnNextState;

        public abstract void OnCountdownSet(int count);
    }
}