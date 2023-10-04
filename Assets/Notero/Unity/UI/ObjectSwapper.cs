using System;
using UnityEngine;

namespace Notero.Unity.UI
{
    /// <summary>
    /// A component that switch activation of GameObjects when an observed button changes its selection state
    /// </summary>
    [Serializable]
    [AddComponentMenu("Notero/UI/ObjectSwapper", 2)]
    public class ObjectSwapper : ButtonObserver
    {
        // Objects used for object swap-based transition.
        [SerializeField]
        private ObjectState m_ObjectState;

        private SelectionState[] m_SelectionStates;

        protected override void Awake()
        {
            m_SelectionStates = (SelectionState[])Enum.GetValues(typeof(SelectionState));
            base.Awake();
        }

        protected override void OnSelectionStateChanged(SelectionState currentState)
        {
            foreach(var state in m_SelectionStates)
            {
                m_ObjectState[state].SetActive(false);
            }
            m_ObjectState[currentState].SetActive(true);
        }
    }
}
