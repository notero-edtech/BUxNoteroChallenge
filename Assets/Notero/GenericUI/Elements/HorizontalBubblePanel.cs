using UnityEngine;

namespace Hendrix.Generic.UI.Elements
{
    public class HorizontalBubblePanel : MonoBehaviour
    {
        private const int m_FromLeftDegree = 180;
        private const int m_FromRightDegree = 0;

        private RectTransform m_Rect => (RectTransform)transform;

        public void FromLeft() => m_Rect.rotation = Quaternion.Euler(0, m_FromLeftDegree, 0);

        public void FromRight() => m_Rect.rotation = Quaternion.Euler(0, m_FromRightDegree, 0);
    }
}