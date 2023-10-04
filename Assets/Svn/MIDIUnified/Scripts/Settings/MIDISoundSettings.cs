using UnityEngine;
using ForieroEngine.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if ((UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR
using Un4seen.Bass;
#endif

[SettingsManager]
public partial class MIDISoundSettings : Settings<MIDISoundSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/Midi/Sound")] public static void MIDISoundSettingsMenu() => Select();   
#endif
        
    public bool init = false;
    [Header("BASS24NET License Login Required. http://bass.radio42.com/")]
    public string userName;
    [Password]
    public string regKey;

    [System.Serializable]
    public class PlatformSettings
    {
        public enum OutputSampleRateDividerEnum
        {
            One = 1,
            Two = 2,
            Four = 4
        }

        [Tooltip("AudioSettings.outputSampleRate / outputSampleRateDivider")]
        public OutputSampleRateDividerEnum outputSampleRateDivider = OutputSampleRateDividerEnum.One;

        public int BASS_CONFIG_UPDATEPERIOD = 10;
        public int BASS_CONFIG_BUFFER = 11;
        public bool BASS_INFO_MIN_BUFFER = true;

        public int sampleRate
        {
            get
            {
                
#if WWISE
                return 44100 / (int) outputSampleRateDivider;
#else
                return AudioSettings.outputSampleRate / (int)outputSampleRateDivider;
#endif
            }
        }
    }

    [Header("Platform settings")]
    public PlatformSettings android;
    public PlatformSettings ios;
    public PlatformSettings wsa;
    public PlatformSettings windows;
    public PlatformSettings linux;
    public PlatformSettings osx;
    public PlatformSettings other;

    public static PlatformSettings settings
    {
        get
        {
#if UNITY_EDITOR_OSX
            return instance.osx == null ? new PlatformSettings() : instance.osx;
#elif UNITY_EDITOR_WIN
            return instance.windows == null ? new PlatformSettings() : instance.windows;
#elif UNITY_EDITOR_LINUX
            return instance.linux == null ? new PlatformSettings() : instance.linux;
#elif UNITY_ANDROID
            return instance.android == null ? new PlatformSettings() : instance.android;
#elif UNITY_IOS
            return instance.ios == null ? new PlatformSettings() : instance.ios;
#elif UNITY_WSA
            return instance.wsa == null ? new PlatformSettings() : instance.wsa;
#elif UNITY_STANDALONE_OSX
            return instance.osx == null ? new PlatformSettings() : instance.osx;
#elif UNITY_STANDALONE_WIN
            return instance.windows == null ? new PlatformSettings() : instance.windows;
#elif UNITY_STANDAOLONE_LINUX
            return instance.linux == null ? new PlatformSettings() : instance.linux;
#else
            return instance.other == null ? new PlatformSettings() : instance.other;
#endif
        }
    }

    public static volatile bool initialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void AutoInit()
    {
        System.Diagnostics.Stopwatch stopWatch = ForieroDebug.CodePerformance ? System.Diagnostics.Stopwatch.StartNew() : null;
        if(instance.init) Init(settings.sampleRate);
        if(ForieroDebug.CodePerformance) Debug.Log("METHOD STOPWATCH (MIDISoundSettings - BeforeSceneLoad): " + stopWatch?.Elapsed.ToString());
    }

#if ((UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR

    public static void Init(int sampleRate = -1)
    {
        if (initialized) return;

        Register(instance.userName, instance.regKey);

        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_VISTA_TRUEPOS, 0);
            IsBassError("BASS_SetConfig(BASSConfig.BASS_CONFIG_VISTA_TRUEPOS, 0)");
        }

        Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, settings.BASS_CONFIG_UPDATEPERIOD);
        IsBassError("BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD)");

        if (Bass.BASS_Init(-1, sampleRate == -1 ? settings.sampleRate : sampleRate, BASSInit.BASS_DEVICE_LATENCY, System.IntPtr.Zero))
        {
            int minBuffer = 0;
            if (settings.BASS_INFO_MIN_BUFFER)
            {
                BASS_INFO info = Bass.BASS_GetInfo();
                IsBassError("Bass.BASS_GetInfo()");
                minBuffer = info.minbuf;
            }

            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, settings.BASS_CONFIG_BUFFER + minBuffer);
            IsBassError("Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER)");

            initialized = true;
        }
        else
        {
            var error = Bass.BASS_ErrorGetCode();
            if (error == BASSError.BASS_ERROR_ALREADY)
            {
                Debug.LogWarning("BASS24 was already initialized.");
                initialized = true;
            }
            else
            {
                Debug.LogError("BASS24 : Init Error " + error.ToString());
            }
        }

        if (initialized)
        {
            var go = new GameObject("MIDISoundSettingsInstance");
            go.AddComponent<MIDISoundSettingsInstance>();
            DontDestroyOnLoad(go);
        }
    }

    public static void Register(string userName, string regKey)
    {

        if (MIDISettings.IsDebug) Debug.Log("BASS24NETSynth - Register : " + userName);

        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(regKey))
        {
            BassNet.Registration(userName, regKey);
        }
    }

    public static void Free()
    {
        if (MIDISettings.IsDebug) Debug.Log("BASS24 : Free");

        if (initialized)
        {
            foreach (var clip in AudioClipBass24.clips)
            {
                clip?.Dispose();
            }

            Bass.BASS_PluginFree(0);
            IsBassError("BASS_PluginFree(0)");
            Bass.BASS_Free();
            IsBassError("BASS_Free()");
        }

        initialized = false;
    }

    static bool IsBassError(string location)
    {
        var error = Bass.BASS_ErrorGetCode();
        if (error == 0)
        {
            return false;
        }
        else
        {
            Debug.LogError("BASS24 Error in " + location + ", code = " + error.ToString());
            return true;
        }
    }

#else
    public static void Init(int frequency = -1)
    {
        if (MIDISettings.isDebug) Debug.LogWarning("BASS24 not supported for this platform");
    }

    public static void Register(string userName, string regKey)
    {
        if (MIDISettings.isDebug) Debug.LogWarning("BASS24 not supported for this platform");
    }

    public static void Free()
    {
        if (MIDISettings.isDebug) Debug.LogWarning("BASS24 not supported for this platform");
    }
#endif
}
