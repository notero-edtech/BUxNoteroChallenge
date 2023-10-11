using UnityEngine;

namespace Hendrix.Generic.UI.Elements
{
    public class StarsContainer : MonoBehaviour
    {
        [SerializeField]
        private BaseToggleGameObject[] m_ToggledObjects;

        public void SetActiveAmount(int amount)
        {
            for(int i = 0; i < m_ToggledObjects.Length; i++)
            {
                m_ToggledObjects[i].Set(i < amount);
            }
        }

        public void SetStarPosition(int index, Vector2 position)
        {
            if(index < 0 || index > m_ToggledObjects.Length) return;
            ((RectTransform)m_ToggledObjects[index].transform).anchoredPosition = position;
        }
    }
}