/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TLUnityMetronomeSprite : MonoBehaviour
{
    public SpriteRenderer sr;

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
        tweener?.Kill();
    }

    void Invoke(int beat, double beatDuration)
    {
        if (sr && beat == 1)
        {
            sr.color = color;
            if (tweener == null)
            {
                tweener = sr.DOColor(new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0), Mathf.Clamp(TLUnityMetronome.instance.beats * (float)beatDuration, 0, 0.5f)).SetEase(Ease.Linear).SetDelay(0.1f);
                tweener.SetAutoKill(false);
            }
            else
            {
                tweener.Restart();
            }
        }
    }
}
