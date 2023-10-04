using System.Linq;
using ForieroEngine.Settings;
using UnityEngine;

public partial class SceneSettings : Settings<SceneSettings>
{
    SceneItem FindSceneItemByInternalGUID(string guid)
    {
        return sceneItems.FirstOrDefault(n => n.internalGUID == guid);
    }

    SceneItem FindSceneItemBySceneAssetGUID(string sceneAssetGUID)
    {
        return sceneItems.FirstOrDefault(n => n.sceneAssetGUID == sceneAssetGUID);
    }

    SceneItem FindSceneItemBySceneName(string sceneName)
    {
        return sceneItems.FirstOrDefault(n => n.sceneName == sceneName);
    }

    SceneItem FindSceneItemBySceneProxyName(string sceneProxyName)
    {
        return sceneItems.FirstOrDefault(n => n.sceneProxyName == sceneProxyName);
    }

    SceneItem FindSceneItemByActiveScene()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        return FindSceneItemBySceneAssetPath(scene.path);
    }

    SceneItem FindSceneItemBySceneAssetPath(string sceneAssetPath)
    {
        return sceneItems.FirstOrDefault(n => n.sceneAssetPath == sceneAssetPath);
    }

    SceneItem FindSceneNodeByBuildIndex(int buildIndex)
    {
        var sceneItem = default(SceneItem);

        int index = -1;
        foreach (var n in sceneItems)
        {
            index++;
            if (buildIndex == index)
            {
                sceneItem = n;
                break;
            }
        }
       
        if (sceneItem == null)
        {
            Debug.Log("SCENE SETTINGS : Probably Loading Async Scene loaded");
        }
        else
        {
            if (sceneItem.buildIndex != buildIndex)
            {
                Debug.LogError("SCENE SETTINGS : Something is wrong. Build Indexes are not equal");
            }
        }

        return sceneItem;
    }
}
