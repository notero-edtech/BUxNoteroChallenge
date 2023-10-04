using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class TTUISound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    public TTUISoundStyle style;

    [NonSerialized]
    public string onMouseEnter = "";
    [NonSerialized]
    public string onMouseExit = "";
    [NonSerialized]
    public string onMouseDown = "";
    [NonSerialized]
    public string onMouseUp = "";

    Selectable selectable = null;

    void Awake()
    {
        if (style)
        {
            onMouseEnter = style.onMouseEnter;
            onMouseExit = style.onMouseExit;
            onMouseDown = style.onMouseDown;
            onMouseUp = style.onMouseUp;
        }
        else if (UISettings.instance.defaultSoundStyle)
        {
            onMouseEnter = UISettings.instance.defaultSoundStyle.onMouseEnter;
            onMouseExit = UISettings.instance.defaultSoundStyle.onMouseExit;
            onMouseDown = UISettings.instance.defaultSoundStyle.onMouseDown;
            onMouseUp = UISettings.instance.defaultSoundStyle.onMouseUp;
        }

        foreach (Component c in GetComponents<Component>())
        {
            if (c is Selectable)
            {
                selectable = c as Selectable;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectable != null)
        {
            if (selectable.interactable)
            {
                SM.PlayFX(onMouseEnter);
            }
        }
        else
        {
            SM.PlayFX(onMouseEnter);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectable != null)
        {
            if (selectable.interactable)
            {
                SM.PlayFX(onMouseExit);
            }
        }
        else
        {
            SM.PlayFX(onMouseExit);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (selectable != null)
        {
            if (selectable.interactable)
            {
                SM.PlayFX(onMouseDown);
            }
        }
        else
        {
            SM.PlayFX(onMouseDown);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (selectable != null)
        {
            if (selectable.interactable)
            {
                SM.PlayFX(onMouseUp);
            }
        }
        else
        {
            SM.PlayFX(onMouseUp);
        }
    }
}
