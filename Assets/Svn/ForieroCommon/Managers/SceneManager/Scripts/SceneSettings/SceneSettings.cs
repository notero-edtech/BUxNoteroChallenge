using UnityEngine;
using ForieroEngine;
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.Settings;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class SceneSettings : Settings<SceneSettings>
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/Scene", false, -1000)]
    public static void SceneSettingsMenu() => Select();
#endif

    public bool debug = false;
    public Texture2D vignette;
    public GameObject logoUIPrefab;
    public GameObject loadingUIPrefab;
    public GameObject sceneLoadingUIPrefab;

    public LoadingAsyncSceneItem loadingAsyncScene;
    
    [FormerlySerializedAs("scenes")]
    public List<SceneItem> sceneItems;

    public static string fadeInSound = "";
    public static string fadeOutSound = "";
       
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        var stopWatch = ForieroDebug.CodePerformance ? System.Diagnostics.Stopwatch.StartNew() : null;

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += instance.SceneLoaded;
        
        if (instance.loadingAsyncScene.asFirstScene)
        {
            if(instance.sceneItems.Count > 0){
                SceneAsync.Options.sceneName = instance.sceneItems[0].sceneName;
            }
        }
        if(ForieroDebug.CodePerformance) Debug.Log("METHOD STOPWATCH (SceneSettings - BeforeSceneLoad): " + stopWatch?.Elapsed.ToString());
    }

    private void SceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
    {
        fadeInSound = loadingAsyncScene.transition.fadeInSound;
        fadeOutSound = loadingAsyncScene.transition.fadeOutSound;

        if (loadingAsyncScene.sceneAssetPath == scene.path)
        {
            Scene.FadeIn(loadingAsyncScene.transition.fadeInTime, loadingAsyncScene.transition.fadeInColor, null);
        }
        else
        {
            _currentItem = FindSceneItemBySceneAssetPath(scene.path);

            if (instance.debug) Debug.Log("Loaded Scene : " + scene.name);

            if (_currentItem == null)
            {
                if (instance.debug) Debug.LogError("Loaded Scene : " + scene.name + " NOT FOUND SCENE ITEM!!!");
            }
            else
            {
                if (instance.debug) Debug.Log("Found scene item : " + _currentItem.sceneName);
                if (_currentItem.transition.fadeIn)
                {
                    Scene.FadeIn(_currentItem.transition.fadeInTime, _currentItem.transition.fadeInColor, null);
                }
            }
        }
    }

    private void PlayMusic(string musicGroupId, string songId)
    {
        if (!string.IsNullOrEmpty(musicGroupId)) { SM.PlayMusic(musicGroupId, songId); }
    }

#if UNITY_EDITOR
    public static void ApplyToBuildSettings(SceneSettings settings)
    {
        if (SceneSettings.instance == settings) return;

        var scenes = new List<EditorBuildSettingsScene>();
        int buildIndex = 0;

        if (settings.loadingAsyncScene.asFirstScene && settings.loadingAsyncScene.sceneAsset)
        {
            var scene = new EditorBuildSettingsScene();
            settings.loadingAsyncScene.sceneAssetPath = scene.path = AssetDatabase.GetAssetPath(settings.loadingAsyncScene.sceneAsset);
            scene.enabled = true;
            scenes.Add(scene);
            settings.loadingAsyncScene.buildIndex = buildIndex;
            EditorUtility.SetDirty(settings);
            buildIndex++;
        }

        foreach(var si in settings.sceneItems)
        {            
            if (si.sceneAsset)
            {
                var scene = new EditorBuildSettingsScene();
                si.sceneAssetPath = scene.path = AssetDatabase.GetAssetPath(si.sceneAsset);
                if (scenes.Any(s=>s.path == scene.path)) continue;
                scene.enabled = true;
                scenes.Add(scene);
                si.buildIndex = buildIndex;
                EditorUtility.SetDirty(settings);
                buildIndex++;
            }
        }
        
        if (!settings.loadingAsyncScene.asFirstScene && settings.loadingAsyncScene.sceneAsset)
        {
            var scene = new EditorBuildSettingsScene();
            settings.loadingAsyncScene.sceneAssetPath = scene.path = AssetDatabase.GetAssetPath(settings.loadingAsyncScene.sceneAsset);
            scene.enabled = true;
            scenes.Add(scene);
            settings.loadingAsyncScene.buildIndex = buildIndex;
            EditorUtility.SetDirty(settings);
            buildIndex++;
        }

        EditorBuildSettings.scenes = scenes.ToArray();

        settings.Apply();
    }
#endif
}
