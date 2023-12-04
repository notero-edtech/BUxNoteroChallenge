using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FramerateCounter : MonoBehaviour
{
    public float updateInterval = 0.5f;
    public Text fpsText;

    private float accum = 0f;
    private int frames = 0;
    private float timeleft;

    void Start()
    {
        timeleft = updateInterval;
    }

    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;

        if (timeleft <= 0.0)
        {
            float fps = accum / frames;
            string fpsTextString = string.Format("{0:F2} FPS", fps);

            fpsText.text = fpsTextString;

            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }
}
