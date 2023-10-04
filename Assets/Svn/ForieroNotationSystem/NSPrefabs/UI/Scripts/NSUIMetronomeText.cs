/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;
using TMPro;
using UnityEngine;

public class NSUIMetronomeText : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    private int denominator = 4;
    private int beat = 0;
    private void Awake()
    {
        NSPlayback.OnBeatChanged += OnBeat;
        NSPlayback.OnTimeSignatureChanged += OnTimeSignatureChanged;
    }
    private void OnTimeSignatureChanged(int partIndex, int staveIndex, NSTimeSignature.Options options)
    {
        this.denominator = options.timeSignatureStruct.denominator;
        UpdateText();
    }
    private void OnBeat(NSPlayback.Beat beat)
    {
        this.beat = beat.number;
        UpdateText();
    }
    private void UpdateText() => tmp.text = beat + " / " + denominator;
    private void Start() { tmp.text = 0 + " / " + denominator; }
    private void OnDestroy()
    {
        NSPlayback.OnBeatChanged -= OnBeat;
        NSPlayback.OnTimeSignatureChanged -= OnTimeSignatureChanged;
    }
}
