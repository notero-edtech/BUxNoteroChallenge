using System.Collections.Generic;

namespace Notero.QuizConnector.Instructor
{
    public abstract class BaseInstructorPopQuizResult<T> : BaseQuizPanel
    {
        protected List<T> ElementListInfo { get; private set; }

        public virtual void SetElementListInfo(List<T> list) => ElementListInfo = list;
    }
}