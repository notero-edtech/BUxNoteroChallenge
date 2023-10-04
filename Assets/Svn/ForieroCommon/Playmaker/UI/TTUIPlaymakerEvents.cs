using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.EventSystems;

public class TTUIPlaymakerEvents : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    public PlayMakerFSM fsm;
    public void OnPointerClick(PointerEventData eventData) => fsm?.Fsm.Event(gameObject, FsmEvent.UiPointerClick);
    public void OnPointerDown(PointerEventData eventData) => fsm?.Fsm.Event(gameObject, FsmEvent.UiPointerDown);
    public void OnPointerEnter(PointerEventData eventData) => fsm?.Fsm.Event(gameObject, FsmEvent.UiPointerEnter);
    public void OnPointerExit(PointerEventData eventData) => fsm?.Fsm.Event(gameObject, FsmEvent.UiPointerExit);
    public void OnPointerUp(PointerEventData eventData) => fsm?.Fsm.Event(gameObject, FsmEvent.UiPointerUp);

    //public void OnPointerClick(PointerEventData eventData) => fsm?.SendEvent(FsmEvent.UiPointerClick.ToString());
    //public void OnPointerDown(PointerEventData eventData) => fsm?.SendEvent(FsmEvent.UiPointerDown.ToString());
    //public void OnPointerEnter(PointerEventData eventData) => fsm?.SendEvent(FsmEvent.UiPointerEnter.ToString());
    //public void OnPointerExit(PointerEventData eventData) => fsm?.SendEvent(FsmEvent.UiPointerExit.ToString());
    //public void OnPointerUp(PointerEventData eventData) => fsm?.SendEvent(FsmEvent.UiPointerUp.ToString());
}
