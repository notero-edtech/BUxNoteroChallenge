using Notero.QuizConnector.Model;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Notero.QuizConnector.Instructor
{
    public abstract class BaseAnswerListItem<T> : BaseQuizPanel
    {
        public UnityEvent<T> OnClicked;

        public T ElementInfo { get; private set; }

        public virtual void SetElementInfo(T info)
        {
            ElementInfo = info;
        }
    }
}