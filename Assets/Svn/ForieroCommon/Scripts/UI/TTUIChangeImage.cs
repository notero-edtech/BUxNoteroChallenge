using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TTUIChangeImage : MonoBehaviour
{
    public Sprite down;
    public Sprite up;
    public Sprite over;

    Image image;

    //bool isDown = false;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (over)
        {
            image.sprite = over;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (up)
        {
            image.sprite = up;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //isDown = true;

        if (down)
        {
            image.sprite = down;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //isDown = false;
        if (up)
        {
            image.sprite = up;
        }
    }
}
