using UnityEngine;
using System.IO;
using ForieroEngine.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public partial class ScreenshotSettings : Settings<ScreenshotSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/Screenshot", false, -1000)] public static void ScreenshotSettingsMenu() => Select();   
#endif
    public KeyCode take1 = KeyCode.F1;
    public KeyCode take2 = KeyCode.F2;
    public KeyCode take3 = KeyCode.F3;
    public KeyCode take4 = KeyCode.F4;
    
    public int superSize = 1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init() => new GameObject("Screenshot").AddComponent<Screenshot>();
    
    public static void TakeScreenShot(int superSize = 1)
    {
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + @"/";

        int i = 0;
        string file = path + "screenshot_" + i.ToString() + ".png";
        while (File.Exists(file))
        {
            i++;
            file = path + "screenshot_" + i.ToString() + ".png";
        }
        Debug.Log("SCRENSHOT : " + file);
        ScreenCapture.CaptureScreenshot(file, superSize);
    }
}
