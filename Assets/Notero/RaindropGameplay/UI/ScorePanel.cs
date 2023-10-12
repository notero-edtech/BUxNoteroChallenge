using Notero.Utilities;
using TMPro;
using UnityEngine;

namespace Notero.RaindropGameplay.UI
{
    public class ScorePanel : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text m_Score;

        public void SetActive(bool isActive) => gameObject.SetActive(isActive);

        public void SetScoreText(float score) => m_Score.text = DataFormatValidator.RoundPositiveFloatToString(score);
    }
}