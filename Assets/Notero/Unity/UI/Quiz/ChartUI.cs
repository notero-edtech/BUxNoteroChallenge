using DataStore.Quiz;
using TMPro;
using UnityEngine;

namespace Notero.Unity.UI.Quiz
{
    public class ChartUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text m_AnswerAmount;

        [SerializeField]
        private GameObject m_ChartBar;

        [SerializeField]
        private GameObject m_ChartBarCorrect;

        [SerializeField]
        private GameObject m_ChartBarWrong;

        private float m_ChartHeightMax => 200; // TODO: get height from game object

        private const int MinHeightChartBar = 16; // is 16px

        public void SetChartBar(float chartPercent)
        {
            var minHeightChartBarPercent = (MinHeightChartBar / m_ChartHeightMax) * 100;

            if(chartPercent != 0 && chartPercent < minHeightChartBarPercent)
            {
                chartPercent = minHeightChartBarPercent;
            }

            var chartHeight = chartPercent * m_ChartHeightMax / 100;
            var rectTransForm = m_ChartBar.GetComponent<RectTransform>();

            rectTransForm.sizeDelta = chartHeight > m_ChartHeightMax ? new Vector2(0, m_ChartHeightMax) : new Vector2(0, chartHeight);
        }

        public void SetAnswerAmount(int amount) => m_AnswerAmount.text = amount.ToString();

        public void SetIsCorrectCharBar(bool isCorrect)
        {
            m_ChartBarCorrect.gameObject.SetActive(isCorrect);
            m_ChartBarWrong.gameObject.SetActive(!isCorrect);
        }
    }
}