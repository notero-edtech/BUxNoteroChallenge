using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public partial class SM : MonoBehaviour
{
    public static SM singleton;

    public static bool log
    {
        get { return SoundSettings.instance.log; }
        set { SoundSettings.instance.log = value; }
    }

    public static bool logUI
    {
        get { return SoundSettings.instance.logUI && Debug.isDebugBuild; }
        set { SoundSettings.instance.logUI = value; }
    }

    enum LogEnum
    {
        Message,
        Warning,
        Error
    }

    struct LogItem
    {
        public string message;
        public LogEnum logEnum;
        public float time;
        public Rect rect;
        
        public LogItem(string message, LogEnum logEnum)
        {
            this.message = message;
            this.logEnum = logEnum;
            this.time = 0;
            this.rect = new Rect(0, 0, 100, 30);
        }
    }

    static GameObject SM_Canvas;
    static GameObject PREFAB_SM_Message;
        
    static void Log(string message, LogEnum logEnum = LogEnum.Message)
    {
        switch (logEnum)
        {
            case LogEnum.Message:
                if(SM.log) Debug.Log(message);
                break;
            case LogEnum.Warning:
                if (SM.log) Debug.LogWarning(message);
                break;
            case LogEnum.Error:
                if (SM.log) Debug.LogError(message);
                break;
        }

        LogUI(message, logEnum);
    }

    static void LogUI(string message, LogEnum logEnum = LogEnum.Message)
    {
        if (!SM.logUI) return;

        if (!SM_Canvas) {
            SM_Canvas = Resources.Load<GameObject>("PREFAB_SM_Canvas");
            SM_Canvas = Instantiate(SM_Canvas);
            DontDestroyOnLoad(SM_Canvas);
               
            PREFAB_SM_Message = Resources.Load<GameObject>("PREFAB_SM_Message");          
        }

        var m = Instantiate<GameObject>(PREFAB_SM_Message, SM_Canvas.transform, false);
        m.GetComponentInChildren<TextMeshProUGUI>().text = message;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Init()
    {
        

        System.Diagnostics.Stopwatch stopWatch = ForieroDebug.CodePerformance ? System.Diagnostics.Stopwatch.StartNew() : null;
        if (!singleton)
        {
            GameObject go = new GameObject("SM");
            foreach (FXGroup fxGroup in SoundSettings.instance.fxGroups)
            {
                AudioSource audioSource = null;
                foreach (FXGroup fxGroupExists in SoundSettings.instance.fxGroups)
                {
                    if (fxGroup.audioMixerGroup == fxGroupExists.audioMixerGroup)
                    {
                        audioSource = fxGroup.audioSource;
                    }
                }

                if (audioSource == null)
                {
                    audioSource = go.AddComponent<AudioSource>();
                    audioSource.playOnAwake = false;
                }

                fxGroup.audioSource = audioSource;
                fxGroup.audioSource.outputAudioMixerGroup = fxGroup.audioMixerGroup;
            }

            go.AddComponent<SM>();

            foreach (MusicGroup musicGroup in SoundSettings.instance.musicGroups)
            {
                musicGroup.audioSource1 = go.AddComponent<AudioSource>();
                musicGroup.audioSource1.playOnAwake = false;
                musicGroup.audioSource1.outputAudioMixerGroup = musicGroup.audioMixerGroup;
                musicGroup.audioSource1.volume = 0f;
                musicGroup.audioSource2 = go.AddComponent<AudioSource>();
                musicGroup.audioSource2.playOnAwake = false;
                musicGroup.audioSource2.outputAudioMixerGroup = musicGroup.audioMixerGroup;
                musicGroup.audioSource2.volume = 0f;
                musicGroup.Init();
            }
        }
        if (ForieroDebug.CodePerformance) Debug.Log("METHOD STOPWATCH (SM - AfterSceneLoad): " + stopWatch?.Elapsed.ToString());
    }

    void Awake()
    {
        if (singleton)
        {
            DestroyImmediate(gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        singleton = this;

        foreach (MusicGroup musicGroup in SoundSettings.instance.musicGroups) { StartCoroutine(musicGroup.Update()); }
    }
}