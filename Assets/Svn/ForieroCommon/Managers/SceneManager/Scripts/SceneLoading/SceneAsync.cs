using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ForieroEngine.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneAsync : MonoBehaviour
{
    public static class Options
    {
        public static string sceneName = "";
        public static string loadingText = "";
        public static string infoText = "";

        public static Dictionary<string, float> progressItems = new Dictionary<string, float>();

        public static bool AllDone()
        {
            var r = true;
            foreach (var kv in progressItems)
            {
                if (kv.Value >= 1) { continue; }
                else { r = false; break; }
            }
            return r;
        }

        public static void Reset()
        {
            loadingText = "";
            infoText = "";
            progressItems = new Dictionary<string, float>();
        }
    }

    [Header("Text")]
    public Text loadingText;
    public Text infoText;

    [Header("TMP")]
    public TextMeshProUGUI loadingTextTmp;
    public TextMeshProUGUI infoTextTmp;

    [Header("Graphics")]
    public Image loadingBackground;
    public Image loadingForeground;
        
    public string loadingDictionary;
    public string loadingRecord;
    public string loadingDefault;

    [HideInInspector] public string loading;
    [HideInInspector] public bool waitFlag = false;
    [HideInInspector] public bool loaded = false;

    public UnityEvent OnSceneLoaded;
    
    private AsyncOperation _asyncLoading = null;

    private float GetProgress()
    {
        if (_asyncLoading == null) { return 0f; }
        var p = _asyncLoading.progress;
        foreach (var kv in Options.progressItems) { p += kv.Value; }
        return (p / (float)(Options.progressItems.Count + 1)) / 0.9f;
    }

    private Tween _delayTween = null;
    private Tweener _loadingTextTweener = null;
    private Tweener _infoTextColorTweener = null;

    private Tween _delayTweenTmp = null;
    private Tweener _loadingTextTweenerTmp = null;
    private Tweener _infoTextColorTweenerTmp = null;

    private void LoadingTween()
    {
        if (loadingText)
        {
            _loadingTextTweener = loadingText.DOText(Options.loadingText + "...", 0.6f, false, ScrambleMode.None, null).SetEase(Ease.OutSine);
            _delayTween = DOVirtual.DelayedCall(4, () =>
            {
                if (loadingText)
                {
                    loadingText.text = "";
                    LoadingTween();
                }
            });
        }

        if (loadingTextTmp)
        {
            #if DOTWEENPRO
            _loadingTextTweenerTmp = loadingTextTmp.DOText(Options.loadingText + "...", 0.6f, false, ScrambleMode.None, null).SetEase(Ease.OutSine);
            #endif
            _delayTweenTmp = DOVirtual.DelayedCall(4, () =>
            {
                if (loadingTextTmp)
                {
                    loadingTextTmp.text = "";
                    LoadingTween();
                }
            });
        }
    }

    private void OnDestroy()
    {
        _delayTween?.Kill();
        _loadingTextTweener?.Kill();
        _infoTextColorTweener?.Kill();

        _delayTweenTmp?.Kill();
        _loadingTextTweenerTmp?.Kill();
        _infoTextColorTweenerTmp?.Kill();

        _progressTweener?.Kill();
    }

    private IEnumerator Start()
    {
        if (string.IsNullOrEmpty(Options.loadingText))
        {
            Options.loadingText = Lang.GetText(loadingDictionary, loadingRecord, loadingDefault);
        }

        if(loadingText) loadingText.text = "";
        if (loadingTextTmp) loadingTextTmp.text = "";

        LoadingTween();

        if (infoText)
        {
            infoText.text = Options.infoText;
            _infoTextColorTweener = infoText.DOColor(infoText.color, 0.3f).SetDelay(0.5f);
            infoText.color = infoText.color.A(0);
        }

        if (infoTextTmp)
        {
            infoTextTmp.text = Options.infoText;
            _infoTextColorTweenerTmp = infoTextTmp.DOColor(infoTextTmp.color, 0.3f).SetDelay(0.5f);
            infoTextTmp.color = infoTextTmp.color.A(0);
        }

        yield return null;

        if (string.IsNullOrEmpty(Options.sceneName))
        {
            if(infoText) infoText.text = "Can not load EMPTY Options.sceneName!!!";
            if (infoTextTmp) infoTextTmp.text = "Can not load EMPTY Options.sceneName!!!";
            yield break;
        }

        if (SceneSettings.instance.debug) Debug.Log("ForieroAsync : Loading Scene Async : " + Options.sceneName);
        _asyncLoading = SceneManager.LoadSceneAsync(Options.sceneName, LoadSceneMode.Single);
        if (_asyncLoading == null) yield break;
        _asyncLoading.allowSceneActivation = false;
        while (!_asyncLoading.isDone && _asyncLoading.progress < 0.9f) { yield return null; }

        loaded = true;
        OnSceneLoaded.Invoke();

        yield return new WaitUntil(() => !waitFlag);

        if (SceneSettings.instance.debug) Debug.Log("ForieroAsync : Waiting for all items 'done'");
        while (!Options.AllDone()) { yield return null; }

        if (SceneSettings.instance.debug) Debug.Log("Scene Loaded 0.9 and FadeOut Started : " + Options.sceneName);
        ForieroEngine.Scene.FadeOut(SceneSettings.instance.loadingAsyncScene.transition.fadeOutTime, SceneSettings.instance.loadingAsyncScene.transition.fadeOutColor, () =>
        {
            if (SceneSettings.instance.debug) Debug.Log("ForieroAsync : Scene FadeOut Finished : " + Options.sceneName);
        });

        yield return new WaitForSeconds(SceneSettings.instance.loadingAsyncScene.transition.fadeOutTime);

        if (SceneSettings.instance.debug) Debug.Log("ForieroAsync : allowSceneActivation = true");
        _asyncLoading.allowSceneActivation = true;

        if (SceneSettings.instance.debug) Debug.Log("ForieroAync : Waiting for asyncLoading.isDone");
        while (!_asyncLoading.isDone) { yield return null; }

    }

    private float _lastProgress = 0;
    private float _lastTweenerProgress = 0;
    private Tweener _progressTweener = null;

    private void Update()
    {
        if (_asyncLoading == null) { return; }
        var progress = GetProgress();
        if (_lastProgress <= progress)
        {
            _progressTweener?.Kill();
            _progressTweener = DOVirtual.Float(_lastTweenerProgress, progress, 0.3f, (f) =>
            {
                loadingForeground.rectTransform.SetSize(new Vector2(loadingBackground.rectTransform.GetSize().x * f, loadingForeground.rectTransform.GetSize().y));
                _lastTweenerProgress = f;
            });
            _lastProgress = progress;
        }
        if (infoText && infoText.text != Options.infoText) infoText.text = Options.infoText;
        if (infoTextTmp && infoTextTmp.text != Options.infoText) infoTextTmp.text = Options.infoText;
    }
}
