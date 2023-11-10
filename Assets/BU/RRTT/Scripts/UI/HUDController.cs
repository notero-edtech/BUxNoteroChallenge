using Notero.Unity.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace BU.RRTT.QuizExample.Scripts.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField]
        private Button m_NextSequenceButton;

        public UnityEvent OnNextClick;

        private Coroutine m_NextQuizDelayCoroutine;

        public void Awake()
        {
            m_NextSequenceButton.onClick.AddListener(OnNextSequenceButtonClickHandler);
        }

        public void OnDestroy()
        {
            m_NextSequenceButton.onClick.RemoveListener(OnNextSequenceButtonClickHandler);
        }

        private void OnNextSequenceButtonClickHandler()
        {
            if(m_NextQuizDelayCoroutine != null) return;

            m_NextQuizDelayCoroutine = StartCoroutine(NextQuizDelay());
        }

        private IEnumerator NextQuizDelay()
        {
            yield return new WaitForSeconds(0.5f);

            m_NextQuizDelayCoroutine = null;
            OnNextClick?.Invoke();
        }
    }
}