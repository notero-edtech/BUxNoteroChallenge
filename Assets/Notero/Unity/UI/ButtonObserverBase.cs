using UnityEngine;

namespace Notero.Unity.UI
{
    /// <summary>
    /// A component that receives selection state changed events
    /// </summary>
    /// <typeparam name="T">Type of button which to be observed</typeparam>
    [ExecuteInEditMode]
    public abstract class ButtonObserverBase<T> : MonoBehaviour
        where T : Button
    {
        [SerializeField]
        protected T m_ObservedButton;

        protected virtual void Awake()
        {
            if(m_ObservedButton != null)
                m_ObservedButton.OnSelectionStateChanged.AddListener(OnSelectionStateChanged);
            OnSelectionStateChanged(m_ObservedButton.CurrentSelectionState);
        }

        protected virtual void OnDestroy()
        {
            if(m_ObservedButton != null)
                m_ObservedButton.OnSelectionStateChanged.RemoveListener(OnSelectionStateChanged);
        }

        protected abstract void OnSelectionStateChanged(SelectionState state);
    }
}
