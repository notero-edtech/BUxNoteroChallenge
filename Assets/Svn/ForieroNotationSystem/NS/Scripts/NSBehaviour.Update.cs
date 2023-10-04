/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.MIDIUnified.Plugins;
using ForieroEngine.Music.NotationSystem;
using UnityEngine;

public partial class NSBehaviour : MonoBehaviour, ITimeProvider
{       
    private bool _wasPlaying = false;
    private float _time;

    private void OnApplicationFocus(bool focus)
    {
        // TODO: discuss with Marek about external option for this
        return;

        switch(focus)
        {
            case true:
                if (_wasPlaying)
                {
                    Debug.Log("OnApplicationFocus(true) - NSPlayback.playbackState = Playing");
                    NSPlayback.playbackState = NSPlayback.PlaybackState.Playing;
                }

                break;
            case false:
                _wasPlaying = NSPlayback.playbackState == NSPlayback.PlaybackState.Playing; 
                if (_wasPlaying)
                {
                    Debug.Log("OnApplicationFocus(false) - NSPlayback.playbackState = Pausing");
                    NSPlayback.playbackState = NSPlayback.PlaybackState.Pausing;
                }
                break;
        }
    }
        
    private void UpdateTime()
    {
        if (!ns) return;

        ns.Update();

        if (NSPlayback.playbackState == NSPlayback.PlaybackState.Playing)
        {
            _time += Time.deltaTime * NSPlayback.speed;
            NSPlayback.Time.UpdateTime(NSPlayback.Time.iTimeProvider.GetTime());   
        }
    }

    private void UpdateRollingPlaybackUpdater()
    {
        switch (NSPlayback.playbackMode)
        {
            case NSPlayback.PlaybackMode.Rolling:
            {
                switch (NSPlayback.NSRollingPlayback.rollingMode)
                {
                    case NSPlayback.NSRollingPlayback.RollingMode.Left:
                    case NSPlayback.NSRollingPlayback.RollingMode.Right:
                    case NSPlayback.NSRollingPlayback.RollingMode.Top:
                    case NSPlayback.NSRollingPlayback.RollingMode.Bottom: rollingPlaybackUpdater.Update(); break;
                }
            }
            break;
            case NSPlayback.PlaybackMode.Ticker:
            {
                switch (NSPlayback.NSTickerPlayback.tickerMode)
                {
                    case NSPlayback.NSTickerPlayback.TickerMode.Screen: tickerPlaybackUpdater.Update(); break;
                    case NSPlayback.NSTickerPlayback.TickerMode.PageLayout: pageLayoutTickerPlaybackUpdater.Update(); break;
                    case NSPlayback.NSTickerPlayback.TickerMode.Undefined: break;
                }    
            } 
            break;
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D)) { this.nsDebug.gameObject.SetActive(!this.nsDebug.gameObject.activeSelf); }
        if (NSPlayback.updateTimeMode == NSPlayback.UpdateMode.Update) { UpdateTime(); }
        if (NSPlayback.updateCameraMode == NSPlayback.UpdateMode.Update) { UpdateRollingPlaybackUpdater(); }
    }

    private void FixedUpdate()
    {
        if (NSPlayback.updateTimeMode == NSPlayback.UpdateMode.FixedUpdate) { UpdateTime(); }
        if (NSPlayback.updateCameraMode == NSPlayback.UpdateMode.FixedUpdate) { UpdateRollingPlaybackUpdater(); }
    }

    private void LateUpdate()
    {
        if (NSPlayback.updateTimeMode == NSPlayback.UpdateMode.LateUpdate) { UpdateTime(); }
        if (NSPlayback.updateCameraMode == NSPlayback.UpdateMode.LateUpdate) { UpdateRollingPlaybackUpdater(); }
    }

    public float GetTime() => _time;
    public void SetTime(float value) => _time = value;
}
