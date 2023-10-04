/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Extensions;
using ForieroEngine.Music.NotationSystem;
using UnityEngine;


public partial class NSBehaviour : MonoBehaviour
{
    //private float OverlayZoom() => NSPlayback.zoom * fixedCanvasRT.GetWidth() / overlayCanvasRT.anchoredPosition.x;
    
    public void OnZoomChanged(float zoom)
    {
        switch (NSSettingsStatic.canvasRenderMode)
        {
            case CanvasRenderMode.Screen:
                switch (NSSettingsStatic.textMode)
                {
                    case TextMode.Text:
                        {
                            movablePoolRT.localScale = Vector3.one;
                            fixedPoolRT.localScale = Vector3.one;
                            movableOverlayPoolRT.localScale = Vector3.one;
                            //overlayPoolRT.localScale = Vector3.one;
                            //overlayCanvasScaler.scaleFactor = OverlayZoom();

                            //Vector2 frr = const_fixedReferenceResolution * (1f / zoom);
                            Vector2 frr = const_fixedReferenceResolution * (1f / zoom).Round(1);

                            //if (Application.isMobilePlatform)
                            //{
                            //    if (System.Math.Abs(frr.x - fixedCanvasScaler.referenceResolution.x) > 0.01f)
                            //    {
                            //        fixedCanvasScaler.referenceResolution = frr;
                            //    }
                            //}
                            //else
                            {
                                fixedCanvasScaler.referenceResolution = frr;
                            }

                            //Vector2 mrr = const_movableReferenceResolution * (1f / zoom);
                            Vector2 mrr = const_movableReferenceResolution * (1f / zoom).Round(1);

                            //if (Application.isMobilePlatform)
                            //{
                            //    if (System.Math.Abs(mrr.x - movableCanvasScaler.referenceResolution.x) > 0.01f)
                            //    {
                            //        movableCanvasScaler.referenceResolution = mrr;
                            //    }
                            //}
                            //else
                            {
                                movableCanvasScaler.referenceResolution = mrr;
                            }
                            break;
                        }
                    case TextMode.TextMeshPRO:
                        {
                            movablePoolRT.localScale = Vector3.one * NSPlayback.Zoom;
                            fixedPoolRT.localScale = Vector3.one * NSPlayback.Zoom;
                            movableOverlayPoolRT.localScale = Vector3.one * NSPlayback.Zoom;
                            //overlayCanvasScaler.scaleFactor = OverlayZoom();
                            break;
                        }
                }
                break;
            case CanvasRenderMode.World:
                movablePoolRT.localScale = Vector3.one;
                fixedPoolRT.localScale = Vector3.one;
                movableOverlayPoolRT.localScale = Vector3.one;
                //overlayCanvasScaler.scaleFactor = OverlayZoom();

                movableCamera.orthographicSize = 5f / zoom;
                fixedCamera.orthographicSize = 5f / zoom;
                break;
        }
        ns.IsNotNull()?.ZoomChanged(NSPlayback.Zoom);
    }
}
