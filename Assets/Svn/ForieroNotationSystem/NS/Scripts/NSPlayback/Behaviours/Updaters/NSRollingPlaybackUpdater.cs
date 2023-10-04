/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.Music.NotationSystem;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public partial class NSRollingPlaybackUpdater : NSUpdaterBase
{
    private RectTransform hitZoneRT;
    
    public NSRollingPlaybackUpdater(NSBehaviour nsBehaviour) : base (nsBehaviour) { }

    public override void SubscribeEvents()
    {
        base.SubscribeEvents();
        UpdateCameraPositionByTime();
    }
    
    public override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
    }
    
    protected override void OnZoomChanged(float obj)
    {
        if (blocked) return;
        UpdateCameraPositionByTime(0);
    }

    protected override void NSBackgroundDrag_OnBeginDragEvent(PointerEventData obj)
    {
        if (blocked) return;
        UpdateTimeByPixelPosition();
    }

    protected override void NSBackgroundDrag_OnDragEvent(PointerEventData obj)
    {
        if (blocked) return;
        UpdateTimeByPixelPosition();
    }

    protected override void NSBackgroundDrag_OnEndDragEvent(PointerEventData obj)
    {
        if (blocked) return;
        UpdateTimeByPixelPosition();
    }

    protected override void OnPlaybackStateChanged(NSPlayback.PlaybackState state)
    {
        base.OnPlaybackStateChanged(state);
        
        if (NSPlayback.playbackState == NSPlayback.PlaybackState.Stop)
        {
            NSPlayback.NSRollingPlayback.pixelPosition = 0f;
            UpdateCameraPositionByTime();
            ResetPassables();            
        }
    }

    protected override void OnPlaybackModeChanged(NSPlayback.PlaybackMode playbackMode)
    {
        if (blocked) return;
    }

    protected override void OnRollingModeChanged(NSPlayback.NSRollingPlayback.RollingMode rollingMode)
    {
        if (blocked) return;
    }

    public void Update()
    {
        if (blocked) return;
        if (!nsBehaviour) return;
        if (!nsBehaviour.ns) return;
        if (!nsBehaviour.hitZoneRT) return;
        
        hitZoneRT = nsBehaviour.hitZoneRT;
        
        UpdateCameraPositionByTime();       
        UpdatePassables();
        UpdateStaveMidiStates();
    }

    private float _pixelPosition = 0f;
    private Vector2 _anchoredPosition;
    private Vector3 _position;
    private void UpdateCameraPositionByTime(float? speed = null)
    {
        speed ??= NSSettingsStatic.movableCameraLerpSpeed;
        if (!hitZoneRT) return;
        if (NSPlayback.playbackMode != NSPlayback.PlaybackMode.Rolling) return;
        
        _pixelPosition = (float)NSPlayback.NSRollingPlayback.pixelPosition;

        if (NSPlayback.Time.time.Approximately(0)) { _pixelPosition = -1; }
        else if (NSPlayback.Time.time.Approximately(NSPlayback.Time.TotalTime)) { _pixelPosition = (float)(NSPlayback.NSRollingPlayback.pixelsPerSecond * NSPlayback.Time.TotalTime + 1); }

        switch (NSPlayback.NSRollingPlayback.rollingMode)
        {
            case NSPlayback.NSRollingPlayback.RollingMode.Undefined: break;
            case NSPlayback.NSRollingPlayback.RollingMode.Left:
            {
                _anchoredPosition = movableCameraRT.anchoredPosition.SetX(_pixelPosition);
                movableCameraRT.anchoredPosition = speed == 0 ? _anchoredPosition : movableCameraRT.anchoredPosition.Lerp(_anchoredPosition, (float)speed);
                _position = movableCameraRT.position.SetX(movableCameraRT.position.x + fixedCamera.transform.position.x + Mathf.Abs(hitZoneRT.position.x));
                movableCameraRT.position = speed == 0 ? _position : movableCameraRT.position.Lerp(_position, (float)speed);
                break;
            }
            case NSPlayback.NSRollingPlayback.RollingMode.Right:
            {
                _anchoredPosition = movableCameraRT.anchoredPosition.SetX(_pixelPosition * -1f);
                movableCameraRT.anchoredPosition = speed == 0 ? _anchoredPosition : movableCameraRT.anchoredPosition.Lerp(_anchoredPosition, (float)speed);
                _position = movableCameraRT.position.SetX(movableCameraRT.position.x + fixedCamera.transform.position.x - Mathf.Abs(hitZoneRT.position.x));
                movableCameraRT.position = speed == 0 ? _position : movableCameraRT.position.Lerp(_position, (float)speed);                        
                break;
            }
            case NSPlayback.NSRollingPlayback.RollingMode.Top:
            {
                _anchoredPosition = movableCameraRT.anchoredPosition.SetY(_pixelPosition * -1f);
                movableCameraRT.anchoredPosition = speed == 0 ? _anchoredPosition : movableCameraRT.anchoredPosition.Lerp(_anchoredPosition, (float)speed);
                _position = movableCameraRT.position.SetY(movableCameraRT.position.y + fixedCamera.transform.position.y - Mathf.Abs(hitZoneRT.position.y));
                movableCameraRT.position = speed == 0 ? _position : movableCameraRT.position.Lerp(_position, (float)speed);                                            
                break;
            }
            case NSPlayback.NSRollingPlayback.RollingMode.Bottom:
            {
                _anchoredPosition = movableCameraRT.anchoredPosition.SetY(_pixelPosition);
                movableCameraRT.anchoredPosition = speed == 0 ? _anchoredPosition : movableCameraRT.anchoredPosition.Lerp(_anchoredPosition, (float)speed);
                _position = movableCameraRT.position.SetY(movableCameraRT.position.y + fixedCamera.transform.position.y + Mathf.Abs(hitZoneRT.position.y));
                movableCameraRT.position = speed == 0 ? _position : movableCameraRT.position.Lerp(_position, (float)speed);
                break;
            }
        }
    }

    private Vector2 _original;
    private float _x, _xStart, _y, _yStart; 
    private void UpdateTimeByPixelPosition()
    {
        if (hitZoneRT == null) return;
        
        _original = movableCameraRT.anchoredPosition;
         _x = _xStart = _y = _yStart = 0f;

        switch (NSPlayback.NSRollingPlayback.rollingMode)
        {
            case NSPlayback.NSRollingPlayback.RollingMode.Undefined: break;
            case NSPlayback.NSRollingPlayback.RollingMode.Left:
                {
                    _x = movableCameraRT.anchoredPosition.x;
                    movableCameraRT.position = new Vector3(fixedCamera.transform.position.x + Mathf.Abs(hitZoneRT.position.x), movableCameraRT.position.y, movableCameraRT.position.z);
                    _xStart = movableCameraRT.anchoredPosition.x;
                    movableCameraRT.anchoredPosition = _original;
                    NSPlayback.NSRollingPlayback.pixelPosition = _x - _xStart;
                    UpdateCameraPositionByTime(0);
                    break;
                }
            case NSPlayback.NSRollingPlayback.RollingMode.Right:
                {
                    _x = movableCameraRT.anchoredPosition.x;
                    movableCameraRT.position = new Vector3(fixedCamera.transform.position.x - Mathf.Abs(hitZoneRT.position.x), movableCameraRT.position.y, movableCameraRT.position.z);
                    _xStart = movableCameraRT.anchoredPosition.x;
                    movableCameraRT.anchoredPosition = _original;
                    NSPlayback.NSRollingPlayback.pixelPosition = _xStart - _x;
                    UpdateCameraPositionByTime(0);
                    break;
                }
            case NSPlayback.NSRollingPlayback.RollingMode.Top:
                {
                    _y = movableCameraRT.anchoredPosition.y;
                    movableCameraRT.position = new Vector3(movableCameraRT.position.x, fixedCamera.transform.position.y - Mathf.Abs(hitZoneRT.position.y), movableCameraRT.position.z);
                    _yStart = movableCameraRT.anchoredPosition.y;
                    movableCameraRT.anchoredPosition = _original;
                    NSPlayback.NSRollingPlayback.pixelPosition = _yStart - _y;
                    UpdateCameraPositionByTime(0);
                    break;
                }
            case NSPlayback.NSRollingPlayback.RollingMode.Bottom:
                {
                    _y = movableCameraRT.anchoredPosition.y;
                    movableCameraRT.position = new Vector3(movableCameraRT.position.x, fixedCamera.transform.position.y + Mathf.Abs(hitZoneRT.position.y), movableCameraRT.position.z);
                    _yStart = movableCameraRT.anchoredPosition.y;
                    movableCameraRT.anchoredPosition = _original;
                    NSPlayback.NSRollingPlayback.pixelPosition = _y - _yStart;
                    UpdateCameraPositionByTime(0);
                }
                break;
        }
    }
}
