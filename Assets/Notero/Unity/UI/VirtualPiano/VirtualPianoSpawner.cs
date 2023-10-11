using Notero.Unity.UI.VirtualPiano.Structure;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Notero.Unity.UI.VirtualPiano
{
    public class VirtualPianoSpawner : MonoBehaviour
    {
        [Header("m_Keys")]
        [SerializeField]
        protected PianoKey m_WhiteKeySeed;

        [SerializeField]
        protected PianoKey m_BlackKeySeed;

        [SerializeField]
        protected RectTransform m_WhiteLayer;

        [SerializeField]
        protected RectTransform m_BlackLayer;

        private PianoKeySpriteInfo m_PianoInfo;
        protected List<PianoSpriteCollection> m_PianoKeySpriteCollection;
        protected List<PianoKey> m_Keys { get; } = new List<PianoKey>();
        protected int m_InputOctaves;
        private string m_PianoType;

        public Rect WhiteKeyRect => ((RectTransform)m_WhiteKeySeed.transform).rect;

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
            return Resources.Load<PianoKeySpriteInfo>($"ScriptableObject/VirtualPianoInfo_{pianoTypeStr}");
        }

        #endregion

        #region Init

        protected virtual void InitializeKeys()
        {
            float containerSize = ((RectTransform)m_WhiteLayer.transform).rect.width;
            float whiteKeySize = ((RectTransform)m_WhiteKeySeed.transform).rect.width;
            float blackKeySize = ((RectTransform)m_BlackKeySeed.transform).rect.width;

            List<float> lanePosXList = VirtualPianoHelper.GetLanePosition(containerSize, whiteKeySize, blackKeySize, m_InputOctaves);

            for(int i = 0; i < lanePosXList.Count;)
            {
                PianoKey whiteKey = CreateNewPianoKey(m_WhiteKeySeed, m_WhiteLayer, new Vector2(lanePosXList[i], 0));
                whiteKey.SetupPianoSpriteCollection(m_PianoKeySpriteCollection[i]);
                i++;

                if(VirtualPianoHelper.IsBlackKey(m_Keys.Count()))
                {
                    PianoKey blackKey = CreateNewPianoKey(m_BlackKeySeed, m_BlackLayer, new Vector2(lanePosXList[i], 0));
                    blackKey.SetupPianoSpriteCollection(m_PianoKeySpriteCollection[i]);
                    i++;
                }
            }
        }

        #endregion

        #region Creation

        protected PianoKey CreateNewPianoKey(PianoKey prefab, RectTransform layer, Vector2 pos)
        {
            PianoKey keyGo = Instantiate(prefab, layer.transform, true);
            keyGo.name = VirtualPianoHelper.GetNoteName(m_Keys.Count(), MinimumKey);
            var label = VirtualPianoHelper.GetNoteLabel(m_Keys.Count(), ShowAccidentalType.Both, "\n");
            keyGo.SetToneLabel(label);
            RectTransform rect = (RectTransform)keyGo.transform;
            rect.localScale = Vector3.one;
            rect.anchoredPosition = pos;
            m_Keys.Add(keyGo);
            return keyGo;
        }

        public void ResetKeys()
        {
            foreach(PianoKey key in m_Keys)
            {
                key.SetSprite("0", Handside.Left, false);
            }
        }

        private void CreateSpriteCollection()
        {
            m_PianoKeySpriteCollection = new List<PianoSpriteCollection>();

            PianoSpriteCollection[] stateSpriteInfo = m_PianoInfo.SpriteCollection;
            List<OverridedPianoSpriteInfo> overridedSpriteInfo = m_PianoInfo.OverrideSprite.ToList();

            int totalKeys = m_InputOctaves * VirtualPianoHelper.OctaveTotalKeys;
            for(int i = 0; i < totalKeys; i++)
            {
                var isBackKey = VirtualPianoHelper.IsBlackKey(i);
                var keyColor = isBackKey ? (int)PianoKeyType.BlackKey : (int)PianoKeyType.WhiteKey;
                var overridedKeys = overridedSpriteInfo.Where(x => x.OverridedNoteID == i).ToList();

                if(overridedKeys.Any())
                {
                    PianoSpriteCollection newSpriteInfo = stateSpriteInfo[keyColor].Clone();

                    foreach(OverridedPianoSpriteInfo overrideKey in overridedKeys)
                    {
                        newSpriteInfo.SetSprite(
                            overrideKey.SpriteInfo.State,
                            overrideKey.SpriteInfo.Hand,
                            overrideKey.SpriteInfo.IsPressing,
                            overrideKey.SpriteInfo.Sprite
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