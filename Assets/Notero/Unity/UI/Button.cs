using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Notero.Unity.UI
{
    /// <summary>
    /// An extended button that sends an event when clicked and sends an event when selection state changed
    /// </summary>
    [AddComponentMenu("Notero/UI/Button", 1)]
    public class Button : UnityEngine.UI.Button
    {
        /// <summary>
        /// Current selection state
        /// </summary>
        public UI.SelectionState CurrentSelectionState => GetSelectionState(currentSelectionState);

        /// <summary>
        /// Function definition for a selection state changed event.
        /// </summary>
        public UnityEvent<UI.SelectionState> OnSelectionStateChanged = new UnityEvent<UI.SelectionState>();

        protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            var selectionState = GetSelectionState(state);
            OnSelectionStateChanged?.Invoke(selectionState);
        }

        private UI.SelectionState GetSelectionState(Selectable.SelectionState state)
        {
            switch(state)
            {
                case Selectable.SelectionState.Highlighted:
                    return UI.SelectionState.Highlighted;
                case Selectable.SelectionState.Pressed:
                    return UI.SelectionState.Pressed;
                case Selectable.SelectionState.Selected:
                    return UI.SelectionState.Selected;
                case Selectable.SelectionState.Disabled:
                    return UI.SelectionState.Disabled;
                default:
                    return UI.SelectionState.Normal;
            }
        }
    }
}