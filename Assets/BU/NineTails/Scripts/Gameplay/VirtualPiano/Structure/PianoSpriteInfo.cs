using Notero.Unity.UI.VirtualPiano;
using System;
using System.Linq;
using UnityEngine;

namespace BU.NineTails.Scripts.UI.VirtualPiano.Structure
{
    [Serializable]
    public class VirtualKeySpriteCollection
    {
        public PianoSpriteInfo[] SpriteStates;

        public void SetSprite(string state, Handside hand, bool isPress, Sprite overrideSprite, int note)
        {
            PianoSpriteInfo spriteWithKey = GetSpriteWithKey(state, hand, isPress, note);

            if (spriteWithKey != default)
            {
                spriteWithKey.Sprite = overrideSprite;
            }
        }

        public Sprite GetSprite(string state, Handside hand, bool isPress, int note)
        {
            return GetSpriteWithKey(state, hand, isPress, note).Sprite;
        }

        private PianoSpriteInfo GetSpriteWithKey(string state, Handside hand, bool isPress, int note)
        {
            return SpriteStates.FirstOrDefault(x => x.State == state && x.Hand == hand && x.IsPressing == isPress && x.Note == note);
        }

        public VirtualKeySpriteCollection Clone()
        {
            return new VirtualKeySpriteCollection
            {
                SpriteStates = SpriteStates
                    .Select(x => new PianoSpriteInfo(x.State, x.Hand, x.IsPressing, x.Sprite, x.Note)).ToArray()
            };
        }
    }

    [Serializable]
    public class PianoSpriteInfo
    {
        public string State;
        public Handside Hand;
        public bool IsPressing;
        public Sprite Sprite;
        public int Note;

        public PianoSpriteInfo(string state, Handside hand, bool isPressing, Sprite sprite, int note)
        {
            State = state;
            Hand = hand;
            IsPressing = isPressing;
            Sprite = sprite;
            Note = note;
        }
    }
}