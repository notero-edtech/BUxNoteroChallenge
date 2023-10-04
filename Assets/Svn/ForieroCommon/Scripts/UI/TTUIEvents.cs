using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TTUIEvents : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Button.ButtonClickedEvent OnClick;
    public Button.ButtonClickedEvent OnDown;
    public Button.ButtonClickedEvent OnUp;
    public Button.ButtonClickedEvent OnEnter;
    public Button.ButtonClickedEvent OnExit;
    
    public void OnPointerClick(PointerEventData eventData) => OnClick.Invoke();
    public void OnPointerEnter(PointerEventData eventData) => OnEnter.Invoke();
    public void OnPointerExit(PointerEventData eventData) => OnExit.Invoke();
    public void OnPointerDown(PointerEventData eventData) => OnDown.Invoke();
    public void OnPointerUp(PointerEventData eventData) => OnUp.Invoke();
}
