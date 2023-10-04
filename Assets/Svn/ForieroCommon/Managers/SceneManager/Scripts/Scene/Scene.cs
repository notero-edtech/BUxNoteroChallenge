using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ForieroEngine
{
    public static class Scene
    {
        public enum SceneEnum { Loading, Logo, SceneLoading, Undefined = int.MaxValue }
        
        private static GameObject _go;
        private static SceneScript _script;

        public static void LoadScene(string sceneName, bool asynchronously, Color transitionColor)
        {
            FadeOut(0.3f, transitionColor, () =>
            {
                if (asynchronously)
                {
                    SceneAsync.Options.sceneName = sceneName;
                    SceneManager.LoadScene(SceneSettings.instance.loadingAsyncScene.sceneName);
                }
                else
                {
                    SceneManager.LoadScene(sceneName);
                }
            });
        }

        public static void LoadScene(string aSceneName, string loadingScene = "Loading", float time = 0.1f)
        {
            Fade(1f, new Color(0, 0, 0, 0), new Color(0, 0, 0, 1),
                () =>
                {
                    _script.StartCoroutine(LoadingScene(aSceneName, loadingScene));
                });
        }

        private static IEnumerator LoadingScene(string aSceneName, string loadingScene = "Loading", float time = 0.1f)
        {
            var async = SceneManager.LoadSceneAsync(loadingScene);
            while (!async.isDone) { yield return async; }
            Fade(time, new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), () =>
            {
                Fade(time, new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), () =>
                {
                     _script.StartCoroutine(WaitForLoadingFinished(aSceneName));
                });
            });
        }

        private static IEnumerator WaitForLoadingFinished(string aSceneName)
        {
            var async = SceneManager.LoadSceneAsync(aSceneName);
            while (!async.isDone) { yield return async; }
            Fade(1f, new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), DestroyFadeObject);
        }

        private static IEnumerator WaitInternal(float aTime,  Action onComplete)
        {
            yield return new WaitForSeconds(aTime);
            onComplete?.Invoke();
        }
        
        public static void Wait(float aTime, Action onComplete) => _script.StartCoroutine(WaitInternal(aTime, onComplete));
        
        public static void Fade(float aFadeTime, Color aFromColor, Color aToColor, Action onComplete)
        {
            if (_script)
            {
                _script.StopAllCoroutines();
                SceneScript.fadeColor = aFromColor;
                _script.StartCoroutine(FadeEnumerator(aFadeTime, aFromColor, aToColor, onComplete));
            }
            else
            {
                CreateFadeObject(aFromColor);
                SceneScript.fadeColor = aFromColor;
                _script.StartCoroutine(FadeEnumerator(aFadeTime, aFromColor, aToColor, onComplete));
            }
        }

        public static void FadeIn(float aFadeTime, Color aColor, Action onComplete)
        {
            var start = new Color(aColor.r, aColor.g, aColor.b, 1);
            var end = new Color(aColor.r, aColor.g, aColor.b, 0);
            SM.PlayFX(SceneSettings.fadeInSound);
            Fade(aFadeTime, start, end, onComplete);
        }

        public static void FadeOut(float aFadeTime, Color aColor, Action onComplete, SceneEnum scene = SceneEnum.Undefined)
        {
            SceneScript.sceneEnum = scene;
            var start = new Color(aColor.r, aColor.g, aColor.b, 0);
            var end = new Color(aColor.r, aColor.g, aColor.b, 1);
            SM.PlayFX(SceneSettings.fadeOutSound);
            Fade(aFadeTime, start, end, onComplete);
        }

        public static void DisableFadeTexture() { if (_script != null) SceneScript.fade = false; }
        public static void EnableFadeTexture() { if (_script != null) SceneScript.fade = true; }

        public static void CreateFadeObject(Color aColor)
        {
            if (!_script)
            {
                _go = new GameObject();
                _go.name = "SceneScript";
                _script = _go.AddComponent<SceneScript>();
            }
            SceneScript.fadeColor = aColor;
        }

        public static void DestroyFadeObject() { if (_script) { UnityEngine.Object.Destroy(_script.gameObject); } }

        private static IEnumerator FadeEnumerator(float aTime, Color aFrom, Color aTo, Action onComplete)
        {
            var t = 0f;
            SceneScript.fade = true;
            do
            {
                yield return null;
                t += Time.deltaTime;
                t = Mathf.Clamp(t, 0, aTime);
                SceneScript.fadeColor.r = aFrom.r + (t / aTime) * (aTo.r - aFrom.r);
                SceneScript.fadeColor.g = aFrom.g + (t / aTime) * (aTo.g - aFrom.g);
                SceneScript.fadeColor.b = aFrom.b + (t / aTime) * (aTo.b - aFrom.b);
                SceneScript.fadeColor.a = aFrom.a + (t / aTime) * (aTo.a - aFrom.a);
            } while (t < aTime);

            yield return null;
            
            onComplete?.Invoke();
        }
    }
}