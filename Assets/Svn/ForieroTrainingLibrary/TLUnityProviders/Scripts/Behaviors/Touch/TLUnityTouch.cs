/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.EventSystems;
using ForieroEngine.Music.Training;

public class TLUnityTouch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (TL.Inputs.OnTapDown != null)
        {
            TL.Inputs.OnTapDown();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (TL.Inputs.OnTapUp != null)
        {
            TL.Inputs.OnTapUp();
        }
    }
}

