/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TLUnityMetronomeImage : MonoBehaviour
{
    public Image image;

    public Color color = Color.green;
    public Color fadeColor = Color.white;

    Tweener tweener = null;

    void Awake()
    {
        TLUnityMetronome.instance.OnBeat += Invoke;
    }

    private void OnDestroy()
    {
        TLUnityMetronome.instance.OnBeat -= Invoke;

        if (tweener != null)
        {
            tweener.Kill();
        }
    }

    void Invoke(int beat, double beatDuration)
    {
        if (image && beat == 1)
        {
            image.color = color;
            if (tweener == null)
            {
                tweener = image.DOColor(new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0), TLUnityMetronome.instance.beats * (float)beatDuration).SetEase(Ease.Linear);
                tweener.SetAutoKill(false);
            }
            else
            {
                tweener.Restart();
            }
        }
    }
}
