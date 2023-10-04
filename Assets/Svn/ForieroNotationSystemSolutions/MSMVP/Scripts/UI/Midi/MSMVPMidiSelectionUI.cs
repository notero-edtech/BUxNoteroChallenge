/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;

using ForieroEngine.Extensions;

public class MSMVPMidiSelectionUI : MonoBehaviour
{

    int _minWidth = 600;
    int _minHeight = 100;

    public int minWidth = 600;
    public int widthMargin = 10;
    public int minHeight = 100;

    public RectTransform midiPanel;
        
    [Header("Midi IN")]
    public RectTransform inPanel;
    public Toggle inActiveToggle;
    public Toggle inOutToggle;
    public Toggle inThroughToggle;
    public Toggle inSynthToggle;
    public Toggle inThreadedToggle;
    
    [Header("Midi OUT")]
    public RectTransform outPanel;
    public Toggle outActiveToggle;
    public Toggle outSynthToggle;
    public Toggle outThreadedToggle;
    
    [Header("Midi Button")]
    public Color connectedColor;
    public Color disconnectedColor;
    public GameObject PREFAB_MidiDeviceButton;

    private List<MSMVPMidiSelectionUIDeviceButton> INs = new ();
    private List<MSMVPMidiSelectionUIDeviceButton> OUTs = new ();

    IEnumerator Start()
    {
        yield return new WaitWhile(() => !MIDI.initialized);
        UpdateToggles();
        Init();
        HookToggles();
    }

    public void OnInActiveToggle(bool v) => MIDISettings.instance.inputSettings.active = v;
    public void OnInOutToggle(bool v) => MIDISettings.instance.inputSettings.midiOut = v;
    public void OnInThroughToggle(bool v) => MIDISettings.instance.inputSettings.through = v;
    public void OnInSynthToggle(bool v) => MIDISettings.instance.inputSettings.synth = v;
    public void OnInThreadedToggle(bool v) => MIDISettings.instance.inputSettings.threaded = v;

    public void OnOutActiveToggle(bool v) => MIDISettings.instance.outputSettings.active = v;
    public void OnOutSynthToggle(bool v) => MIDISettings.instance.outputSettings.synth = v;
    public void OnOutThreadedToggle(bool v) => MIDISettings.instance.outputSettings.threaded = v;

    private void HookToggles()
    {
        inActiveToggle.onValueChanged.AddListener(OnInActiveToggle);
        inOutToggle.onValueChanged.AddListener(OnInOutToggle);
        inThroughToggle.onValueChanged.AddListener(OnInThroughToggle);
        inSynthToggle.onValueChanged.AddListener(OnInSynthToggle);
        inThreadedToggle.onValueChanged.AddListener(OnInThreadedToggle);
        
        outActiveToggle.onValueChanged.AddListener(OnOutActiveToggle);
        outSynthToggle.onValueChanged.AddListener(OnOutSynthToggle);
        outThreadedToggle.onValueChanged.AddListener(OnOutThreadedToggle);
    }
    private void UpdateToggles()
    {
        inActiveToggle.isOn = MIDISettings.instance.inputSettings.active;
        inOutToggle.isOn = MIDISettings.instance.inputSettings.midiOut;
        inThroughToggle.isOn = MIDISettings.instance.inputSettings.through;
        inSynthToggle.isOn = MIDISettings.instance.inputSettings.synth;
        inThreadedToggle.isOn = MIDISettings.instance.inputSettings.threaded;

        outActiveToggle.isOn = MIDISettings.instance.outputSettings.active;
        outSynthToggle.isOn = MIDISettings.instance.outputSettings.synth;
        outThreadedToggle.isOn = MIDISettings.instance.outputSettings.threaded;
    }

    private void Update()
    {
        //UpdateToggles();
    }

