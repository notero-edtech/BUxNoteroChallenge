using UnityEngine;
using UnityEngine.UI;

namespace Notero.Unity.UI
{
    /// <summary>
    /// A component that change the target image sprite when an observed TransformableButton changes its selection state
    /// </summary>
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("Notero/UI/TransformableSpriteSwapper", 2)]
    public class TransformableSpriteSwapper : TransformableButtonObserver
    {
        // Sprites used for a Image swap-based transition.
        [SerializeField]
        private SpriteStateCollectionPreset m_SpriteStateCollectionPreset;

        private Image m_TargetGraphic;

        protected override void Awake()
        {
            base.Awake();
            m_TargetGraphic = GetComponent<Image>();
        }

        protected override void OnSelectionStateChanged(SelectionState state)
        {
            ApplyChange(state, m_ObservedButton.CurrentVariant);
        }

        protected override void OnVariantChanged(int variant)
        {
            ApplyChange(m_ObservedButton.CurrentSelectionState, variant);
        }

        private void ApplyChange(SelectionState state, int variant)
        {
            if(m_TargetGraphic == null || m_SpriteStateCollectionPreset == null || m_SpriteStateCollectionPreset[variant] == null)
                return;

            m_TargetGraphic.sprite = m_SpriteStateCollectionPreset[variant][state];
        }
    }
}
