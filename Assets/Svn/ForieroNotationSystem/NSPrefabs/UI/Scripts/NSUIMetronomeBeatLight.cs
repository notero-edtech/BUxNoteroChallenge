/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using DG.Tweening;
using ForieroEngine.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class NSUIMetronomeBeatLight : MonoBehaviour
{
    public Color color = Color.white;

    public SpriteRenderer lightRenderer;
    public Image lightImage;

    public SpriteRenderer backgroundRenderer;
    public Image backgroundImage;

    public Sprite[] backgroundVariants;

    private void Awake()
    {
        if (lightRenderer) lightRenderer.color = color.A(0);
        if (lightImage) lightImage.color = color.A(0);

        if (backgroundRenderer && backgroundVariants.Length > 0) backgroundRenderer.sprite = backgroundVariants[Random.Range(0, backgroundVariants.Length - 1)];
        if (backgroundImage && backgroundVariants.Length > 0) backgroundImage.sprite = backgroundVariants[Random.Range(0, backgroundVariants.Length - 1)];
    }

    private Tweener tweenerSpriteRenderer = null;
    private Tweener tweenerImage = null;

    private void OnDestroy()
    {
        tweenerSpriteRenderer?.Kill();
        tweenerImage?.Kill();
    }

    public void DoLight(float duration, Ease ease = Ease.OutSine)
    {
        if (lightRenderer)
        {
            lightRenderer.color = color;
            if (tweenerSpriteRenderer == null)
            {
                tweenerSpriteRenderer = lightRenderer.DOColor(color.A(0), duration).SetEase(ease);
                tweenerSpriteRenderer.SetAutoKill(false);
            }
            else
            {
                tweenerSpriteRenderer.Restart();
            }
        }

        if (lightImage)
        {
            lightImage.color = color;
            if (tweenerImage == null)
            {
                tweenerImage = lightImage.DOColor(color.A(0), duration).SetEase(ease);
                tweenerImage.SetAutoKill(false);
            }
            else
            {
                tweenerImage.Restart();
            }
        }
    }
}
