/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Michsky.UI.ModernUIPack;
using TMPro;

public class MSMVPMidiSelectionUIDeviceButton : MonoBehaviour
{
	public enum Device { In, Out }
	public MSMVPMidiSelectionUI midiSelectionUI;
	public Device device = Device.In;
	public TextMeshProUGUI text;
	public Image image;

	public bool connected = false;

	public Button button;
	public SwitchManager switchButton;
   
	public void OnClick () => midiSelectionUI.OnDeviceClick (this);
}
