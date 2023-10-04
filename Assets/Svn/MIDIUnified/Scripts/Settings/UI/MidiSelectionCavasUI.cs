using UnityEngine;

public class MidiSelectionCavasUI : MonoBehaviour
{
    public RectTransform midiRT;
    
    public void OnMidiShowHideClick()
    {
        midiRT.gameObject.SetActive(!midiRT.gameObject.activeSelf);        
    }
}
