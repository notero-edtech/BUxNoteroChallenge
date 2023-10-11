using UnityEngine;

namespace Hendrix.Generic.UI.Elements
{
    public class BaseToggleGameObject : MonoBehaviour
    {
        [SerializeField] private GameObject m_FillGo;

        public virtual void Set(bool active)
        {
            m_FillGo.SetActive(active);
        }
    }
}