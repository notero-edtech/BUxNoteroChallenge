using UnityEngine;

using PlayerPrefs = UnityEngine.PlayerPrefs;
using System;
using ForieroEngine.Settings;
using ForieroEngine.Extensions;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public class ForieroSettings : Settings<ForieroSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/Foriero", false, -1000)] public static void ScreenSettingsMenu() => Select();
#endif

    public interface IForieroHand
    {
        void DoShow();
        void DoHide();
        bool DoToggle();
    }
    
    [Header("SRDebug")]
    public bool srDebug = false;
   
    [Header("Addressable")]
    public bool addressables = true;
    
    [Header("Parental Lock")]
    public bool parentalLock = false;
    
    [Header("PlayerPrefs")]
    [Tooltip("ForieroEngine.PlayerPrefs.autoSave")]
    public bool autoSave = true;
    public bool threaded = true;

    [Header("Easy Save 3")]
    public ES3Wrapper es3;

    [Header("Reviews")]
    public GameObject PREFAB_Rate_It;

    [Header("DoNotDestroy Objects")]
    public GameObject[] doNotDestroyObjects;

    [Serializable]
    public class StatusMessage
    {
        public GameObject prefabCanvas;
        public GameObject prefabMessage;
        public int sortingOrder = 1000;
    }

    [Header("UI Status Message")]
    public StatusMessage statusMessage;

    [Serializable]
    public class HandGestures
    {
        public GameObject prefabCanvas;
        public GameObject prefabHand;
        public int sortingOrder = 1000;
    }

    [Header("UI Hand Gestures")]
    public HandGestures handGestures;

    [Serializable]
    public class HandCursor
    {
        public GameObject handCursor;
        public bool dontDestroyOnLoad = false;
        public bool onlyInEditor = true;
        public int sortingOrder = 2000;
    }

    [Header("UI Hand Cursor")]
    public HandCursor handCursor;

    [Serializable]
    public class VoiceOverDownlaoder
    {
        public GameObject downloader;        
        public int sortingOrder = 2000;
    }

    [Header("UI Voice Over Downloader")]
    public VoiceOverDownlaoder voiceOverDownloader;

    [Serializable]
    public class ES3Wrapper
    {
    	public string cloudPhp = "https://backend.foriero.com/ES3Cloud.php";
    	public string apiKey = "";
    	public ES3.EncryptionType encryption = ES3.EncryptionType.AES;
    	[Password]
    	public string password = "";
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitBefore()
    {
        System.Diagnostics.Stopwatch stopWatch = ForieroDebug.CodePerformance ? System.Diagnostics.Stopwatch.StartNew() : null;
        
        ParentalLock.active = instance.parentalLock;

        ES3.Init();
        PlayerManager.autoSave = instance.autoSave;

        new GameObject("ForieroSettings").AddComponent<ForieroSettingsBehaviour>();
        
        if (instance.srDebug)
        {
            SRDebug.Init();
            UnityEngine.Debug.Log("SRDebug.Init()");
        }

        if (ForieroDebug.CodePerformance) Debug.Log("METHOD STOPWATCH (ForieroSettings - BeforeSceneLoad): " + stopWatch?.Elapsed.ToString());
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitAfter()
    {
        if (instance.doNotDestroyObjects != null)
        {
            foreach (var o in instance.doNotDestroyObjects)
            {
                Instantiate(o).AddOrGetComponent<DoNotDestroy>();
            }
        }
        
        if (instance.srDebug)
        {
            SRDebug.Init();
            UnityEngine.Debug.Log("SRDebug.Init()");
        }
    }
}
