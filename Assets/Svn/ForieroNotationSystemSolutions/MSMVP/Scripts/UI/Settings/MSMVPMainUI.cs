/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using ForieroEngine.Music.NotationSystem;
using Michsky.UI.ModernUIPack;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MSMVPMainUI : MonoBehaviour
{
    public WindowManager windowManager;

    [Header("UI Scripts")] 
    public MSMVPSongsUI songsUI;
    public MSMVPCoursesUI coursesUI;
    public MSMVPPlaybackUI playbackUI;
    public MSMVPNotationUI notationUI;
    public MSMVPMetronomeUI metronomeUI;
    public MSMVPMidiUI midiUI;
    public MSMVPSubscriptionUI subscriptionUI;

    [Header("Sliders")]
    public Slider zoomSlider;
    public Slider speedSlider;

    [Header("PickupBar")] public Toggle pickupBarToggle;
    
    [Header("Hands")] 
    public Image leftHandIcon;
    public Toggle waitForInputToggle;
    public Image rightHandIcon;

    [Header("Notifications")] 
    public NotificationManager notifications;
    public ModalWindowManager result;
    
    private NSBehaviour NSB => NSBehaviour.instance;

    public void OnCloseClick()
    {
        windowManager.gameObject.SetActive(false);
    }
    
    private void Awake()
    {
        NSPlayback.OnPlaybackStateChanged += OnPlaybackStateChanged;
        NSPlayback.OnSongInitialized += OnSongInitialized;
    }

    private void OnDestroy()
    {
        NSPlayback.OnPlaybackStateChanged -= OnPlaybackStateChanged;
        NSPlayback.OnSongInitialized -= OnSongInitialized;
    }

    private void OnSongInitialized() => StartCoroutine(OnSongInitializedCoroutine());
    
    private IEnumerator OnSongInitializedCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        notifications.title = NSPlayback.Session.Name;
        notifications.description = Environment.NewLine + 
                                    "<b>Description</b>" + Environment.NewLine + Environment.NewLine 
                                    + NSPlayback.Session.Description
                                    + Environment.NewLine + Environment.NewLine
                                    + "<b>Instructions</b>" + Environment.NewLine + Environment.NewLine 
                                    + NSPlayback.Session.Instructions;
                                                           
        notifications.timer = 5f;
        notifications.UpdateUI();
        notifications.OpenNotification();
        LayoutRebuilder.MarkLayoutForRebuild(this.transform as RectTransform);
    }

    private void OnPlaybackStateChanged(NSPlayback.PlaybackState state)
    {
        switch (state)
        {
            case NSPlayback.PlaybackState.Pickup: break;
            case NSPlayback.PlaybackState.Playing: notifications.CloseNotification(); break;
            case NSPlayback.PlaybackState.WaitingForInput: break;
            case NSPlayback.PlaybackState.Pausing: break;
            case NSPlayback.PlaybackState.Stop: break;
            case NSPlayback.PlaybackState.Finished: result.OpenWindow(); break;
            case NSPlayback.PlaybackState.Undefined: break;
        }
    }


    public void Init()
    {
    }
    private IEnumerator Start()
    {
        waitForInputToggle.SetIsOnWithoutNotify(NSPlayback.Interaction.waitForInput);
        yield return new WaitWhile(() => NSB == null);
        yield return new WaitWhile(() => NSB.ns == null);
        playbackUI.Init();
    }

    public void OnSettingsClick()
    {
        windowManager.gameObject.SetActive(!windowManager.gameObject.activeSelf);
    }
    
    public void OnStopButtonClick()
    {
        NSPlayback.playbackState = NSPlayback.PlaybackState.Stop;
    }
    
    public void OnZoomSliderChange()
    {
        NSPlayback.Zoom = zoomSlider.value;
        // zoomText.text = zoomSlider.value.Round(1).ToString() + "x";
    }
    
    public void OnSpeedSliderChange()
    {
        NSPlayback.speed = speedSlider.value;
        //speedText.text = NSPlayback.speed.ToString("F1") + "x";
    }

    private bool _pianoHidden = false;

    public void OnPianoShowHide()
    {
        if (_pianoHidden)
        {
            Piano.SelectedController?.Show();
            _pianoHidden = !_pianoHidden;
        }
        else
        {
            Piano.SelectedController?.Hide();
            _pianoHidden = !_pianoHidden;
        }
    }

    public void OnLeftHandClick()
    {
        var b = NSPlayback.Interaction.midiChannelInteractive[1] = !NSPlayback.Interaction.midiChannelInteractive[1];
        NSPlayback.Interaction.midiChannelSound[1] = !b;
        leftHandIcon.color = b ? Color.green : Color.white;
        NSB.ns.SetStaveAlpha(1, b ? 1f : 0.5f);
    }
    public void OnWaitForClick()
    {
        NSPlayback.Interaction.waitForInput = waitForInputToggle.isOn;
    }

    public void OnRightHandClick()
    {
        var b = NSPlayback.Interaction.midiChannelInteractive[0] = !NSPlayback.Interaction.midiChannelInteractive[0];
        NSPlayback.Interaction.midiChannelSound[0] = !b;
        rightHandIcon.color = b ? Color.green : Color.white;
        NSB.ns.SetStaveAlpha(0, b ? 1f : 0.5f);
    }

    public void OnPickupBarCLick()
    {
        NSPlayback.PickupBar = pickupBarToggle.isOn;
    }

    private void Update()
    {
        if (Keyboard.current[Key.Escape].IsPressed() && windowManager.gameObject.activeSelf)
        {
            windowManager.gameObject.SetActive(false);
        }
    }

    public void OnNSTestSuiteClick()
    {
        if(NSTestSuite.Instance) NSTestSuite.Instance.OnWindowManagerClick();
    }

    public void OnFPSClick()
    {
        if(NSTestSuite.Instance) NSTestSuite.Instance.OnFPSClick();
    }
}
