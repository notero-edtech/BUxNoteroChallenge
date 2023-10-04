using UnityEngine;

namespace Notero.Unity.UI
{
    /// <summary>
    /// ScriptableObject that stores the collection of sprite transitions on a Selectable.
    /// </summary>
    [CreateAssetMenu(fileName = "SpriteStateCollectionPreset", menuName = "Notero/UI/SpriteStateCollectionPreset", order = 2)]
    public class SpriteStateCollectionPreset : TransitionPresetCollection<SpriteStatePreset, SpriteState, Sprite> { }
}
