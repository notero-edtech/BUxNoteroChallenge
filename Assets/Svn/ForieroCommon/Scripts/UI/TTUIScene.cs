using UnityEngine;
using UnityEngine.EventSystems;

public class TTUIScene : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public SceneSettings.LoadEnum loadEnum = SceneSettings.LoadEnum.Command;
    public string sceneValue = "";

    public void Load()
    {
        SceneSettings.LoadScene(loadEnum, sceneValue);
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Load();
    }
}
