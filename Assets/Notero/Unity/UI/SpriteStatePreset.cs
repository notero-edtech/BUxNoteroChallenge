using UnityEngine;

namespace Notero.Unity.UI
{
    /// <summary>
    /// ScriptableObject that stores the state of a sprite transition on a Selectable.
    /// </summary>
    [CreateAssetMenu(fileName = "SpriteStatePreset", menuName = "Notero/UI/SpriteStatePreset", order = 1)]
    public class SpriteStatePreset : TransitionPreset<SpriteState, Sprite> { }
}
