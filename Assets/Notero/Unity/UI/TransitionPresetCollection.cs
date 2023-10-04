using UnityEngine;

namespace Notero.Unity.UI
{
    /// <summary>
    /// ScriptableObject that stores the collection of state transitions on a Selectable.
    /// </summary>
    public class TransitionPresetCollection<TPreset, TState, TValue> : ScriptableObject
        where TPreset : TransitionPreset<TState, TValue>
        where TState : TransitionState<TValue>
    {
        [SerializeField]
        private TPreset[] m_Presets;

        public TState this[int i]
        {
            get { return m_Presets[i].State; }
        }
    }
}
