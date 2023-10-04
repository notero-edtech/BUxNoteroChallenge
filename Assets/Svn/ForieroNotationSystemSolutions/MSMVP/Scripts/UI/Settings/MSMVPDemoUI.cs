/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using System;
using System.Collections;
using ForieroEngine.Music.NotationSystem;
using Michsky.UI.ModernUIPack;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MSMVPDemoUI : MonoBehaviour
{
    public bool advanced = false;
    public WindowManager windowManager;
    public NSSessionBank bank;
    public RectTransform menu;
    [Header("Toggles")]
    public Toggle musicSheetRightLeftToggle;
    public Toggle musicSheetTickerToggle;
    public Toggle topBottomToggle;
    public Toggle bottomTopToggle;
    
    public VideoPlayer videoPlayer;
   
    public Slider zoomSlider;
    
    private NSBehaviour nsBehaviour => NSBehaviour.instance;

    public void Init()
    {
    }

    private IEnumerator Start()
    {
        NSPlayback.OnPlaybackStateChanged += state =>
        {
            // switch (state)
            // {
            //     case NSPlayback.PlaybackState.Playing:
            //         videoPlayer.Play();
            //         break;
            //     case NSPlayback.PlaybackState.Stop:
            //         videoPlayer.Stop();
            //         videoPlayer.time = 0.5;
            //         videoPlayer.Prepare();
            //         break;
            // }
        };

        NSPlayback.onSystemChanged += OnSystemChanged;
        
        yield return new WaitWhile(() => nsBehaviour == null);
        yield return null;
        yield return new WaitWhile(() => nsBehaviour.ns == null);
        yield return null;
        musicSheetRightLeftToggle.isOn = true;
        topBottomToggle.isOn = false;
        bottomTopToggle.isOn = false;
        
        OnMusicSheetRightLeftToggleChange(true);
        
        topBottomToggle.onValueChanged.AddListener(OnTopBottomToggleChange);
        bottomTopToggle.onValueChanged.AddListener(OnBottomTopToggleChange);
        musicSheetRightLeftToggle.onValueChanged.AddListener(OnMusicSheetRightLeftToggleChange);
        musicSheetTickerToggle.onValueChanged.AddListener(OnMusicSheetTickerToggleChange);
        
        MIDISettings.instance.inputSettings.active = true;
    }

    private void OnDestroy()
    {
        NSPlayback.onSystemChanged -= OnSystemChanged;
    }

    private void OnSystemChanged(SystemEnum s)
    {
        switch (s)
        {
            case SystemEnum.Default: 
            case SystemEnum.PageLayoutTicker: 
            case SystemEnum.RollingTicker: 
            case SystemEnum.RollingLeftRight: 
            case SystemEnum.RollingRightLeft: 
            case SystemEnum.RollingTopBottom:
                menu.pivot = new Vector2(0.5f, 1);
                menu.anchorMin = new Vector2(0, 1);
                menu.anchorMax = new Vector2(1, 1);
                break;
            case SystemEnum.RollingBottomTop: 
                menu.pivot = new Vector2(0.5f, 0);
                menu.anchorMin = new Vector2(0, 0);
                menu.anchorMax = new Vector2(1, 0);
                break;
            case SystemEnum.Custom: break;
            case SystemEnum.Undefined: break;
        }
            
        zoomSlider.minValue = NSSettingsStatic.zoomMin;
        zoomSlider.maxValue = NSSettingsStatic.zoomMax;
        zoomSlider.value = NSPlayback.Zoom;
    }

    public void OnTopBottomToggleChange(bool b)
    {
        if (!b) return;
        NSPlayback.InitSystem(SystemEnum.RollingTopBottom);
    }
    
    public void OnBottomTopToggleChange(bool b)
    {
         if (!b) return;
         NSPlayback.InitSystem(SystemEnum.RollingBottomTop);
    }

    public void OnMusicSheetRightLeftToggleChange(bool b)
    {
        if (!b) return;
        NSPlayback.InitSystem(SystemEnum.RollingRightLeft);
    }
    
    private void OnMusicSheetTickerToggleChange(bool b)
    {
        if (!b) return;
        NSPlayback.InitSystem(SystemEnum.RollingTicker);
    }
    
    public void OnSampleClick(int index)
    {
        windowManager.gameObject.SetActive(false);
        NSPlayback.Session.Init(bank.sessions[index]);
    }
    
    public void OnBackClick() { windowManager.gameObject.SetActive(false); }
    public void OnQuitClick() { Application.Quit(); }
}
