using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

using DG.Tweening;

public class TTUIAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    public enum Anim
    {
        None,
        ShakePosition,
        ShakeRotation,
        ShakeScale
    }

    public float time = 0.2f;

    public TTUIAnimStyle style;

    Selectable selectable = null;
    RectTransform rt = null;

    TTUIAnimStyle _style;

    void Awake()
    {
        _style = new TTUIAnimStyle();

        if (style)
        {
            _style = style;
        }
        else if (UISettings.instance.defaultAnimStyle)
        {
            _style = UISettings.instance.defaultAnimStyle;
        }

        foreach (Component c in GetComponents<Component>())
        {
            if (c is Selectable)
            {
                selectable = c as Selectable;
            }
        }

        rt = transform as RectTransform;
    }

    Tweener tweener = null;

    void DoEffect(Anim anim)
    {
        if (tweener != null && tweener.IsPlaying())
        {
            return;
        }

        switch (anim)
        {
            case Anim.ShakePosition:
                tweener = rt.DOShakePosition(time);
                return;
            case Anim.ShakeRotation:
                tweener = rt.DOShakeRotation(time);
                return;
            case Anim.ShakeScale:
                tweener = rt.DOShakeScale(time);
                return;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectable != null)
        {
            if (selectable.interactable)
            {
                DoEffect(_style.onPointerEnter);
            }
        }
        else
        {
            DoEffect(_style.onPointerEnter);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectable != null)
        {
            if (selectable.interactable)
            {
                DoEffect(_style.onPointerExit);
            }
        }
        else
        {
            DoEffect(_style.onPointerExit);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (selectable != null)
        {
            if (selectable.interactable)
            {
                DoEffect(_style.onPointerDown);
            }
        }
        else
        {
            DoEffect(_style.onPointerDown);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (selectable != null)
        {
            if (selectable.interactable)
            {
                DoEffect(_style.onPointerUp);
            }
        }
        else
        {
            DoEffect(_style.onPointerUp);
        }
    }
}
