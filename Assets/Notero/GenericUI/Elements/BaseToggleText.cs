using TMPro;
using UnityEngine;

namespace Hendrix.Generic.UI.Elements
{
    public class BaseToggleText : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text m_InfoText;

        public void SetInfoText(string info) => m_InfoText.text = info;
    }
}
