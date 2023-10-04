/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TLUnityMetronomeBeatsText : MonoBehaviour
{
    public Text text;
    public TextMeshPro tmpText;
    public TextMeshProUGUI tmpuiText;

    private void Awake() { OnBeat(TLUnityMetronome.instance.beats); }
  
    void OnBeat(int beats)
    {
        _beats = beats;
        if (text) text.text = beats.ToString();
        if (tmpText) tmpText.text = beats.ToString();
        if (tmpuiText) tmpuiText.text = beats.ToString();
    }
    
    private int _beats = 0;

    private void Update()
    {
        if (_beats != TLUnityMetronome.instance.beats) { OnBeat(TLUnityMetronome.instance.beats); }
    }
}
