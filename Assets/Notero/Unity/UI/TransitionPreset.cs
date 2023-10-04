using UnityEngine;

namespace Notero.Unity.UI
{
    /// <summary>
    /// ScriptableObject that stores the state of a transition on a Selectable.
    /// </summary>
    public abstract class TransitionPreset<TState, TValue> : ScriptableObject
        where TState : TransitionState<TValue>
    {
        [SerializeField]
        private TState m_State;

        public TState State => m_State;

        public TValue Normal => m_State.Normal;

        public TValue Highlighted => m_State.Highlighted;

        public TValue Pressed => m_State.Pressed;

        public TValue Selected => m_State.Selected;

        public TValue Disabled => m_State.Disabled;

        public TValue this[SelectionState selectionState] => m_State[selectionState];
    }
}
