/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NSUITimeSlider : MonoBehaviour, IPointerDownHandler
{
    public Slider slider;

    private bool _block = false;
    private void Awake()
    {
        slider.onValueChanged.AddListener((f) => { 
            _block = true;
            NSPlayback.playbackState = NSPlayback.PlaybackState.Pausing;
            NSPlayback.Time.TimeNormalized = f; 
            _block = false; 
        });
        
        NSPlayback.OnTimeChanged += d => { if(!_block) slider.SetValueWithoutNotify(NSPlayback.Time.TimeNormalized); };
    }
    private void Update()
    {
        if (NSPlayback.playbackState == NSPlayback.PlaybackState.Playing) 
            slider.SetValueWithoutNotify(NSPlayback.Time.TimeNormalized);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (NSPlayback.playbackState == NSPlayback.PlaybackState.Playing) 
            NSPlayback.playbackState = NSPlayback.PlaybackState.Pausing;
    }
}
