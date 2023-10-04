using System.Diagnostics;
using ForieroEngine.Settings;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public partial class ForieroDebug : Settings<ForieroDebug>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/Debug", false, -1000)] public static void ScreenSettingsMenu() => Select();
#endif
    [Header("Debug")]
    public bool debug = false;

    public static bool Debug
    {
        get => instance.debug;
        set {
            if (value != instance.debug)
            {
                PlayerPrefs.SetInt("FORIERO_DEBUG", value ? 1 : 0);
                PlayerPrefs.Save();
            }
            instance.debug = value;
        }
    }
    
    [Header("General")]
    public bool general = false;
    public static bool General { get => instance.general && instance.debug; set => instance.general = value; }
    
    [Header("Player")]
    public bool player = false;
    public static bool Player { get => instance.player && instance.debug; set => instance.player = value; }
    
    [Header("Languages")]
    public bool languages = false;
    public static bool Languages { get => instance.languages && instance.debug; set => instance.languages = value; }

    [Header("Purchases")]
    public bool inAppPurchases = false;
    public static bool InAppPurchases { get => instance.inAppPurchases && instance.debug; set => instance.inAppPurchases = value; }
    
    [Header("UI")]
    public bool ui = false;
    public static bool UI { get => instance.ui && instance.debug; set => instance.ui = value; }
    
    [Header("Camera")]
    public bool camera = false;
    public static bool Camera { get => instance.camera && instance.debug; set => instance.camera = value; }
    
    [Header("Physics 2D")]
    public bool gameViewColliders2D = false;
    public static bool GameViewColliders2D { get => instance.gameViewColliders2D && instance.debug; set => instance.gameViewColliders2D = value; }

    [Header("Level Design")]
    public bool interactive = false;
    public static bool Interactive { get => instance.interactive && instance.debug; set => instance.interactive = value; }
    
    public bool environment;
    public static bool Environment { get => instance.environment && instance.debug; set => instance.environment = value; }
     
    [Header("Code")] 
    public bool codePerformance;
    public static bool CodePerformance { get => instance.codePerformance && instance.debug; set => instance.codePerformance = value; }
    
    [Header("Graphics")]
    public bool rendering;
    public static bool Rendering { get => instance.rendering && instance.debug; set => instance.rendering = value; }
    
    public bool vfx;
    public static bool VFX { get => instance.vfx && instance.debug; set => instance.vfx = value; }
    
    [Header("Audio")]
    public bool audio;
    public static bool Audio { get => instance.audio && instance.debug; set => instance.audio = value; }
    
    public bool fmod;
    public static bool FMOD { get => instance.fmod && instance.debug; set => instance.fmod = value; }
    
    public bool wwise;
    public static bool WWise { get => instance.wwise && instance.debug; set => instance.wwise = value; }
    
    [Header("Platforms")]
    public bool steam;
    public static bool STEAM { get => instance.steam && instance.debug; set => instance.steam = value; }
    
    public bool appleAppStore;
    public static bool AppleAppStore { get => instance.appleAppStore && instance.debug; set => instance.appleAppStore = value; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitBefore()
    {
        if (PlayerPrefs.HasKey("FORIERO_DEBUG")) instance.debug = PlayerPrefs.GetInt("FORIERO_DEBUG", instance.debug ? 1 : 0) == 1 ? true : false;        
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitAfter()
    {
        if (GameViewColliders2D && UnityEngine.Camera.main)
        {
            UnityEngine.Camera.main.gameObject.AddComponent<DebugDrawPhysics2D>();
        }
    }
}
