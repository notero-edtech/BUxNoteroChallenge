using BU.NineTails.Scripts.UI.VirtualPiano.Structure;
using Notero.Unity.UI.VirtualPiano;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BU.NineTails.Scripts.UI.VirtualPiano
{
    public class VirtualPianoSpawner : MonoBehaviour
    {
        [Header("m_Keys")]
        [SerializeField]
        protected PianoKey C_WhiteKeySeed;

        [SerializeField]
        protected PianoKey D_WhiteKeySeed;

        [SerializeField]
        protected PianoKey E_WhiteKeySeed;

        [SerializeField]
        protected PianoKey F_WhiteKeySeed;

        [SerializeField]
        protected PianoKey G_WhiteKeySeed;

        [SerializeField]
        protected PianoKey A_WhiteKeySeed;

        [SerializeField]
        protected PianoKey B_WhiteKeySeed;

        [SerializeField]
        protected PianoKey m_BlackKeySeed;

        [SerializeField]
        protected RectTransform m_WhiteLayer;

        [SerializeField]
        protected RectTransform m_BlackLayer;

        private PianoKeySpriteInfo m_PianoInfo;
        protected List<VirtualKeySpriteCollection> m_PianoKeySpriteCollection;
        protected List<PianoKey> m_Keys { get; } = new List<PianoKey>();
        protected int m_InputOctaves;
        private string m_PianoType;

        public Rect C_WhiteKeyRect => ((RectTransform)C_WhiteKeySeed.transform).rect;
        public Rect D_WhiteKeyRect => ((RectTransform)D_WhiteKeySeed.transform).rect;
        public Rect E_WhiteKeyRect => ((RectTransform)E_WhiteKeySeed.transform).rect;
        public Rect F_WhiteKeyRect => ((RectTransform)F_WhiteKeySeed.transform).rect;
        public Rect G_WhiteKeyRect => ((RectTransform)G_WhiteKeySeed.transform).rect;
        public Rect A_WhiteKeyRect => ((RectTransform)A_WhiteKeySeed.transform).rect;
        public Rect B_WhiteKeyRect => ((RectTransform)B_WhiteKeySeed.transform).rect;
        public Rect BlackKeyRect => ((RectTransform)m_BlackKeySeed.transform).rect;

        public virtual List<PianoKey> Create(string pianoType)
        {
            m_PianoType = pianoType;
            m_PianoInfo = GetPianoInfoStorage(m_PianoType.ToString());
            m_InputOctaves = m_PianoInfo.OctaveInputAmount;
            CreateSpriteCollection();
            InitializeKeys();
            return m_Keys;
        }

        public int MinimumKey => m_PianoInfo.MinimumKeyGiven;
        public int MaximumKey => m_PianoInfo.MaximumKeyGiven;

        public bool IsMidiIdInRange(int midiId)
        {
            var maximumKey = (MinimumKey + m_Keys.Count) - 1;

            return midiId >= MinimumKey && midiId <= maximumKey;
        }

        #region Loaders

        protected virtual PianoKeySpriteInfo GetPianoInfoStorage(string pianoTypeStr)
        {
            return Resources.Load<PianoKeySpriteInfo>($"ScriptableObject/VirtualPianoKeySpriteInfo_{pianoTypeStr}");
        }

        #endregion

        #region Init

        protected virtual void InitializeKeys()
        {
            float containerSize = ((RectTransform)m_WhiteLayer.transform).rect.width;
            float whiteKeySize = ((RectTransform)C_WhiteKeySeed.transform).rect.width;
            float blackKeySize = ((RectTransform)m_BlackKeySeed.transform).rect.width;

            List<float> lanePosXList = VirtualPianoHelper.GetLanePosition(containerSize, whiteKeySize, blackKeySize, m_InputOctaves);

            for (int i = 0; i < lanePosXList.Count;)
            {
                string note = VirtualPianoHelper.GetNoteName(i);
                PianoKey whiteKeySeed = GetWhiteKeySeedByNote(note);
                PianoKey whiteKey = CreateNewPianoKey(whiteKeySeed, m_WhiteLayer, new Vector2(lanePosXList[i], 0));
                whiteKey.SetupPianoSpriteCollection(m_PianoKeySpriteCollection[i]);
                i++;

                if (VirtualPianoHelper.IsBlackKey(m_Keys.Count()))
                {
                    PianoKey blackKey = CreateNewPianoKey(m_BlackKeySeed, m_BlackLayer, new Vector2(lanePosXList[i], 0));
                    blackKey.SetupPianoSpriteCollection(m_PianoKeySpriteCollection[i]);
                    i++;
                }
            }
        }

        private PianoKey GetWhiteKeySeedByNote(string note)
        {
            string actualNote = note.Substring(0, 1);
            switch (actualNote)
            {
                case "C":
                    return C_WhiteKeySeed;
                case "D":
                    return D_WhiteKeySeed;
                case "E":
                    return E_WhiteKeySeed;
                case "F":
                    return F_WhiteKeySeed;
                case "G":
                    return G_WhiteKeySeed;
                case "A":
                    return A_WhiteKeySeed;
                case "B":
                    return B_WhiteKeySeed;
                default:
                    return C_WhiteKeySeed;
            }
        }
        #endregion

        #region Creation

        protected PianoKey CreateNewPianoKey(PianoKey prefab, RectTransform layer, Vector2 pos)
        {
            PianoKey keyGo = Instantiate(prefab, layer.transform, true);
            keyGo.name = VirtualPianoHelper.GetNoteName(m_Keys.Count(), MinimumKey);
            var label = VirtualPianoHelper.GetNoteLabel(m_Keys.Count(), ShowAccidentalType.Both, "\n");
            //keyGo.SetToneLabel(label);
            RectTransform rect = (RectTransform)keyGo.transform;
            rect.localScale = Vector3.one;
            rect.anchoredPosition = pos;
            m_Keys.Add(keyGo);
            return keyGo;
        }

        public void ResetKeys(int note)
        {
            foreach (PianoKey key in m_Keys)
            {
                key.SetSprite("0", Handside.Left, false, note);
            }
        }

        private void CreateSpriteCollection()
        {
            m_PianoKeySpriteCollection = new List<VirtualKeySpriteCollection>();

            VirtualKeySpriteCollection[] stateSpriteInfo = m_PianoInfo.SpriteCollection;
            List<OverridedPianoSpriteInfo> overridedSpriteInfo = m_PianoInfo.OverrideSprite.ToList();

            int totalKeys = m_InputOctaves * VirtualPianoHelper.OctaveTotalKeys;
            for (int i = 0; i < totalKeys; i++)
            {
                var isBackKey = VirtualPianoHelper.IsBlackKey(i);
                var keyColor = isBackKey ? (int)PianoKeyType.BlackKey : (int)PianoKeyType.WhiteKey;
                var overridedKeys = overridedSpriteInfo.Where(x => x.OverridedNoteID == i).ToList();

                if (overridedKeys.Any())
                {
                    VirtualKeySpriteCollection newSpriteInfo = stateSpriteInfo[keyColor].Clone();

                    foreach (OverridedPianoSpriteInfo overrideKey in overridedKeys)
                    {
                        newSpriteInfo.SetSprite(
                            overrideKey.SpriteInfo.State,
                            overrideKey.SpriteInfo.Hand,
                            overrideKey.SpriteInfo.IsPressing,
                            overrideKey.SpriteInfo.Sprite,
                            overrideKey.SpriteInfo.Note
                        );
                    }

                    m_PianoKeySpriteCollection.Add(newSpriteInfo);
                    continue;
                }

                m_PianoKeySpriteCollection.Add(stateSpriteInfo[keyColor]);
            }
        }

        #endregion
    }
}