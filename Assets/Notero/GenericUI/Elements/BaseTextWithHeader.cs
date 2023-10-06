using TMPro;
using UnityEngine;

namespace Hendrix.Generic.UI.Elements
{
    public class BaseTextWithHeader : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text m_HeaderText;

        [SerializeField]
        private TMP_Text m_BodyText;

        public void SetHeader(string text)
        {
            m_HeaderText.text = text;
        }

        public void SetBody(string text)
        {
            m_BodyText.text = text;
        }
    }
}