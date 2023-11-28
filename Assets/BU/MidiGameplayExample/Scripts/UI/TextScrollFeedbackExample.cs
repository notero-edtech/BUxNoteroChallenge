using TMPro;
using UnityEngine;

namespace BU.MidiGameplay.UI
{
    public class TextScrollFeedbackExample : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text m_Text;

        [SerializeField]
        float m_FadeOutTime;

        [SerializeField]
        Vector2 m_Velocity;

        public void SetText(string text)
        {
            m_Text.text = text;
        }

        public void SetVelocity(Vector2 velo)
        {
            m_Velocity = velo;
        }

        private void Awake()
        {
            Destroy(gameObject, m_FadeOutTime);
        }

        private void Update()
        {
            transform.Translate(m_Velocity);
        }
    }
}