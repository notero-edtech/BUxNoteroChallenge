using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ForieroEngine.Extensions;
using UnityEngine.InputSystem;

public class MouseFollow2D : MonoBehaviour
{
    public Camera mouseCamera;

    public bool lerpFollow = true;
    public float lerpSpeed = 20f;
    public bool maxPixelSpeed = false;
    public float maxPixelSpeedPixels = 50f;
    public float z = 0;

    private Vector3 mousePosition;
    private Vector3 mouseWorldPosition;
    private Vector2 anchoredPosition;
    private Vector2 prevAnchoredPosition;
    private Canvas canvas;
    private RectTransform rectTransform;

    void Start()
    {
        canvas = transform.GetComponentInParent<Canvas>();
        if (transform is RectTransform) rectTransform = transform as RectTransform;

        Update(false);
    }

    public void SetCurrentPosition()
    {
        Update(false);
    }

    void Update(){
        Update(lerpFollow);
    }

    void Update(bool lerp)
    {
        if (!mouseCamera) mouseCamera = Camera.main;
        if (!mouseCamera && Camera.allCameras.Length > 0) mouseCamera = Camera.allCameras[0];
        if (!mouseCamera) return;

        #if ENABLE_INPUT_SYSTEM
        mousePosition = Mouse.current.position.ReadValue();        
        #else 
        mousePosition = Input.mousePosition;
        #endif
        mouseWorldPosition = mouseCamera.ScreenToWorldPoint(mousePosition).SetZ(z);

        if(rectTransform && canvas){
            switch(canvas.renderMode){
                case RenderMode.ScreenSpaceOverlay:
                    if (mouseCamera)
                    {
                        anchoredPosition = canvas.WorldToCanvas(mouseWorldPosition, mouseCamera);
                    } else {

                    }
                    break;
                case RenderMode.ScreenSpaceCamera:
                    if(canvas.worldCamera){
                        anchoredPosition = canvas.WorldToCanvas(mouseWorldPosition, canvas.worldCamera);
                    } else {

                    }
                    break;
                case RenderMode.WorldSpace:
                    if(canvas.worldCamera){
                        anchoredPosition = canvas.WorldToCanvas(mouseWorldPosition, canvas.worldCamera);
                    } else {

                    }
                    break;
            }

            if (lerp)
            {
                prevAnchoredPosition = rectTransform.anchoredPosition;
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, anchoredPosition, lerpSpeed * Time.deltaTime);
                if (maxPixelSpeed)
                {
                    var distance = Mathf.Abs(Vector2.Distance(prevAnchoredPosition, rectTransform.anchoredPosition) / Time.deltaTime);
                    if (distance > maxPixelSpeedPixels)
                    {
                        var sub = rectTransform.anchoredPosition - prevAnchoredPosition;
                        sub.Normalize();
                        rectTransform.anchoredPosition = maxPixelSpeedPixels * sub * Time.deltaTime + prevAnchoredPosition;
                    }
                }
            }
            else rectTransform.anchoredPosition = anchoredPosition;
        } else {
            if (lerp) transform.position = Vector2.Lerp(transform.position, mousePosition, lerpSpeed * Time.deltaTime);
            else transform.position = mousePosition.SetZ(z);
        }
    }
 
}