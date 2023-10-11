using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hendrix.Generic.UI.Elements
{
    public class PanelHeaderToggle : MonoBehaviour
    {
        [SerializeField]
        private Image m_HeaderPanelImage;

        [SerializeField]
        private TMP_Text m_HeaderText;

        [SerializeField]
        private HeaderToggleOption m_DefaultToggleOption;

        [SerializeField]
        private HeaderToggleOption m_WarningToggleOption;

        public void SetToDefault() => SetHeaderOption(m_DefaultToggleOption);

        public void SetToWarning() => SetHeaderOption(m_WarningToggleOption);

        private void SetHeaderOption(HeaderToggleOption option)
        {
            m_HeaderPanelImage.sprite = option.Sprite;
            m_HeaderText.color = option.TextColor;
        }

        [Serializable]
        protected class HeaderToggleOption
        {
            [SerializeField]
            private Sprite m_Sprite;

            [SerializeField]
            private Color m_TextColor;

            public Sprite Sprite => m_Sprite;

            public Color TextColor => m_TextColor;
        }
    }
}