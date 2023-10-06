using Hendrix.Generic.UI.Elements;
using Notero.MidiGameplay.Core;
using Notero.Utilities;
using TMPro;
using UnityEngine;

namespace Hendrix.Gameplay.UI
{
    public class AccuracyMeterBarPanel : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_ProgressBarRect;

        [SerializeField]
        private SmoothProgressBar m_ProgressBar;

        [SerializeField]
        private TMP_Text m_AccuracyPercent;

        [SerializeField]
        private StarsContainer m_StarsContainer;

        private const float m_StarPosY = -10f;

        public void SetActive(bool isActive) => gameObject.SetActive(isActive);

        public void SetProgressBarValue(float value) => m_ProgressBar.value = value;

        public void SetAccuracyPercentText(float accuracy)
        {
            string accuracyText = DataFormatValidator.FloatToDecimalFormat(accuracy);
            m_AccuracyPercent.text = $"{accuracyText}%";
        }

        public void SetStar(int starCount) => m_StarsContainer.SetActiveAmount(starCount);

        public void SetStarsPosition(GameplayConfig config)
        {
            m_StarsContainer.SetStarPosition(0, CalculateStarPosition(config.OneStarAccuracy));
            m_StarsContainer.SetStarPosition(1, CalculateStarPosition(config.TwoStarAccuracy));
            m_StarsContainer.SetStarPosition(2, CalculateStarPosition(config.ThreeStarAccuracy));
        }

        private Vector2 CalculateStarPosition(int accuracyPercent)
        {
            float posX = m_ProgressBarRect.rect.width * accuracyPercent / 100;
            return new Vector2(posX, m_StarPosY);
        }
    }
}