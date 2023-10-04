using Notero.Unity.UI.VirtualPiano.Structure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Notero.Unity.UI.VirtualPiano
{
    public class PianoKey : MonoBehaviour
    {
        [SerializeField]
        private Image m_PianoKeyImage;

        [SerializeField]
        private TMP_Text ToneLabel;

        private PianoSpriteCollection m_LocalPianoInfo;

        public void SetupPianoSpriteCollection(PianoSpriteCollection spriteCollection)
        {
            m_LocalPianoInfo = spriteCollection;
        }

        public void SetSprite(string state, Handside handSide, bool isPressing)
        {
            m_PianoKeyImage.sprite = m_LocalPianoInfo.GetSprite(state, handSide, isPressing);
        }

        public void ShowLabel(bool showLabel)
        {
            if(ToneLabel == null) return;
            ToneLabel.gameObject.SetActive(showLabel);
        }

        public void SetToneLabel(string toneName)
        {
            if(ToneLabel == null) return;
            ToneLabel.text = toneName;
            ShowLabel(false);
        }
    }
}