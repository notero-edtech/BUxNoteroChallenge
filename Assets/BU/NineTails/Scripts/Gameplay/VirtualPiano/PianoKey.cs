using BU.NineTails.Scripts.UI.VirtualPiano.Structure;
using Notero.Unity.UI.VirtualPiano;
using UnityEngine;
using UnityEngine.UI;

namespace BU.NineTails.Scripts.UI.VirtualPiano
{
    public class PianoKey : MonoBehaviour
    {
        [SerializeField]
        private Image m_PianoKeyImage;

        private VirtualKeySpriteCollection m_LocalPianoInfo;

        public void SetupPianoSpriteCollection(VirtualKeySpriteCollection spriteCollection)
        {
            m_LocalPianoInfo = spriteCollection;
        }

        public void SetSprite(string state, Handside handSide, bool isPressing, int note)
        {
            m_PianoKeyImage.sprite = m_LocalPianoInfo.GetSprite(state, handSide, isPressing, note);
        }
    }
}