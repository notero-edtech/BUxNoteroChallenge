using System.Collections.Generic;
using DataStore.Quiz;
using UnityEngine;
using UnityEngine.Events;

namespace Notero.QuizConnector
{
    public interface IQuizController
    {
        public QuizStore QuizStore { get; set; }

        public UnityEvent OnNextStateReceive { get; set; }

        public UnityEvent<string> OnStudentSubmit { get; set; }

        public UnityEvent<byte[]> OnCustomDataReceive { get; set; }

        public void Init(Transform container, QuizStore quizStore);

        public void OnStudentAnswerReceive(string stationId, string answer, int answerStudentAmount, int studentAmount);

        public void OnCustomDataMessageReceive(byte[] data);

        #region instructor loading state

        public void LoadQuizToQuizStore();

        #endregion

        #region instructor countIn state

        public void SpawnInstructorCountInStateUI();

        public void DestroyInstructorCountInStateUI();

        public void AddInstructorFinishCountInStateEventListener(UnityAction action);

        public void RemoveInstructorFinishCountInStateEventListener(UnityAction action);

        #endregion

        #region instructor Question state

        public void SpawnInstructorQuestionStateUI();

        public void DestroyInstructorQuestionStateUI();

        #endregion

        #region instructor preresult state

        public void SpawnInstructorPreResultStateUI();

        public void DestroyInstructorPreResultStateUI();

        #endregion

        #region instructor end state

        public void SpawnInstructorEndStateUI();

        public void DestroyInstructorEndStateUI();

        #endregion

        #region instructor result state

        public void SpawnInstructorResultStateUI(string mode);

        public void DestroyInstructorResultStateUI(string mode);

        #endregion

        #region student count in state

        public void SpawnStudentCountInStateUI();

        public void DestroyStudentCountInStateUI();

        #endregion

        #region student question state

        public void SpawnStudentQuestionStateUI();

        public void DestroyStudentQuestionStateUI();

        public void SpawnStudentSubmitedStateUI();

        public void DestroyStudentSubmitedStateUI();

        public void AddStudentSubmitedQuestionStateEventListener(UnityAction action);

        public void RemoveStudentSubmitedQuestionStateEventListener(UnityAction action);

        #endregion

        #region student preresult state

        public void SpawnStudentPreResultStateUI();

        public void DestroyStudentPreResultStateUI();

        #endregion

        #region student end state

        public void SpawnStudentEndStateUI();

        public void DestroyStudentEndStateUI();

        #endregion

        #region student result state

        public void SpawnStudentResultStateUI(string mode);

        public void DestroyStudentResultStateUI(string mode);

        #endregion
    }
}