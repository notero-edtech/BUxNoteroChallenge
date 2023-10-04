/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.Music.NotationSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class NSBackgroundZoom : MonoBehaviour, IScrollHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform fixedCameraRT;
    public RectTransform movableCameraRT;
    
    public float speed = 1f;

    private bool _scroll = true;
    
    public void OnScroll(PointerEventData eventData)
    {
        if(_scroll) NSPlayback.Zoom += eventData.scrollDelta.y * speed;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            NSPlayback.Zoom = 1f;
            switch (NSPlayback.NSRollingPlayback.rollingMode)
            {
                case NSPlayback.NSRollingPlayback.RollingMode.Left:
                case NSPlayback.NSRollingPlayback.RollingMode.Right:
                    fixedCameraRT.anchoredPosition = new Vector2(fixedCameraRT.anchoredPosition.x, 0);
                    movableCameraRT.anchoredPosition = new Vector2(movableCameraRT.anchoredPosition.x, 0);
                    break;
                case NSPlayback.NSRollingPlayback.RollingMode.Top:
                case NSPlayback.NSRollingPlayback.RollingMode.Bottom:
                    fixedCameraRT.anchoredPosition = new Vector2(0,fixedCameraRT.anchoredPosition.y);
                    movableCameraRT.anchoredPosition = new Vector2(0,movableCameraRT.anchoredPosition.y);
                    break;
                case NSPlayback.NSRollingPlayback.RollingMode.Undefined:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _scroll = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       DOVirtual.DelayedCall(0.5f, callback: () => _scroll = true);
    }
    
    // public void OnPointerClick(PointerEventData eventData)
    // {
    //     // if (eventData.button == PointerEventData.InputButton.Right)
    //     // {
    //     //     NSPlayback.Stop();
    //     //     fixedCamera.anchoredPosition = new Vector2(fixedCamera.anchoredPosition.x, 0);
    //     //     movableCamera.anchoredPosition = new Vector2(movableCamera.anchoredPosition.x, 0);
    //     // }
    // }

    //Touch touchZero;
    //Touch touchOne;

    //Vector2 touchZeroPrevPos;
    //Vector2 touchOnePrevPos;

    //float prevTouchDeltaMag;
    //float touchDeltaMag;
    //float deltaMagnitudeDiff;

    // void Update()
    // {
    //     if (Input.touchCount == 2)
    //     {
    //         _pinch = true;
    //         // Store both touches.
    //         //touchZero = Input.GetTouch(0);
    //         //touchOne = Input.GetTouch(1);
    //
    //         // Find the position in the previous frame of each touch.
    //         //touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
    //         //touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
    //
    //         // Find the magnitude of the vector (the distance) between the touches in each frame.
    //         //prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
    //         //touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
    //
    //         // Find the difference in the distances between each frame.
    //         //deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
    //
    //         //nsBehaviour.zoomSlider.value += deltaMagnitudeDiff * zoomSpeed;
    //         //nsBehaviour.zoomSlider.value = Mathf.Clamp (nsBehaviour.zoomSlider.value, nsBehaviour.zoomSlider.minValue, nsBehaviour.zoomSlider.maxValue); 
    //     }
    // }
}