    void Init()
    {
        UpdateToggles();

        foreach (MSMVPMidiSelectionUIDeviceButton button in INs) { Destroy(button.gameObject); }

        INs = new ();

        foreach (MSMVPMidiSelectionUIDeviceButton button in OUTs) { Destroy(button.gameObject); }

        OUTs = new ();

        int count = MidiINPlugin.deviceNames.Count > MidiOUTPlugin.deviceNames.Count ? MidiINPlugin.deviceNames.Count : MidiOUTPlugin.deviceNames.Count;

        _minHeight = count * (int)PREFAB_MidiDeviceButton.GetComponent<LayoutElement>().preferredHeight + 100;

        if (_minHeight < minHeight) { _minHeight = minHeight; }

        _minWidth = minWidth;

        for (int i = 0; i < MidiINPlugin.GetDeviceCount(); i++)
        {
            var go = Instantiate(PREFAB_MidiDeviceButton, inPanel, false);
            var button = go.GetComponent<MSMVPMidiSelectionUIDeviceButton>();
            button.device = MSMVPMidiSelectionUIDeviceButton.Device.In;
            button.midiSelectionUI = this;
            button.text.text = MidiINPlugin.GetDeviceName(i);

            bool connected = false;

            foreach (var device in MidiINPlugin.connectedDevices.Where(device => device.name == button.text.text)) { connected = true; }

            button.connected = connected;

            button.image.color = connected ? connectedColor : disconnectedColor;
            button.switchButton.isOn = connected;
            button.switchButton.UpdateUI();
            
            INs.Add(button);

            int preferredWidth = (int)button.text.preferredWidth;
            if (preferredWidth > _minWidth / 2) { _minWidth = 2 * preferredWidth; }
        }

        for (int i = 0; i < MidiOUTPlugin.GetDeviceCount(); i++)
        {
            var go = Instantiate(PREFAB_MidiDeviceButton, outPanel, false);
            var button = go.GetComponent<MSMVPMidiSelectionUIDeviceButton>();
            button.device = MSMVPMidiSelectionUIDeviceButton.Device.Out;
            button.midiSelectionUI = this;
            button.text.text = MidiOUTPlugin.GetDeviceName(i);

            bool connected = false;

            foreach (var device in MidiOUTPlugin.connectedDevices.Where(device => device.name == button.text.text)) { connected = true; }

            button.connected = connected;

            button.image.color = connected ? connectedColor : disconnectedColor;
            button.switchButton.isOn = connected;
            //button.switchButton.AnimateSwitch();
            
            OUTs.Add(button);

            int preferredWidth = (int)button.text.preferredWidth;
            if (preferredWidth > _minWidth / 2) { _minWidth = 2 * preferredWidth; }
        }

        _minWidth += 2 * widthMargin;
        //_minHeight += 2 * heightMargin;

        //midiPanel.SetSize(new Vector2(_minWidth, _minHeight));
    }

    public void Reset()
    {
        MidiOut.AllSoundOff();
        MidiOut.ResetAllControllers();
    }

    public void Refresh()
    {
        MIDI.RefreshMidiIO();

        Init();
    }

    public void OnDeviceClick(MSMVPMidiSelectionUIDeviceButton button)
    {
        //Debug.Log("Device : " + button.device.ToString() + " " + button.text.text);
        if (button.connected)
        {
            switch (button.device)
            {
                case MSMVPMidiSelectionUIDeviceButton.Device.In:
                    MidiINPlugin.DisconnectDeviceByName(button.text.text);
                    button.image.color = disconnectedColor;
                    button.connected = false;
                    button.switchButton.isOn = true;
                    button.switchButton.AnimateSwitch();
                    MidiINPlugin.StoreConnections();
                    break;
                case MSMVPMidiSelectionUIDeviceButton.Device.Out:
                    MidiOUTPlugin.DisconnectDeviceByName(button.text.text);
                    button.image.color = disconnectedColor;
                    button.connected = false;
                    button.switchButton.isOn = true;
                    button.switchButton.AnimateSwitch();
                    MidiOUTPlugin.StoreConnections();
                    break;
            }
        }
        else
        {
            switch (button.device)
            {
                case MSMVPMidiSelectionUIDeviceButton.Device.In:
                    if (MidiINPlugin.ConnectDeviceByName(button.text.text) != null)
                    {
                        button.image.color = connectedColor;
                        button.connected = true;
                        button.switchButton.isOn = false;
                        button.switchButton.AnimateSwitch();
                        MidiINPlugin.StoreConnections();
                    }
                    break;
                case MSMVPMidiSelectionUIDeviceButton.Device.Out:
                    if (MidiOUTPlugin.ConnectDeviceByName(button.text.text) != null)
                    {
                        button.image.color = connectedColor;
                        button.connected = true;
                        button.switchButton.isOn = false;
                        button.switchButton.AnimateSwitch();
                        MidiOUTPlugin.StoreConnections();
                    }
                    break;
            }
        }

    }
}
