/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TLUnityMetronomeBeatText : MonoBehaviour
{
    public Text text;
    public TextMeshPro tmpText;
    public TextMeshProUGUI tmpuiText;

    void Awake()
    {
        TLUnityMetronome.instance.OnBeat += OnBeat;
        TLUnityMetronome.instance.OnStop += OnStop;
        OnBeat(TLUnityMetronome.instance.beats, 0);
        lastBeats = TLUnityMetronome.instance.beats;
    }

    private void OnDestroy()
    {
        TLUnityMetronome.instance.OnBeat -= OnBeat;
        TLUnityMetronome.instance.OnStop -= OnStop;
    }

    void OnBeat(int beat, double beatDuration)
    {
        if (text) text.text = beat.ToString();
        if (tmpText) tmpText.text = beat.ToString();
        if (tmpuiText) tmpuiText.text = beat.ToString();
    }

    void OnStop()
    {
        OnBeat(TLUnityMetronome.instance.beats, 0);
    }

    int lastBeats = 0;

    private void Update()
    {
        if (TLUnityMetronome.instance.state == TLUnityMetronome.State.Stop && lastBeats != TLUnityMetronome.instance.beats)
        {
            OnBeat(TLUnityMetronome.instance.beats, 0);
        }
    }
}
