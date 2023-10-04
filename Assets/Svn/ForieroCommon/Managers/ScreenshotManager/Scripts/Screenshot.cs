using UnityEngine;
using UnityEngine.InputSystem;

public class Screenshot : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        #if ENABLE_INPUT_SYSTEM
        if (Keyboard.current[Key.LeftShift].wasPressedThisFrame || Keyboard.current[Key.RightShift].wasPressedThisFrame)
        {
            // if (Input.GetKeyDown(ScreenshotSettings.instance.take1)) ScreenshotSettings.TakeScreenShot(1);
            // if (Input.GetKeyDown(ScreenshotSettings.instance.take2)) ScreenshotSettings.TakeScreenShot(2);
            // if (Input.GetKeyDown(ScreenshotSettings.instance.take3)) ScreenshotSettings.TakeScreenShot(3);
            // if (Input.GetKeyDown(ScreenshotSettings.instance.take4)) ScreenshotSettings.TakeScreenShot(4);
        }
        #else
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(ScreenshotSettings.instance.take1)) ScreenshotSettings.TakeScreenShot(1);
                if (Input.GetKeyDown(ScreenshotSettings.instance.take2)) ScreenshotSettings.TakeScreenShot(2);
                if (Input.GetKeyDown(ScreenshotSettings.instance.take3)) ScreenshotSettings.TakeScreenShot(3);
                if (Input.GetKeyDown(ScreenshotSettings.instance.take4)) ScreenshotSettings.TakeScreenShot(4);
            }
        #endif
    }
}
