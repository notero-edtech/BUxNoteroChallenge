/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NSUIPlayPauseButton : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage;

    public Sprite playSprite;
    public Sprite pauseSprite;

    public Color pickupColor = Color.red;
    public Color playColor = Color.green;
    public Color stopColor = Color.white;
    public Color pauseColor = Color.yellow;

    private void Awake() { NSPlayback.OnPlaybackStateChanged += OnPlaybackStateChanged; }
    private void OnDestroy() { NSPlayback.OnPlaybackStateChanged -= OnPlaybackStateChanged; }

    private void OnPlaybackStateChanged(NSPlayback.PlaybackState state)
    {
        switch (state)
        {
            case NSPlayback.PlaybackState.Pickup:
                iconImage.sprite = playSprite;
                iconImage.color = pickupColor;
                break;
            case NSPlayback.PlaybackState.Playing:
                iconImage.sprite = playSprite;
                iconImage.color = playColor;
                break;
            case NSPlayback.PlaybackState.WaitingForInput:
            case NSPlayback.PlaybackState.Pausing:
                iconImage.sprite = pauseSprite;
                iconImage.color = pauseColor;
                break;
            case NSPlayback.PlaybackState.Stop:
            case NSPlayback.PlaybackState.Finished:
            case NSPlayback.PlaybackState.Undefined:
                iconImage.sprite = playSprite;
                iconImage.color = stopColor;
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (NSPlayback.playbackState)
        {
            case NSPlayback.PlaybackState.Pickup: NSPlayback.playbackState = NSPlayback.PlaybackState.Stop; break;
            case NSPlayback.PlaybackState.Playing: NSPlayback.playbackState = NSPlayback.PlaybackState.Pausing; break;
            case NSPlayback.PlaybackState.WaitingForInput:
            case NSPlayback.PlaybackState.Pausing: 
            case NSPlayback.PlaybackState.Stop:
            case NSPlayback.PlaybackState.Undefined: 
                if(NSPlayback.Time.time == 0 && NSPlayback.PickupBar) NSPlayback.playbackState = NSPlayback.PlaybackState.Pickup;
                else NSPlayback.playbackState = NSPlayback.PlaybackState.Playing; 
                break;
        }
    }
}
