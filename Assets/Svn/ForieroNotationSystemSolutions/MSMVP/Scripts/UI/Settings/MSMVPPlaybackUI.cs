/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using ForieroEngine;
using ForieroEngine.MIDIUnified;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;
using ForieroEngine.Music.NotationSystem.Systems;
using Michsky.UI.ModernUIPack;
using UnityEngine;
using UnityEngine.UI;

public class MSMVPPlaybackUI : MonoBehaviour
{
    public Toggle waitForInputToggle;
    public CustomDropdown theorySystemDropdown;

    [Header("Display Toggles")] public Toggle rightToLeftToggle;
    public Toggle leftToRightToggle;
    public Toggle topToBottomToggle;
    public Toggle bottomToTopToggle;
    
    public Slider zoomSlider;

    private NSBehaviour nsBehaviour => NSBehaviour.instance;
    private bool initialized = false;

    public void Init()
    {
        if (initialized) return;
        initialized = true;
        
        InitTheorySystem();
        InitToggles();
        
        MIDISettings.instance.inputSettings.active = true;
    }

    private void OnEnable()
    {
        waitForInputToggle.SetIsOnWithoutNotify(NSPlayback.Interaction.waitForInput);
    }

    private void InitTheorySystem()
    {
        var values = System.Enum.GetNames(typeof(ToneNamesEnum));
        theorySystemDropdown.CreateNewItemFast("None", null);
        foreach (var v in values) { if(v != "Undefined") theorySystemDropdown.CreateNewItemFast(v, null); }
        theorySystemDropdown.SetupDropdown();

        theorySystemDropdown.index = NSSettingsStatic.noteNamesEnum == ToneNamesEnum.Undefined
            ? 0
            : NSSettingsStatic.noteNamesEnum.ToInt() + 1;
        theorySystemDropdown.SetupDropdown();
        
        theorySystemDropdown.dropdownEvent.AddListener(OnTheorySystemDropdownChange);
    }

    private void OnTheorySystemDropdownChange(int v)
    {
        if (v == 0) NSSettingsStatic.noteNamesEnum = NSSettingsStatic.keysNamesEnum = ToneNamesEnum.Undefined;
        else NSSettingsStatic.noteNamesEnum = NSSettingsStatic.keysNamesEnum = (ToneNamesEnum)(v - 1);
        
        if (!nsBehaviour || !nsBehaviour.ns) return;
        
        var notes = nsBehaviour.ns.GetObjectsOfType<NSNote>(true);
        foreach (var note in notes) note.UpdateText();
        
        nsBehaviour.rollingPlaybackUpdater.InitSignatures();
    }

    private void InitToggles()
    {
        rightToLeftToggle.SetIsOnWithoutNotify(true);
        leftToRightToggle.SetIsOnWithoutNotify(false);
        topToBottomToggle.SetIsOnWithoutNotify(false);
        bottomToTopToggle.SetIsOnWithoutNotify(false);
        waitForInputToggle.SetIsOnWithoutNotify(NSPlayback.Interaction.waitForInput);
        OnRight2LeftToggleChange(true);
        rightToLeftToggle.onValueChanged.AddListener(OnRight2LeftToggleChange);
        leftToRightToggle.onValueChanged.AddListener(OnLeft2RightToggleChange);
        topToBottomToggle.onValueChanged.AddListener(OnTop2BottomToggleChange);
        bottomToTopToggle.onValueChanged.AddListener(OnBottom2TopToggleChange);
        waitForInputToggle.onValueChanged.AddListener(OnWaitForInputToggleChange);
    }

    private void OnWaitForInputToggleChange(bool b)
    {
        this.GetComponentInParent<MSMVPMainUI>().waitForInputToggle.isOn = b;
    }

    private void OnTop2BottomToggleChange(bool b)
    {
        if (!b) return;
        NSPlayback.InitSystem(SystemEnum.RollingTopBottom);
        CommonInit();
    }

    private void OnBottom2TopToggleChange(bool b)
    {
        if (!b) return;
        NSPlayback.InitSystem(SystemEnum.RollingTopBottom);
        CommonInit();
    }

    private void OnRight2LeftToggleChange(bool b)
    {
        if (!b) return;
        NSPlayback.InitSystem(SystemEnum.RollingLeftRight);
        CommonInit();
    }

    private void OnLeft2RightToggleChange(bool b)
    {
        if (!b) return;
        NSPlayback.InitSystem(SystemEnum.RollingLeftRight);
        CommonInit();
    }

    private void CommonInit()
    {
        zoomSlider.minValue = NSSettingsStatic.zoomMin;
        zoomSlider.maxValue = NSSettingsStatic.zoomMax;
        zoomSlider.value = NSPlayback.Zoom;
    }

    private IEnumerator Start()
    {
        yield return null;
    }
}
