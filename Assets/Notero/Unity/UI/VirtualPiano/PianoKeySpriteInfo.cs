using Notero.Unity.UI.VirtualPiano.Structure;
using UnityEngine;

namespace Notero.Unity.UI.VirtualPiano
{
    [CreateAssetMenu(fileName = "newVirtualPianoInfo", menuName = "Template/VirtualPiano")]
    public class PianoKeySpriteInfo : ScriptableObject
    {
        public int OctaveInputAmount;
        public int MinimumKeyGiven;
        public PianoSpriteCollection[] SpriteCollection = new PianoSpriteCollection[2];
        public OverridedPianoSpriteInfo[] OverrideSprite;

        public int MaximumKeyGiven => MinimumKeyGiven + (VirtualPianoHelper.OctaveTotalKeys * OctaveInputAmount);
    }
}