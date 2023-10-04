/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;
using ForieroEngine.MIDIUnified.Synthesizer;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public partial class NSBehaviour : MonoBehaviour, IMidiSender
{
    public static NSBehaviour instance;

    [Tooltip("Midi Generator ID")]
    public string id;
    public string Id => id;
    public event ShortMessageEventHandler ShortMessageEvent;
    
    public NSDebug nsDebug;

    [Header("Fixed camera")]
    [Tooltip("Fixed camera for staves and vertical movement.")]
    public Camera fixedCamera;
    [HideInInspector] public RectTransform fixedCameraRT;

    [Header("Movable camera")]
    [Tooltip("Movable camera for notation objects and horizontal movement.")]
    public Camera movableCamera;
    [HideInInspector] public RectTransform movableCameraRT;

    [Header("Other settings")]
    public float scaleStep = 20f;
    public RoundingEnum lineWidthCalculation = RoundingEnum.Undefined;
    public RoundingEnum stemWidthCalculation = RoundingEnum.Undefined;
    public RoundingEnum barLineWidthCalculation = RoundingEnum.Undefined;

    [Header("Background canvas")]
    public Canvas backgroundCanvas;
    [HideInInspector] public RectTransform backgroundCanvasRT;
    
    [Header("Fixed canvas")]
    public Canvas fixedCanvas;
    [HideInInspector] public RectTransform fixedCanvasRT;
    public CanvasScaler fixedCanvasScaler;
    public RectTransform fixedPoolRT;
    public RectTransform fixedOverlayPoolRT;
    
    [Header("Movable canvas")]
    public Canvas movableCanvas;
    [HideInInspector] public RectTransform movableCanvasRT;
    public CanvasScaler movableCanvasScaler;
    public RectTransform movablePoolRT;
    public RectTransform movableOverlayPoolRT;
    
    [Header("Playback Updaters")]
    public NSRollingPlaybackUpdater rollingPlaybackUpdater;
    public NSRollingTickerPlaybackUpdater tickerPlaybackUpdater;
    public NSPageLayoutTickerPlaybackUpdater pageLayoutTickerPlaybackUpdater;

    [Header("System settings")]
    public NSSystemSettings defaultSystemSettings;
    public NSSystemSettings rollingLeftRightSystemSettings;
    public NSSystemSettings rollingUpDownSystemSettings;
    public NSSystemSettings rollingTickerSystemSettings;
    public NSSystemSettings pageLayoutTickerSystemSettings;
    public NSSystemSettings rhythmSystemSettings;

    [Header("RectTransforms")]
    public RectTransform backgroundDragRT;
    [HideInInspector] public RectTransform hitZoneRT;

    [Header("Time Providers")] 
    public NSAudioSourceTimeProvider audioSourceTimeProvider;
    
    public NS ns { get; private set; }
    
    Vector2 const_fixedReferenceResolution = Vector2.zero;
    Vector3 const_fixedCanvasScale = Vector3.one;

    Vector2 const_movableReferenceResolution = Vector2.zero;
    Vector3 const_movableCanvasScale = Vector3.one;
  
    public void EnableTimeProvider() { }
    public void DisableTimeProvider() { }

    public void ResetAllControllers() { if (NSSettingsStatic.midiOut) MidiOut.ResetAllControllers(); }
    public void AllSoundOff() { if (NSSettingsStatic.midiOut) MidiOut.AllSoundOff(); }
    
    public void ClearSynthQueue() => Synth.ClearQueue();
    
    public void SendMidiMessage(int channel, int command, int data1, int data2) => ShortMessageEvent?.Invoke(channel + command, data1, data2, -1);

    private void OnDestroy()
    {
        rollingPlaybackUpdater.blocked = true;
        rollingPlaybackUpdater.UnsubscribeEvents();
        tickerPlaybackUpdater.blocked = true;
        tickerPlaybackUpdater.UnsubscribeEvents();
        pageLayoutTickerPlaybackUpdater.blocked = true;
        pageLayoutTickerPlaybackUpdater.UnsubscribeEvents();
        
        NSPlayback.playbackState = NSPlayback.PlaybackState.Stop;
        NSPlayback.OnZoomChanged -= OnZoomChanged;

        if (ns != null)
        {
            ns.Dispose();
            ns = null;
        }

        instance = null;
        TimeProviders.Unregister(this);
    }

    private void Awake()
    {
        TimeProviders.Register(this);
        
        if (instance) { Debug.LogError("NSBehaviour instance is not null!!!"); }

        instance = this;
        
        backgroundCanvasRT = backgroundCanvas.transform as RectTransform;
        
        fixedCameraRT = fixedCamera.transform as RectTransform;
        fixedCanvasRT = fixedCanvas.transform as RectTransform;

        movableCameraRT = movableCamera.transform as RectTransform;
        movableCanvasRT = movableCanvas.transform as RectTransform;

        //overlayCanvasRT = overlayCanvas.transform as RectTransform;
        
        if (NS.debug)
        {
            Debug.Log("NSDebug : TRUE");
            nsDebug.debugRectTransform.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("NSDebug : FALSE");
            nsDebug.debugRectTransform.gameObject.SetActive(false);
        }

        NS.lineWidthCalculation = lineWidthCalculation;
        NS.stemWidthCalculation = stemWidthCalculation;
        NS.barLineWidthCalculation = barLineWidthCalculation;

        fixedCamera.backgroundColor = NSDisplaySettings.instance.backgroundColor;
    }

    private void Start()
    {
        rollingPlaybackUpdater = new NSRollingPlaybackUpdater(this);
        tickerPlaybackUpdater = new NSRollingTickerPlaybackUpdater(this);
        pageLayoutTickerPlaybackUpdater = new NSPageLayoutTickerPlaybackUpdater(this);
        NSPlayback.OnZoomChanged += OnZoomChanged;
        UpdateReferenceResolutions();
        MidiOut.InitPercussion();
        NSPlayback.InitSystem(SystemEnum.Default);
    }

    private void UpdateReferenceResolutions()
    {
        const_fixedReferenceResolution = fixedCanvasScaler.referenceResolution;
        const_fixedCanvasScale = fixedCanvas.transform.localScale;

        const_movableReferenceResolution = movableCanvasScaler.referenceResolution;
        const_movableCanvasScale = movableCanvas.transform.localScale;
    }

    public void Init(NS nsParam)
    {
        rollingPlaybackUpdater.blocked = true;
        rollingPlaybackUpdater.UnsubscribeEvents();
        tickerPlaybackUpdater.blocked = true;
        tickerPlaybackUpdater.UnsubscribeEvents();
        pageLayoutTickerPlaybackUpdater.blocked = true;
        pageLayoutTickerPlaybackUpdater.UnsubscribeEvents();
        

        NSPlayback.playbackState = NSPlayback.PlaybackState.Undefined;

        this.ns?.DestroyChildren();
        if (this.ns != null && this.ns != nsParam)
        {
            this.ns.Dispose();
            this.ns = null;
        }
        
        movableCameraRT.anchoredPosition = Vector2.zero;
        fixedCameraRT.anchoredPosition = Vector2.zero;

        if (nsParam.IsNull())
        {
            Debug.LogError("'ns' parameter can not be null!!!");
            return;
        }

        UpdateReferenceResolutions();

        NSPlayback.playbackMode = nsParam.nsSystemSettings.playbackMode;        
        NSPlayback.updateCameraMode = nsParam.nsSystemSettings.updateMode;
        NSPlayback.NSRollingPlayback.rollingMode = nsParam.nsSystemSettings.rollingMode;
        NSPlayback.NSTickerPlayback.tickerMode = nsParam.nsSystemSettings.tickerMode;

        this.ns = nsParam;
        NSSettingsStatic.Apply(this.ns.nsSystemSettings);

        SwitchCameraMode(NSSettingsStatic.canvasRenderMode, true);

        this.ns.Commit();

        switch (this.ns.nsSystemSettings.playbackMode)
        {
            case NSPlayback.PlaybackMode.Rolling:
                rollingPlaybackUpdater.blocked = false;
                rollingPlaybackUpdater.SubscribeEvents();
                break;
            case NSPlayback.PlaybackMode.Ticker:
                switch (NSPlayback.NSTickerPlayback.tickerMode)
                {
                    case NSPlayback.NSTickerPlayback.TickerMode.Screen:
                        tickerPlaybackUpdater.blocked = false;
                        tickerPlaybackUpdater.SubscribeEvents();
                        break;
                    case NSPlayback.NSTickerPlayback.TickerMode.PageLayout:
                        pageLayoutTickerPlaybackUpdater.blocked = false;
                        pageLayoutTickerPlaybackUpdater.SubscribeEvents();
                        break;
                }
                
                break;
        }

        this.ns.Init();
    }
}
