using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Notero.RaindropGameplay.UI
{
    public class CountInPanel : MonoBehaviour
    {
        // TODO: will be changed to an animation
        [SerializeField]
        private TMP_Text m_CountInText;

        [SerializeField]
        private double m_CountInDuration;

        [SerializeField]
        private double m_CountInGap;

        public event Action OnCountInFinished;

        private const string m_NumberFormat = "{0}";
        private const string m_StartText = "START";

        public void SetActive(bool isActive) => gameObject.SetActive(isActive);

        public void StartCount()
        {
            StartCoroutine(CountIn());
        }

        public void SetCountInDuration(double countInDuration, double gap = 1d)
        {
            m_CountInDuration = countInDuration;
            m_CountInGap = gap;
        }

        private IEnumerator CountIn()
        {
            for(int i = 1; i <= m_CountInDuration; i++)
            {
                bool isLastCount = i == m_CountInDuration;
                string text = isLastCount ? m_StartText
                                          : string.Format(m_NumberFormat, i);
                m_CountInText.text = text;
                yield return new WaitForSeconds((float)m_CountInGap);
            }

            // TODO: find reason why this line is not called
            OnCountInFinished?.Invoke();
        }
    }
}