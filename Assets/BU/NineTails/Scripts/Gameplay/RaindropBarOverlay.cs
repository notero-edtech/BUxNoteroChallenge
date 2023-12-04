using System.Collections.Generic;
using UnityEngine;

namespace BU.NineTails.MidiGameplay.Scripts.Gameplay
{
    public class RaindropBarOverlay : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_SplitterMid;

        [SerializeField]
        private RectTransform m_MainBar;

        [SerializeField]
        private Transform m_SplitterContainer;

        public void SetActive(bool isActive) => gameObject.SetActive(isActive);

        private const int OctaveTotalKeys = 12;

        public void Init(float pianoFitWidth, float octaveAmount, List<float> lanePosXList)
        {
            m_MainBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pianoFitWidth);

            for (int i = 1; i < octaveAmount; i++)
            {
                int index = i * OctaveTotalKeys;
                SpawnSplitterMid(lanePosXList[index]);
            }
        }

        private void SpawnSplitterMid(float posX)
        {
            RectTransform newSplitter = Instantiate(m_SplitterMid, m_SplitterContainer);
            newSplitter.anchoredPosition = new Vector2(posX, 0);
        }
    }
}