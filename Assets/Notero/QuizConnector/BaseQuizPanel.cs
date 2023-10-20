using UnityEngine;
using UnityEngine.Events;

namespace Notero.QuizConnector
{
    public abstract class BaseQuizPanel : MonoBehaviour
    {
        public UnityEvent<byte[]> OnSendCustomData;

        public UnityEvent OnDestroyed;

        protected byte[] CustomData { get; private set; }

        protected string Chapter { get; private set; }

        protected string Mission { get; private set; }

        protected string QuizMode { get; private set; }

        protected int CurrentPage { get; private set; }

        protected int TotalPage { get; private set; }

        public virtual void SetChapter(string chapter) => Chapter = chapter;

        public virtual void SetMission(string mission) => Mission = mission;

        public virtual void SetQuizMode(string quizMode) => QuizMode = quizMode;

        public virtual void SetCurrentPage(int currentPage) => CurrentPage = currentPage;

        public virtual void SetTotalPage(int totalpage) => TotalPage = totalpage;

        private void OnDestroy() => OnDestroyed?.Invoke();

        public virtual void OnCustomDataReceive(byte[] data) => CustomData = data;
    }
}