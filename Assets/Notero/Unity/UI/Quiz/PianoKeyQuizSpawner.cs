using Notero.Unity.UI.VirtualPiano;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Notero.Unity.UI.Quiz
{
    public class PianoKeyQuizSpawner : VirtualPianoSpawner
    {
        [SerializeField]
        private PianoKey m_YellowKeySeed;

        [SerializeField]
        private PianoKey m_CyanKeySeed;

        private int[] m_KeyChoices = new int[] { 0, 4, 7, 11 };

        private List<Image> m_TimerList = new List<Image>();

        private Image m_Timer;
        public List<Image> GetTimer() => m_TimerList;

        protected override void InitializeKeys()
        {
            float containerSize = ((RectTransform)m_WhiteLayer.transform).rect.width;
            float whiteKeySize = ((RectTransform)m_WhiteKeySeed.transform).rect.width;
            float blackKeySize = ((RectTransform)m_BlackKeySeed.transform).rect.width;

            List<float> lanePosXList = VirtualPianoHelper.GetLanePosition(containerSize, whiteKeySize, blackKeySize, m_InputOctaves);
            int keyIndex = 1;

            for(int i = 0; i < lanePosXList.Count;)
            {
                if(m_KeyChoices.Contains(i))
                {
                    // Spawn keys
                    PianoKey yellowKey = CreateNewPianoKey(m_YellowKeySeed, m_WhiteLayer, new Vector2(lanePosXList[i], 0));
                    yellowKey.SetupPianoSpriteCollection(m_PianoKeySpriteCollection[i]);

                    // Set Text
                    var choiceTextContainer = yellowKey.GetComponentInChildren<TMP_Text>();
                    choiceTextContainer.GetComponent<TMP_Text>().text = keyIndex.ToString();
                    keyIndex++;

                    // Initial timer value
                    m_Timer = yellowKey.transform.GetChild(1).GetComponentInChildren<Image>();
                    m_Timer.fillAmount = 0;
                    m_TimerList.Add(m_Timer);

                    i++;
                }
                else
                {
                    PianoKey whiteKey = CreateNewPianoKey(m_WhiteKeySeed, m_WhiteLayer, new Vector2(lanePosXList[i], 0));
                    whiteKey.SetupPianoSpriteCollection(m_PianoKeySpriteCollection[i]);
                    i++;
                }

                if(VirtualPianoHelper.IsBlackKey(m_Keys.Count()))
                {
                    PianoKey blackKey = CreateNewPianoKey(m_BlackKeySeed, m_BlackLayer, new Vector2(lanePosXList[i], 0));
                    blackKey.SetupPianoSpriteCollection(m_PianoKeySpriteCollection[i]);
                    i++;
                }
            }
        }

        public void ClearKeyList()
        {
            m_Keys.Clear();
            m_TimerList.Clear();
        }

        public void OnConfirmPianoKeyChoice(int pianoKeyIndex, string choice)
        {
            float containerSize = ((RectTransform)m_WhiteLayer.transform).rect.width;
            float whiteKeySize = ((RectTransform)m_WhiteKeySeed.transform).rect.width;
            float blackKeySize = ((RectTransform)m_BlackKeySeed.transform).rect.width;
            List<float> lanePosXList = VirtualPianoHelper.GetLanePosition(containerSize, whiteKeySize, blackKeySize, m_InputOctaves);

            ResetKeys();

            // create cyan key
            PianoKey cyanKey = CreateNewPianoKey(m_CyanKeySeed, m_WhiteLayer, new Vector2(lanePosXList[pianoKeyIndex], 0));
            cyanKey.SetupPianoSpriteCollection(m_PianoKeySpriteCollection[pianoKeyIndex]);
            cyanKey.GetComponentInChildren<TMP_Text>().text = choice;
        }
    }
}