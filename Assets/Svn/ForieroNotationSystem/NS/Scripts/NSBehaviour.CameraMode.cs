/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem;
using UnityEngine;


public partial class NSBehaviour : MonoBehaviour
{
    float fixedCameraOrthographicSize = 5f;

    public void SwitchCameraMode(CanvasRenderMode canvasRenderMode, bool force = false)
    {
        if (NSSettingsStatic.canvasRenderMode != canvasRenderMode || force)
        {
            switch (canvasRenderMode)
            {
                case CanvasRenderMode.Screen:
                    {
                        var zoom = NSPlayback.Zoom;

                        movableCamera.orthographicSize = 5f;
                        fixedCamera.orthographicSize = 5f;

                        fixedCameraOrthographicSize = 5f;

                        fixedCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                        movableCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                        NSSettingsStatic.canvasRenderMode = CanvasRenderMode.Screen;

                        NSPlayback.Zoom = zoom;
                        break;
                    }
                case CanvasRenderMode.World:
                    {
                        var zoom = 1f / (1f / NSPlayback.Zoom).Round(1);

                        fixedCanvas.renderMode = RenderMode.WorldSpace;
                        movableCanvas.renderMode = RenderMode.WorldSpace;
                        NSSettingsStatic.canvasRenderMode = CanvasRenderMode.World;

                        fixedCanvas.transform.localScale = const_fixedCanvasScale;
                        movableCanvas.transform.localScale = const_movableCanvasScale;

                        fixedCameraOrthographicSize = 5f / zoom;

                        NSPlayback.Zoom = zoom;
                        break;
                    }
            }
        }
    }

    public float GetWorldOrthographicSizeRatio()
    {
        return fixedCamera.orthographicSize / fixedCameraOrthographicSize;
    }
}
