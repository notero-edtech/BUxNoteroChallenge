using UnityEngine;
using UnityEngine.UI;

namespace Hendrix.Generic.UI.Elements
{
    public class TextToggle : Toggle
    {
        [SerializeField]
        private BaseToggleText m_PositiveToggleText;

        [SerializeField]
        private BaseToggleText m_NegativeToggleText;

        protected override void Awake()
        {
            base.Awake();
            transition = Transition.None;
            onValueChanged.AddListener(OnToggleValueChanged);
        }

        public virtual void OnToggleValueChanged(bool isOn)
        {
            if(m_PositiveToggleText != null)
            {
                m_PositiveToggleText.gameObject.SetActive(isOn);
            }

            if(m_NegativeToggleText != null)
            {
                m_NegativeToggleText.gameObject.SetActive(!isOn);
            }
        }

        public void SetAvailable(bool isAvailable) => isOn = isAvailable;

        public void SetPositiveToggleText(string info) => m_PositiveToggleText.SetInfoText(info);

        public void SetNegativeToggleText(string info) => m_NegativeToggleText.SetInfoText(info);
    }
}
