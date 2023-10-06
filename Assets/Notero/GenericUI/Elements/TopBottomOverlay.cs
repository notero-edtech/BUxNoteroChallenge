using UnityEngine;

namespace Hendrix.Generic.UI.Elements
{
    public class TopBottomOverlay : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_TopOverlay;

        [SerializeField]
        private GameObject m_BottomOverlay;

        [SerializeField]
        private GameObject m_LeftButtonContainer;

        [SerializeField]
        private GameObject m_RightButtonContainer;

        public enum ButtonContainerSide
        {
            Left,
            Right,
            Both
        }

        public void SetTopOverlayActive(bool isActive) => m_TopOverlay.SetActive(isActive);

        public void SetBottomOverlayActive(bool isActive) => m_BottomOverlay.SetActive(isActive);

        public void SetButtonContainerActive(ButtonContainerSide side, bool isActive)
        {
            switch (side)
            {
                case ButtonContainerSide.Left:
                    m_LeftButtonContainer.SetActive(isActive);
                    break;
                case ButtonContainerSide.Right:
                    m_RightButtonContainer.SetActive(isActive);
                    break;
                default:
                    m_LeftButtonContainer.SetActive(isActive);
                    m_RightButtonContainer.SetActive(isActive);
                    break;
            }
        }
    }
}