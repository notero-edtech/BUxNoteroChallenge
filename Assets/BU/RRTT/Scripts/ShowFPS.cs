using TMPro;
using UnityEngine;

namespace BU.RRTT.Scripts
{
    public class ShowFPS : MonoBehaviour
    {
        public TextMeshProUGUI m_Text;
        private float time;
        private float frameCount;
        private float pollingTime = 1f;

        private void Update()
        {
            time += Time.deltaTime;

            frameCount++;
            if (time >= pollingTime)
            {
                float frameRate = Mathf.RoundToInt(frameCount / time);
                m_Text.text = frameRate.ToString("#.##");

                time -= pollingTime;
                frameCount = 0;
            }
        }
    }
}
