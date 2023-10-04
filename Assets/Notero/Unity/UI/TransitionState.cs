using System;
using UnityEngine;

namespace Notero.Unity.UI
{
    /// <summary>
    /// Class that stores the state of a transition on a Selectable.
    /// </summary>
    [Serializable]
    public class TransitionState<TValue>
    {
        [SerializeField]
        private TValue m_Normal;

        [SerializeField]
        private TValue m_Highlighted;

        [SerializeField]
        private TValue m_Pressed;

        [SerializeField]
        private TValue m_Selected;

        [SerializeField]
        private TValue m_Disabled;

        public TValue Normal { get { return m_Normal; } set { m_Normal = value; } }

        public TValue Highlighted { get { return m_Highlighted; } set { m_Highlighted = value; } }

        public TValue Pressed { get { return m_Pressed; } set { m_Pressed = value; } }

        public TValue Selected { get { return m_Selected; } set { m_Selected = value; } }

        public TValue Disabled { get { return m_Disabled; } set { m_Disabled = value; } }

        public TValue this[SelectionState selectionState]
        {
            get
            {
                switch(selectionState)
                {
                    case SelectionState.Highlighted:
                        return Highlighted;
                    case SelectionState.Pressed:
                        return Pressed;
                    case SelectionState.Selected:
                        return Selected;
                    case SelectionState.Disabled:
                        return Disabled;
                    default:
                        return Normal;
                }
            }
        }
    }
}
