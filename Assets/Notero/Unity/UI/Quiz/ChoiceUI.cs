using DataStore.Quiz;
using UnityEngine;

namespace Notero.Unity.UI.Quiz
{
    public class ChoiceUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_ChartBarCorrect;

        [SerializeField]
        private GameObject m_ChartBarWrong;

        public void SetIsCorrectChoice(bool isCorrect)
        {
            m_ChartBarCorrect.gameObject.SetActive(isCorrect);
            m_ChartBarWrong.gameObject.SetActive(!isCorrect);
        }
    }
}