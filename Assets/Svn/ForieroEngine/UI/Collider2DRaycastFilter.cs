using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent (typeof (RectTransform), typeof (Collider2D))]
public class Collider2DRaycastFilter : MonoBehaviour, ICanvasRaycastFilter, IPointerClickHandler
{
    private Collider2D _c;
    private RectTransform _rt;
   
    private void Awake ()
    {
        _c = GetComponent<Collider2D>();
        _rt = GetComponent<RectTransform>();
    }
    
    public bool IsRaycastLocationValid (Vector2 screenPos, Camera eventCamera)
    {
        var isInside = RectTransformUtility.ScreenPointToWorldPointInRectangle(
            _rt,
            screenPos,
            eventCamera,
            out var worldPoint
        );
        if (isInside) isInside = _c.OverlapPoint(worldPoint);
 
        return isInside;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CLICK");
    }
}