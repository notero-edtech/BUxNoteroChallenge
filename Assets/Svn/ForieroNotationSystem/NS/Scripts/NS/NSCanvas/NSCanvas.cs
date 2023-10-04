/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class NSCanvas
{
    public const float DPIFallback = 96f;
    public static float DPI => Mathf.Approximately(Screen.dpi, 0f) ? DPIFallback : Screen.dpi;
    //public static float dpiScaleFactor { get { return dpi / dpiFallback; } }
    public const float DPIScaleFactor = 1f;
    public const float OrthographicsSize = 5f;
    public static float AspectRatio => Screen.width / Screen.height;
    public static float PixelsPerUnit => (Screen.height / 2f / DPIScaleFactor) / OrthographicsSize;
    public static float UnitsPerPixel => OrthographicsSize / (Screen.height / 2f / DPIScaleFactor);
    public static float Pixels2UnitsX(float pixels, bool pixelSnap = false) => pixels * UnitsPerPixel * AspectRatio;
    public static float Pixels2UnitsY(float pixels, bool pixelSnap = false) => pixels * UnitsPerPixel;
    public static float UnitsX2Pixels(float units, bool pixelSnap = true) => units * PixelsPerUnit * AspectRatio;
    public static float UnitsY2Pixels(float units, bool pixelSnap = false) => units * PixelsPerUnit;
    public static void SetPixelPosition(this RectTransform rt, Vector2 pixelPosition) => rt.anchoredPosition = new(Pixels2UnitsX(pixelPosition.x), Pixels2UnitsY(pixelPosition.y));
    public static Vector2 GetPixelPosition(this RectTransform rt) => new(UnitsX2Pixels(rt.anchoredPosition.x), UnitsY2Pixels(rt.anchoredPosition.y));
    public static float GetLineHeightInUnits(this TMP_FontAsset font, float fontSize) => (fontSize / font.faceInfo.pointSize * font.faceInfo.scale * 0.1f) * (font.faceInfo.pointSize / 4f);
    public static float GetLineHeightInPixels(this TMP_FontAsset font, float fontSize) => GetLineHeightInUnits(font, fontSize) * PixelsPerUnit;
}
