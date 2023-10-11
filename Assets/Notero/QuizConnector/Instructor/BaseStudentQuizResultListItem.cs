namespace Notero.QuizConnector.Instructor
{
    public abstract class BaseStudentQuizResultListItem<T> : BaseQuizPanel
    {
        public T ElementInfo { get; private set; }

        public virtual void SetElementInfo(T info)
        {
            ElementInfo = info;
        }
    }
}