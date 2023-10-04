using System;
using UnityEngine;

namespace Notero.Unity.UI
{
    /// <summary>
    /// Class that stores the collection of state transitions on a Selectable.
    /// </summary>
    [Serializable]
    public class TransitionStateCollection<TState, TValue>
        where TState : TransitionState<TValue>
    {
        [SerializeField]
        private TState[] m_States;

        public int Count => m_States.Length;

        public TState this[int i]
        {
            get { return m_States[i]; }
        }
    }
}
