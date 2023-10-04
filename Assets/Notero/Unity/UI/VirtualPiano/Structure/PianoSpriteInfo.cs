using System;
using System.Linq;
using UnityEngine;

namespace Notero.Unity.UI.VirtualPiano.Structure
{
    [Serializable]
    public class PianoSpriteCollection
    {
        public PianoSpriteInfo[] SpriteStates;

        public void SetSprite(string state, Handside hand, bool isPress, Sprite overrideSprite)
        {
            PianoSpriteInfo spriteWithKey = GetSpriteWithKey(state, hand, isPress);

            if(spriteWithKey != default)
            {
                spriteWithKey.Sprite = overrideSprite;
            }
        }

        public Sprite GetSprite(string state, Handside hand, bool isPress)
        {
            return GetSpriteWithKey(state, hand, isPress).Sprite;
        }

        private PianoSpriteInfo GetSpriteWithKey(string state, Handside hand, bool isPress)
        {
            return SpriteStates.FirstOrDefault(x => x.State == state && x.Hand == hand && x.IsPressing == isPress);
        }

        public PianoSpriteCollection Clone()
        {
            return new PianoSpriteCollection
            {
                SpriteStates = SpriteStates
                    .Select(x => new PianoSpriteInfo(x.State, x.Hand, x.IsPressing, x.Sprite)).ToArray()
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

        public PianoSpriteInfo(string state, Handside hand, bool isPressing, Sprite sprite)
        {
            State = state;
            Hand = hand;
            IsPressing = isPressing;
            Sprite = sprite;
        }
    }
}