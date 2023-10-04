using UnityEngine.Events;

namespace Notero.Unity.UI
{
    /// <summary>
    /// An extended button that sends an event when clicked, selection state changed and variant changed
    /// </summary>    
    public class TransformableButton : Button
    {
        /// <summary>
        /// Current variant
        /// </summary>
        public int CurrentVariant
        {
            get => m_Variant;
            set
            {
                if(m_Variant == value)
                    return;

                m_Variant = value;
                OnVariantChanged?.Invoke(value);
            }
        }
        private int m_Variant;

        /// <summary>
        /// Function definition for a variant changed event.
        /// </summary>
        public UnityEvent<int> OnVariantChanged = new UnityEvent<int>();
    }
}
