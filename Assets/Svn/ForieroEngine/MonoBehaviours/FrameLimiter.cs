using UnityEngine;
using System;
 
public class FrameLimiter : MonoBehaviour {
         
    public int desiredFPS = 60;
 
    private void Awake()
    {
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 0;
    }
 
    private void Update()
    {
        var lastTicks = DateTime.Now.Ticks;
        var currentTicks = lastTicks;
        var delay = 1f / desiredFPS;
        var elapsedTime = 0f;
 
        if (desiredFPS <= 0) return;
 
        while (true)
        {
            currentTicks = DateTime.Now.Ticks;
            elapsedTime = (float)TimeSpan.FromTicks(currentTicks - lastTicks).TotalSeconds;
            if(elapsedTime >= delay) { break; }
        }
    }
}
