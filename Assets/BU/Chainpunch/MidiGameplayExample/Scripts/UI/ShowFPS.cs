using UnityEngine;
using TMPro;

namespace BU.Chainpunch.MidiGameplay.UI
{
    public class ShowFPS : MonoBehaviour
    {
        [SerializeField] private TMP_Text fpsText;
        [SerializeField] private float deltaTime;

        // Update is called once per frame
        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            fpsText.text = Mathf.Ceil(fps).ToString();
        }
    }
}

