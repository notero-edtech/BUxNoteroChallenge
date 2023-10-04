/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TLUnityMetronomeBpmText : MonoBehaviour
{
    public Text text;
    public TextMeshPro tmpText;
    public TextMeshProUGUI tmpuiText;

    int lastBpm = 0;

    void Start()
    {
        SetBpm(TLUnityMetronome.instance.bpm);
    }

    private void OnDestroy()
    {

    }

    void SetBpm(int bpm)
    {
        if (lastBpm == bpm) return;
        if (text) text.text = bpm.ToString();
        if (tmpText) tmpText.text = bpm.ToString();
        if (tmpuiText) tmpuiText.text = bpm.ToString();
        lastBpm = bpm;
    }

    private void Update()
    {
        SetBpm(TLUnityMetronome.instance.bpm);
    }
}
