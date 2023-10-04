using UnityEngine;
using System.Collections;

public class SceneFade : MonoBehaviour
{
    public enum FadeEnum
    {
        In,
        Out
    }

    public FadeEnum fadeEnum = FadeEnum.In;
    public Color transitionColor = Color.white;
    public float fadeTime = 0.3f;

    void Start()
    {
        switch (fadeEnum)
        {
            case FadeEnum.In:
                ForieroEngine.Scene.FadeIn(fadeTime, transitionColor, () =>
                {
                    Destroy(this);
                });
                break;
            case FadeEnum.Out:
                ForieroEngine.Scene.FadeOut(fadeTime, transitionColor, () =>
                {
                    Destroy(this);
                });
                break;
        }
    }
}
