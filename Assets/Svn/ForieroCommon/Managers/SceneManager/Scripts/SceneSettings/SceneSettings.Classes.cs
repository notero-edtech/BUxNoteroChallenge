using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using ForieroEngine.Settings;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

[SettingsManager]
public partial class SceneSettings : Settings<SceneSettings>, ISettingsProvider
{
    public enum LoadEnum
    {
        SceneName = 10,
        SceneProxyName = 20,
        InternalGUID = 30,
        SceneAssetGUID = 40,
        Command = 50,
        Undefined = int.MaxValue
    }

#if UNITY_EDITOR
    public void Serialize()
    {
        loadingAsyncScene.Serialize();                
        sceneItems.ForEach(n => n.Serialize());
    }
#endif

    [System.Serializable]
    public class SceneTransition
    {
        public bool fadeIn = true;
        public float fadeInTime = 0.3f;
        public Color fadeInColor = Color.black;
        public string fadeInSound = "";

        public bool fadeOut = true;
        public float fadeOutTime = 0.3f;
        public Color fadeOutColor = Color.black;
        public string fadeOutSound = "";
    }

    [System.Serializable]
    public class SceneList
    {
        public string name = "";
        public List<SceneItem> sceneItems = new List<SceneItem>();

#if UNITY_EDITOR
        public void Serialize()
        {
            foreach (SceneItem si in sceneItems)
            {
                si.Serialize();
            }
        }
#endif
    }

    [System.Serializable]
    public class SceneReferenceItem
    {
        [FormerlySerializedAs("commnad")]
        public string command = "";
        public string internalGUID = "";
#if UNITY_EDITOR
        [System.NonSerialized]
        public Port port;    
#endif
    }

    [System.Serializable]
    public class SceneItem
    {
        public string internalGUID = "";

        public string sceneName = "";
        public string sceneProxyName = "";
        public string sceneAssetPath = "";
        public string sceneAssetGUID = "";
        public int buildIndex = -1;

        public bool asynchronously = false;
        public bool asyncLoadingScene = true;

        public SceneTransition transition = new SceneTransition();

        public List<SceneReferenceItem> exits = new List<SceneReferenceItem>();

        public void LoadScene(string playerId = null, string applicationVersion = null)
        {
            UnityEngine.PlayerPrefs.SetString(GetPlayerPrefsId(playerId, applicationVersion), "LOADED");
            UnityEngine.PlayerPrefs.Save();

            var _activeSceneNode = SceneSettings.instance.FindSceneItemByActiveScene();
            var _activeSceneItem = _activeSceneNode;

            if (_activeSceneItem != null && _activeSceneItem.transition.fadeOut)
            {
                ForieroEngine.Scene.FadeOut(_activeSceneItem.transition.fadeOutTime, _activeSceneItem.transition.fadeOutColor, LoadSceneInternal);
            }
            else
            {
                LoadSceneInternal();
            }
        }

        void LoadSceneInternal()
        {
            var _activeSceneNode = SceneSettings.instance.FindSceneItemByActiveScene();
            var _activeSceneItem = _activeSceneNode;

            if (asynchronously)
            {
                if (asyncLoadingScene)
                {
                    SceneAsync.Options.sceneName = sceneName;
                    SceneManager.LoadScene(SceneSettings.instance.loadingAsyncScene.sceneName);
                }
                else
                {
                    SceneAsyncEmpty.Options.Reset();

                    SceneAsyncEmpty.Options.sceneName = sceneName;

                    if (_activeSceneItem != null)
                    {
                        SceneAsyncEmpty.Options.fadeOut = _activeSceneItem.transition.fadeOut;
                        SceneAsyncEmpty.Options.fadeOutTime = _activeSceneItem.transition.fadeOutTime;
                        SceneAsyncEmpty.Options.fadeOutColor = _activeSceneItem.transition.fadeOutColor;
                        SceneAsyncEmpty.Options.fadeOutSound = _activeSceneItem.transition.fadeOutSound;
                    }
                    var go = new GameObject("Async");
                    go.AddComponent<SceneAsyncEmpty>();
                }
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        public bool WasOnceLoaded(string playerId = null, string applicationVersion = null)
        {
            return !string.IsNullOrEmpty(UnityEngine.PlayerPrefs.GetString(GetPlayerPrefsId(playerId, applicationVersion), ""));
        }

        string GetPlayerPrefsId(string playerId = null, string applicationVersion = null)
        {
            string result = internalGUID + "_" + (string.IsNullOrEmpty(playerId) ? "NULL" : playerId) + "_" + (string.IsNullOrEmpty(applicationVersion) ? "NULL" : applicationVersion);

            if (instance.debug) Debug.Log(result);

            return result;
        }

#if UNITY_EDITOR
        public SceneAsset sceneAsset;
        public Vector2 position;
        
        public void Serialize()
        {
            sceneName = sceneAsset ? sceneAsset.name : "";
            sceneAssetPath = sceneAsset ? AssetDatabase.GetAssetPath(sceneAsset) : "";
            sceneAssetGUID = sceneAsset ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sceneAsset)) : "";
        }
#endif
    }

    [System.Serializable]
    public class LoadingAsyncSceneItem
    {
        public bool asFirstScene = false;
        public string sceneName = "";
        public string sceneAssetPath = "";
        public string sceneAssetGUID = "";
        public int buildIndex = -1;

        public SceneTransition transition = new SceneTransition();

#if UNITY_EDITOR
        public SceneAsset sceneAsset;

        public void Serialize()
        {
            sceneName = sceneAsset ? sceneAsset.name : "";
            sceneAssetPath = sceneAsset ? AssetDatabase.GetAssetPath(sceneAsset) : "";
            sceneAssetGUID = sceneAsset ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sceneAsset)) : "";
        }
#endif
    }
}
