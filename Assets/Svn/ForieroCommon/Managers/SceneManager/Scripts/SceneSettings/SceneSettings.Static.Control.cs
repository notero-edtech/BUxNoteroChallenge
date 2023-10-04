using ForieroEngine.Settings;
using UnityEngine;

public partial class SceneSettings : Settings<SceneSettings>
{
    static SceneItem _currentItem;

    public static SceneItem currentItem{ get{ return _currentItem ?? new SceneItem(); } }

    public static bool WasSceneOnceLoaded(LoadEnum loadEnum, string sceneValue, string playerId = null, string applicationVersion = null)
    {
        SceneItem sceneItem = null;

        switch (loadEnum)
        {
            case LoadEnum.SceneName:
                sceneItem = SceneSettings.instance.FindSceneItemBySceneName(sceneValue);
                break;
            case LoadEnum.SceneProxyName:
                sceneItem = SceneSettings.instance.FindSceneItemBySceneProxyName(sceneValue);
                break;
            case LoadEnum.InternalGUID:
                sceneItem = SceneSettings.instance.FindSceneItemByInternalGUID(sceneValue);
                break;
            case LoadEnum.SceneAssetGUID:
                sceneItem = SceneSettings.instance.FindSceneItemBySceneAssetGUID(sceneValue);
                break;
            case LoadEnum.Command:
                foreach (SceneReferenceItem sri in currentItem.exits)
                {
                    if (sri.command == sceneValue)
                    {
                        sceneItem = SceneSettings.instance.FindSceneItemByInternalGUID(sri.internalGUID);
                    }
                }
                break;
        }

        if (sceneItem == null)
        {
            Debug.LogError("SceneSettings - SceneItem not found for : " + loadEnum.ToString() + " " + sceneValue);
            return false;
        }

        return sceneItem.WasOnceLoaded(playerId, applicationVersion);
    }

    public static void LoadScene(LoadEnum loadEnum, string sceneValue, string playerId = null, string applicationVersion = null)
    {
        SceneItem sceneItem = null;

        switch (loadEnum)
        {
            case LoadEnum.SceneName:
                sceneItem = SceneSettings.instance.FindSceneItemBySceneName(sceneValue);
                break;
            case LoadEnum.SceneProxyName:
                sceneItem = SceneSettings.instance.FindSceneItemBySceneProxyName(sceneValue);
                break;
            case LoadEnum.InternalGUID:
                sceneItem = SceneSettings.instance.FindSceneItemByInternalGUID(sceneValue);
                break;
            case LoadEnum.SceneAssetGUID:
                sceneItem = SceneSettings.instance.FindSceneItemBySceneAssetGUID(sceneValue);
                break;
            case LoadEnum.Command:
                foreach (SceneReferenceItem sri in currentItem.exits)
                {
                    if (sri.command == sceneValue)
                    {
                        sceneItem = SceneSettings.instance.FindSceneItemByInternalGUID(sri.internalGUID);
                    }
                }
                break;
        }

        if (sceneItem == null)
        {
            Debug.LogError("SceneSettings - SceneItem not found for : " + loadEnum.ToString() + " " + sceneValue);
        }
        else
        {
            sceneItem.LoadScene(playerId, applicationVersion);
        }
    }

    public static void LoadSceneBySceneName(string sceneName, string playerId = null, string applicationVersion = null)
    {
        LoadScene(LoadEnum.SceneName, sceneName, playerId, applicationVersion);
    }

    public static void LoadSceneBySceneProxyName(string sceneProxyName, string playerId = null, string applicationVersion = null)
    {
        LoadScene(LoadEnum.SceneProxyName, sceneProxyName, playerId, applicationVersion);
    }

    public static void LoadSceneByInternalGUID(string internalGUID, string playerId = null, string applicationVersion = null)
    {
        LoadScene(LoadEnum.InternalGUID, internalGUID, playerId, applicationVersion);
    }

    public static void LoadSceneBySceneAssetGUID(string sceneAssetGUID, string playerId = null, string applicationVersion = null)
    {
        LoadScene(LoadEnum.SceneAssetGUID, sceneAssetGUID, playerId, applicationVersion);
    }

    public static void LoadSceneByCommand(string command, string playerId = null, string applicationVersion = null)
    {
        LoadScene(LoadEnum.Command, command, playerId, applicationVersion);
    }
}
