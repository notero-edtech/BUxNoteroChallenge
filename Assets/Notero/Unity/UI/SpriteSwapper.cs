using UnityEngine;
using UnityEngine.UI;

namespace Notero.Unity.UI
{
    /// <summary>
    /// A component that change the target image sprite when an observed button changes its selection state
    /// </summary>
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("Notero/UI/SpriteSwapper", 1)]
    public class SpriteSwapper : ButtonObserver
    {
        // Sprites used for a Image swap-based transition.
        [SerializeField]
        private SpriteStatePreset m_SpriteStatePreset;

        protected Image TargetGraphic
        {
            get
            {
                if(m_TargetGraphic == null)
                {
                    m_TargetGraphic = GetComponent<Image>();
                }
                return m_TargetGraphic;
            }
        }
        private Image m_TargetGraphic;

        protected override void OnSelectionStateChanged(SelectionState state)
        {
            TargetGraphic.sprite = m_SpriteStatePreset[state];
        }
    }
}
