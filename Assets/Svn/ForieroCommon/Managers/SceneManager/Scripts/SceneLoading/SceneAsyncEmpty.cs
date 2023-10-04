using System.Collections;
using ForieroEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAsyncEmpty : MonoBehaviour
{
    public static class Options
    {
        public static string sceneName = "";

        public static bool fadeOut = true;
        public static float fadeOutTime = 0.3f;
        public static Color fadeOutColor = Color.black;
        public static string fadeOutSound = "";

        public static void Reset()
        {
            sceneName = "";

            fadeOut = true;
            fadeOutTime = 0.3f;
            fadeOutColor = Color.black;
            fadeOutSound = "";
        }
    }

    AsyncOperation asyncLoading = null;

    IEnumerator Start()
    {
        yield return null;

        if (string.IsNullOrEmpty(Options.sceneName))
        {
            Debug.LogError("Can not load EMPTY Options.sceneName!!!");
            yield break;
        }

        if (SceneSettings.instance.debug) Debug.Log("ForieroAsync : Loading Scene Async : " + Options.sceneName);
        asyncLoading = SceneManager.LoadSceneAsync(Options.sceneName, LoadSceneMode.Single);
        asyncLoading.allowSceneActivation = false;
        while (!asyncLoading.isDone && asyncLoading.progress < 0.9f) { yield return null; }

        if (SceneSettings.instance.debug) Debug.Log("Scene Loaded 0.9 and FadeOut Started : " + Options.sceneName);
        if (Options.fadeOut && false)
        {
            ForieroEngine.Scene.FadeOut(Options.fadeOutTime, Options.fadeOutColor, () =>
            {
                if (SceneSettings.instance.debug) Debug.Log("ForieroAsync : Scene FadeOut Finished : " + Options.sceneName);
            });

            yield return new WaitForSeconds(Options.fadeOutTime);
        }

        if (SceneSettings.instance.debug) Debug.Log("ForieroAsync : allowSceneActivation = true");
        asyncLoading.allowSceneActivation = true;

        if (SceneSettings.instance.debug) Debug.Log("ForieroAync : Waiting for asyncLoading.isDone");
        while (!asyncLoading.isDone) { yield return null; }
    }
}
