using ForieroEngine.Purchasing;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneLoaderUIClick : MonoBehaviour, IPointerUpHandler
{
    public enum SceneConfig
    {
        Settings,
        Selected,
        Custom
    }

    public SceneSettings.LoadEnum loadEnum = SceneSettings.LoadEnum.Command;
    public string sceneValue = "";

    public SceneSettings.LoadEnum inAppLoadEnum = SceneSettings.LoadEnum.Command;
    public string inAppSceneValue = "UNLOCKED";
    public string inApp = "UNLOCK";

    #region IPointerUpHandler implementation

    public void OnPointerUp(PointerEventData eventData)
    {
#if FORIERO_INAPP
        if (Store.Purchased(inApp))
        {
            SceneSettings.LoadScene(inAppLoadEnum, inAppSceneValue);
        }
        else
#endif
        {
            SceneSettings.LoadScene(loadEnum, sceneValue);
        }
    }

    #endregion
}
