using System.Collections.Generic;
using UnityEngine.Events;

namespace Notero.QuizConnector.Instructor
{
    public abstract class BaseInstructorPostTestStudentListResult<T> : BaseQuizPanel
    {
        public UnityEvent<string> OnSwapResultState;

        protected List<T> ElementListInfo { get; private set; }

        public virtual void SetElementListInfo(List<T> list) => ElementListInfo = list;
    }
}