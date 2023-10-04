/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem;
using TMPro;
using UnityEngine;

public class NSUIMeasureText : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    private void Awake()
    {
        NSPlayback.OnMeasureChanged += OnMeasureChanged;   
    }

    private void Start()
    {
        tmp.text = "0";
    }

    private void OnDestroy()
    {
        NSPlayback.OnMeasureChanged -= OnMeasureChanged;
    }

    private void OnMeasureChanged(NSPlayback.Measure m)
    {
        tmp.text = m.number.ToString();
    }
    
}
