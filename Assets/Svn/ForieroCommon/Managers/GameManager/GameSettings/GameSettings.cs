using System;
using ForieroEngine.Settings;
using Sirenix.OdinInspector;
using UnityEngine;
using PlayerPrefs = ForieroEngine.PlayerPrefs;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class GameSettings : Settings<GameSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/Game", false, -1000)]
    public static void GameSettingsMenu() => Select();
#endif
    
    public Game.GameEnum gameEnum = Game.GameEnum.Single;
    
    public AudioSettings audio;
    public GraphicsSettings graphics;
    public ControllersSettings controllers;
    public PlayersSettings players;
    public SlotsSettings slots;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RuntimeInitializeAfterSceneLoad()
    {
        Load();
        Init();
        new GameObject("GameSettingsInit").AddComponent<GameSettingsInit>();
    }

    public static void Init()
    {
        instance.audio?.Init();
        instance.graphics?.Init();
        instance.controllers?.Init();
        switch (instance.gameEnum)
        {
            case Game.GameEnum.Single: break;
            case Game.GameEnum.Slots: instance.slots?.Init(); break;
            case Game.GameEnum.Players: instance.players?.Init(); break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    [Button]
    public static void Save()
    {
        instance.audio?.Save();
        instance.graphics?.Save();
        instance.controllers?.Save();
        switch (instance.gameEnum)
        {
            case Game.GameEnum.Single: break;
            case Game.GameEnum.Slots: instance.slots?.Save(); break;
            case Game.GameEnum.Players: instance.players?.Save(); break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    [Button]
    public static void Load()
    {
        instance.audio = AudioSettings.Load(instance.audio);
        instance.graphics = GraphicsSettings.Load(instance.graphics);
        instance.controllers = ControllersSettings.Load(instance.controllers);
        
        
        switch (instance.gameEnum)
        {
            case Game.GameEnum.Single: break;
            case Game.GameEnum.Slots: instance.slots = SlotsSettings.Load(instance.slots); break;
            case Game.GameEnum.Players: instance.players = PlayersSettings.Load(instance.players); break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
    
    public abstract class AbstractSettings<T> where T : class
    {
        public virtual void Save() => PlayerPrefs.SetString(typeof(T).Name, JsonUtility.ToJson(this));
        public static T Load(T defaultObject)
        {
            if (PlayerPrefs.HasKey<string>(typeof(T).Name))
            {
                return JsonUtility.FromJson<T>(PlayerPrefs.GetString(typeof(T).Name, ""));
            }
            
            return defaultObject;
        }

        public abstract void Init();
    }
}
