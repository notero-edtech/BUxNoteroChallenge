﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;

using ForieroEngine.Extensions;

public class MidiSelectionUI : MonoBehaviour
{

    int _minWidth = 600;
    int _minHeight = 100;

    public int minWidth = 600;
    public int widthMargin = 10;
    public int minHeight = 100;

    public RectTransform midiPanel;
    public RectTransform inPanel;
    public RectTransform outPanel;

    public Toggle inActiveToggle;
    public Toggle inOutToggle;
    public Toggle inThroughToggle;
    public Toggle inSynthToggle;
    public Toggle inThreadedToggle;

    public Toggle outActiveToggle;
    public Toggle outSynthToggle;
    public Toggle outThreadedToggle;

    public Color connectedColor;
    public Color disconnectedColor;

    public GameObject PREFAB_MidiDeviceButton;

    List<MidiSelectionUIDeviceButton> INs = new List<MidiSelectionUIDeviceButton>();
    List<MidiSelectionUIDeviceButton> OUTs = new List<MidiSelectionUIDeviceButton>();

    IEnumerator Start()
    {
        yield return new WaitWhile(() => !MIDI.initialized);
        //yield return null;
        Init();
    }

    public void OnInActiveToggle() => MIDISettings.instance.inputSettings.active = inActiveToggle.isOn;
    public void OnInOutToggle() => MIDISettings.instance.inputSettings.midiOut = inOutToggle.isOn;
    public void OnInThroughToggle() => MIDISettings.instance.inputSettings.through = inThroughToggle.isOn;
    public void OnInSynthToggle() => MIDISettings.instance.inputSettings.synth = inSynthToggle.isOn;
    public void OnInThreadedToggle() => MIDISettings.instance.inputSettings.threaded = inOutToggle.isOn;

    public void OnOutActiveToggle() => MIDISettings.instance.outputSettings.active = outActiveToggle.isOn;
    public void OnOutSynthToggle() => MIDISettings.instance.outputSettings.synth = outSynthToggle.isOn;
    public void OnOutThreadedToggle() => MIDISettings.instance.outputSettings.threaded = outThreadedToggle.isOn;

    void UpdateToggles()
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

        foreach (MidiSelectionUIDeviceButton button in INs)
        {
            Destroy(button.gameObject);
        }

        INs = new List<MidiSelectionUIDeviceButton>();

        foreach (MidiSelectionUIDeviceButton button in OUTs)
        {
            Destroy(button.gameObject);
        }

        OUTs = new List<MidiSelectionUIDeviceButton>();

        int count = MidiINPlugin.deviceNames.Count > MidiOUTPlugin.deviceNames.Count ? MidiINPlugin.deviceNames.Count : MidiOUTPlugin.deviceNames.Count;

        _minHeight = count * (int)PREFAB_MidiDeviceButton.GetComponent<LayoutElement>().preferredHeight + 100;

        if (_minHeight < minHeight)
        {
            _minHeight = minHeight;
        }

        _minWidth = minWidth;

        for (int i = 0; i < MidiINPlugin.GetDeviceCount(); i++)
        {
            var go = Instantiate(PREFAB_MidiDeviceButton);

            go.transform.SetParent(inPanel, false);
            MidiSelectionUIDeviceButton button = go.GetComponent<MidiSelectionUIDeviceButton>();
            button.device = MidiSelectionUIDeviceButton.Device.IN;
            button.midiSelectionUI = this;
            button.text.text = MidiINPlugin.GetDeviceName(i);

            bool connected = false;

            foreach (MidiDevice device in MidiINPlugin.connectedDevices)
            {
                if (device.name == button.text.text)
                {
                    connected = true;
                }
            }

            button.connected = connected;

            if (connected)
            {
                button.image.color = connectedColor;
            }
            else
            {
                button.image.color = disconnectedColor;
            }

            INs.Add(button);

            int preferredWidth = (int)button.text.preferredWidth;
            if (preferredWidth > _minWidth / 2)
            {
                _minWidth = 2 * preferredWidth;
            }
        }

        for (int i = 0; i < MidiOUTPlugin.GetDeviceCount(); i++)
        {
            var go = Instantiate(PREFAB_MidiDeviceButton);
            go.transform.SetParent(outPanel, false);
            MidiSelectionUIDeviceButton button = go.GetComponent<MidiSelectionUIDeviceButton>();
            button.device = MidiSelectionUIDeviceButton.Device.OUT;
            button.midiSelectionUI = this;
            button.text.text = MidiOUTPlugin.GetDeviceName(i);

            bool connected = false;

            foreach (MidiDevice device in MidiOUTPlugin.connectedDevices)
            {
                if (device.name == button.text.text)
                {
                    connected = true;
                }
            }

            button.connected = connected;

            if (connected)
            {
                button.image.color = connectedColor;
            }
            else
            {
                button.image.color = disconnectedColor;
            }

            OUTs.Add(button);

            int preferredWidth = (int)button.text.preferredWidth;
            if (preferredWidth > _minWidth / 2)
            {
                _minWidth = 2 * preferredWidth;
            }
        }

        _minWidth += 2 * widthMargin;
        //_minHeight += 2 * heightMargin;

        midiPanel.SetSize(new Vector2(_minWidth, _minHeight));
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

    public void OnDeviceClick(MidiSelectionUIDeviceButton button)
    {
        //Debug.Log("Device : " + button.device.ToString() + " " + button.text.text);
        if (button.connected)
        {
            switch (button.device)
            {
                case MidiSelectionUIDeviceButton.Device.IN:
                    MidiINPlugin.DisconnectDeviceByName(button.text.text);
                    button.image.color = disconnectedColor;
                    button.connected = false;
                    MidiINPlugin.StoreConnections();
                    break;
                case MidiSelectionUIDeviceButton.Device.OUT:
                    MidiOUTPlugin.DisconnectDeviceByName(button.text.text);
                    button.image.color = disconnectedColor;
                    button.connected = false;
                    MidiOUTPlugin.StoreConnections();
                    break;
            }
        }
        else
        {
            switch (button.device)
            {
                case MidiSelectionUIDeviceButton.Device.IN:
                    if (MidiINPlugin.ConnectDeviceByName(button.text.text) != null)
                    {
                        button.image.color = connectedColor;
                        button.connected = true;
                        MidiINPlugin.StoreConnections();
                    }
                    break;
                case MidiSelectionUIDeviceButton.Device.OUT:
                    if (MidiOUTPlugin.ConnectDeviceByName(button.text.text) != null)
                    {
                        button.image.color = connectedColor;
                        button.connected = true;
                        MidiOUTPlugin.StoreConnections();
                    }
                    break;
            }
        }

    }
}
