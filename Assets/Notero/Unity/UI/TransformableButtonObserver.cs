namespace Notero.Unity.UI
{
    /// <summary>
    /// A component that receives selection state changed events and variant changed events from TransformableButton
    /// </summary>
    public abstract class TransformableButtonObserver : ButtonObserverBase<TransformableButton>
    {
        protected override void Awake()
        {
            base.Awake();

            if(m_ObservedButton != null)
                m_ObservedButton.OnVariantChanged.AddListener(OnVariantChanged);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(m_ObservedButton != null)
                m_ObservedButton.OnVariantChanged.RemoveListener(OnVariantChanged);
        }

        protected abstract void OnVariantChanged(int variant);
    }
}
