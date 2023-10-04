using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem.Extensions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


[Singleton] public class NSTestSuitePNGViewer : MonoBehaviour, ISingleton
{
    public RawImage xmlPng;
    public Slider pngSlider;
    
    public static NSTestSuitePNGViewer Instance => Singleton<NSTestSuitePNGViewer>.instance;
    
    public void OnPngSlider()
    {
        if (!xmlPng.texture) return;
        var scale = 1f / pngSlider.value;
        xmlPng.rectTransform.SetSize(new Vector2(xmlPng.texture.width * scale, xmlPng.texture.height * scale));
    }

    public void SetTexture(Texture texture)
    {
        xmlPng.texture = texture;
        OnPngSlider();
    }
}
