using System;
using UnityEngine;
using UnityEngine.UI;

namespace Hendrix.Generic.UI.Elements
{
    public class SpriteToggle : MonoBehaviour
    {
        [Header("(state 0: false), (state 1: true)")]
        [SerializeField]
        private SpriteToggleOption[] m_TargetImages;

        /// <summary>
        /// If isActive, sprite will be set to state 1. 
        /// If not isActive, sprite will be set to state 0.
        /// </summary>
        /// <param name="isActive"></param>
        public void SetActive(bool isActive)
        {
            int state = Convert.ToInt32(isActive);
            SetSpriteState(state);
        }

        public void SetSpriteState(int state)
        {
            foreach (SpriteToggleOption option in m_TargetImages)
            {
                option.SetSprite(state);
            }
        }

        [Serializable]
        protected class SpriteToggleOption
        {
            public Image TargetImage;
            public Sprite[] Sprites;

            public int LastState => Sprites.Length - 1;

            public void SetSprite(int state)
            {
                if (state > LastState || state < 0)
                {
                    Debug.LogError($"[{nameof(SpriteToggleOption)}] Cannot set state {state}");
                    return;
                }

                TargetImage.sprite = Sprites[state];
            }
        }
    }
}