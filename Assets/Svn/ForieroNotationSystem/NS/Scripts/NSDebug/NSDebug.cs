/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;
using TMPro;

public class NSDebug : MonoBehaviour
{
    public static NSBehaviour NSB => NSBehaviour.instance;

    public bool logNotImplementedXmlNode = true;
    public RectTransform debugRectTransform;
    public TextMeshProUGUI statisticsText;
    public TextMeshProUGUI settingsText;
    public TextMeshProUGUI playbackText;

    private int _gcCollectionCount = 0;
    private TextMode _lastTextMode = TextMode.Text;
    private CanvasRenderMode _lastCanvasRenderMode = CanvasRenderMode.Screen;
    private bool _statisticsChanged = false;

    private void Awake()
    {
        NS.logNotImplementedXmlNode = logNotImplementedXmlNode;
        NS.OnStatisticsChanged += () => _statisticsChanged = true;
    }

    IEnumerator Start()
    {
        yield return null;
        yield return new WaitUntil(() => NSB != null);
        yield return new WaitUntil(() => NSB.ns != null);

        _lastTextMode = NSSettingsStatic.textMode;
        _lastCanvasRenderMode = NSSettingsStatic.canvasRenderMode;

        UpdateSettingsText();
        UpdateStatisticsText();
        UpdateMovableCameraText();

        NSPlayback.OnMeasureChanged += NSPlayback_OnMeasureChanged;
        NSPlayback.OnBeatChanged += NSPlayback_OnBeatChanged;
    }

    private void OnDestroy()
    {
        NSPlayback.OnMeasureChanged -= NSPlayback_OnMeasureChanged;
        NSPlayback.OnBeatChanged -= NSPlayback_OnBeatChanged;
    }

    void NSPlayback_OnMeasureChanged(NSPlayback.Measure measure)
    {
        UpdateMovableCameraText();
    }

    void NSPlayback_OnBeatChanged(NSPlayback.Beat beat)
    {
        UpdateMovableCameraText();
    }

    private float _cumulatedTime = 0f;

    private void Update()
    {
        if (NSB.ns == null) return;

        _cumulatedTime += Time.deltaTime;

        if (_gcCollectionCount != System.GC.CollectionCount(0))
        {
            _gcCollectionCount = System.GC.CollectionCount(0);
            UpdateSettingsText();
        }

        if (_lastTextMode != NSSettingsStatic.textMode)
        {
            _lastTextMode = NSSettingsStatic.textMode;
            UpdateSettingsText();
        }

        if (_lastCanvasRenderMode != NSSettingsStatic.canvasRenderMode)
        {
            _lastCanvasRenderMode = NSSettingsStatic.canvasRenderMode;
            UpdateSettingsText();
        }

        if (_statisticsChanged)
        {
            UpdateStatisticsText();
            _statisticsChanged = false;
        }

        if (_cumulatedTime > 0.1f)
        {
            UpdateMovableCameraText();
            _cumulatedTime = 0;
        }
    }

    public void UpdateSettingsText()
    {
        settingsText.text =
        "Target Frame Rate : " + Application.targetFrameRate.ToString() + System.Environment.NewLine +
        "VSync : " + QualitySettings.vSyncCount.ToString() + System.Environment.NewLine +
        "AA : " + QualitySettings.antiAliasing.ToString() + "x" + System.Environment.NewLine +
        "GC : " + _gcCollectionCount.ToString() + System.Environment.NewLine +
        "Camera Mode : " + NSSettingsStatic.canvasRenderMode.ToString() + System.Environment.NewLine +
        "Text Mode : " + NSSettingsStatic.textMode.ToString();
    }

    public void UpdateStatisticsText()
    {
        statisticsText.text =
        "Total : " + NSObject.statisticTotalObjectCount.ToString() + System.Environment.NewLine +
        "Objects : " + NSObject.statisticObjectsCount.ToString() + System.Environment.NewLine +
        "Images : " + NSObject.statisticImagesCount.ToString() + System.Environment.NewLine +
        "RawImages : " + NSObject.statisticRawImagesCount.ToString() + System.Environment.NewLine +
        "Texts : " + NSObject.statisticTextsCount.ToString() + System.Environment.NewLine +
        "SMuFL : " + NSObject.statisticSMuFLsCount.ToString() + System.Environment.NewLine +
        "Prefabs : " + NSObject.statisticPrefabsCount.ToString() + System.Environment.NewLine +
        "Vectors : " + NSObject.statisticVectorsCount.ToString();


    }

    public void UpdateMovableCameraText()
    {
        playbackText.text =
                             "x : " + NSB.movableCameraRT.anchoredPosition.x.ToString("F1") + System.Environment.NewLine +
                             "y : " + NSB.movableCameraRT.anchoredPosition.y.ToString("F1") + System.Environment.NewLine +
                             "Pixels Per Seconds : " + NSPlayback.NSRollingPlayback.pixelsPerSecond.ToString("F1") + "px/s" + System.Environment.NewLine +
                             "Time : " + NSPlayback.Time.time.ToString("0.000") + "s" + System.Environment.NewLine +
                             "DspTime : " + NSPlayback.Time.DSP.time.ToString("0.000") + "s" + System.Environment.NewLine +
                             "Total Time : " + NSPlayback.Time.TotalTime.ToString("F1") + "s" + System.Environment.NewLine +
                             "Pixel Position : " + NSPlayback.NSRollingPlayback.pixelPosition.ToString("F1") + "spx" + System.Environment.NewLine +
                             "Rolling Mode : " + NSPlayback.NSRollingPlayback.rollingMode.ToString() + System.Environment.NewLine +
                             "Playback State : " + NSPlayback.playbackState.ToString() + System.Environment.NewLine +
                             "Playback Mode : " + NSPlayback.playbackMode.ToString() + System.Environment.NewLine +
                             "Update Mode : " + NSPlayback.updateCameraMode.ToString() + System.Environment.NewLine +
                             "Measure : " + (NSPlayback.measure == null ? "NULL" : NSPlayback.measure.number.ToString()) + System.Environment.NewLine +
                             "Beat : " + (NSPlayback.beat == null ? "NULL" : NSPlayback.beat.number.ToString()) + System.Environment.NewLine +
                             "Normalized Beat Time : " + NSPlayback.NormalizedBeatTime.ToString("F2") + System.Environment.NewLine +
                             "Normalized Measure Time : " + NSPlayback.NormalizedMeasureTime.ToString("F2") + System.Environment.NewLine;

    }
}
