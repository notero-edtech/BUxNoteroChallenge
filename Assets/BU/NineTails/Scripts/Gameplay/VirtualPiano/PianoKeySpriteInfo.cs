using BU.NineTails.Scripts.UI.VirtualPiano.Structure;
using Notero.Unity.UI.VirtualPiano;
using UnityEngine;

namespace BU.NineTails.Scripts.UI.VirtualPiano
{
    [CreateAssetMenu(fileName = "newVirtualPianoKeySpriteInfo", menuName = "Template/VirtualPianoKeySpriteInfo")]
    public class PianoKeySpriteInfo : ScriptableObject
    {
        public int OctaveInputAmount;
        public int MinimumKeyGiven;
        public VirtualKeySpriteCollection[] SpriteCollection = new VirtualKeySpriteCollection[2];
        public OverridedPianoSpriteInfo[] OverrideSprite;

        public int MaximumKeyGiven => MinimumKeyGiven + (VirtualPianoHelper.OctaveTotalKeys * OctaveInputAmount);
    }
}