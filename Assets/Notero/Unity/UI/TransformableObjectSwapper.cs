using System;
using UnityEngine;

namespace Notero.Unity.UI
{
    /// <summary>
    /// A component that switch activation of GameObjects when an observed TransformableButton changes its selection state
    /// </summary>
    [Serializable]
    [AddComponentMenu("Notero/UI/TransformableObjectSwapper", 3)]
    public class TransformableObjectSwapper : TransformableButtonObserver
    {
        // Objects used for object swap-based transition.
        [SerializeField]
        private ObjectState[] m_ObjectStates;

        private SelectionState[] m_SelectionStates;

        protected override void Awake()
        {
            m_SelectionStates = (SelectionState[])Enum.GetValues(typeof(SelectionState));
            base.Awake();
        }

        protected override void OnSelectionStateChanged(SelectionState state)
        {
            ApplyChange(state, m_ObservedButton.CurrentVariant);
        }

        protected override void OnVariantChanged(int variant)
        {
            ApplyChange(m_ObservedButton.CurrentSelectionState, variant);
        }

        private void ApplyChange(SelectionState currentState, int variant)
        {
            foreach(var objectState in m_ObjectStates)
            {
                foreach(var state in m_SelectionStates)
                {
                    objectState[state].SetActive(false);
                }
            }

            m_ObjectStates[variant][currentState].SetActive(true);
        }
    }
}
