/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.Music.NotationSystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class NSBackgroundDrag : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform fixedCameraRT;
    public RectTransform movableCameraRT;
    
    public Canvas backgroundCanvas;

    public float zoomSpeed = 1f;

    private Vector2 _touchPosition = Vector2.zero;
    private bool _pinch = false;

    public Action<PointerEventData> OnBeginDragEvent;
    public Action<PointerEventData> OnDragEvent;
    public Action<PointerEventData> OnEndDragEvent;

    private Vector2 TouchPosition()
    {
        if (Input.GetMouseButton(0)) { return Input.mousePosition; }
        else if (Input.touchCount > 0) { return Input.touches[0].position; }
        else { return Vector2.zero; }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        NSPlayback.dragging = true;

        OnBeginDragEvent?.Invoke(eventData);

        _touchPosition = TouchPosition();
        NSPlayback.playbackState = NSPlayback.PlaybackState.Pausing;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_pinch && (Input.touchCount == 1 || Input.GetMouseButton(0)))
        {
            var diff = (_touchPosition - TouchPosition()) / backgroundCanvas.scaleFactor;

            if (!NSSettingsStatic.movableCameraHDragMove) diff.x = 0;
            if (!NSSettingsStatic.movableCameraVDragMove) diff.y = 0;

            movableCameraRT.anchoredPosition += diff * 1f / (NSSettingsStatic.canvasRenderMode == CanvasRenderMode.Screen ? 1 : NSPlayback.Zoom);

            diff = (_touchPosition - TouchPosition()) / backgroundCanvas.scaleFactor;

            if (!NSSettingsStatic.fixedCameraHDragMove) diff.x = 0;
            if (!NSSettingsStatic.fixedCameraVDragMove) diff.y = 0;
            
            fixedCameraRT.anchoredPosition += diff * 1f / (NSSettingsStatic.canvasRenderMode == CanvasRenderMode.Screen ? 1 : NSPlayback.Zoom);

            _touchPosition = TouchPosition();
        }
        else
        {
            _pinch = true;
            _touchPosition = TouchPosition();
        }

        OnDragEvent?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDragEvent?.Invoke(eventData);

        _touchPosition = Vector2.zero;
        _pinch = false;

        NSPlayback.dragging = false;
    }

    public void OnPointerUp(PointerEventData eventData) { }
    public void OnPointerExit(PointerEventData eventData) { }
    public void OnPointerEnter(PointerEventData eventData) { }
    public void OnPointerDown(PointerEventData eventData) { }
    public void OnPointerClick(PointerEventData eventData) { }
    
}
