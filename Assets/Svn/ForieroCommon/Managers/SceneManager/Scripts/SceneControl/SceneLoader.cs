using ForieroEngine.Extensions;
using UnityEngine;
using ForieroEngine.Purchasing;

public class SceneLoader : MonoBehaviour
{
    public enum SceneConfig
    {
        Settings,
        Selected,
        Custom
    }

    public SceneSettings.LoadEnum loadEnum = SceneSettings.LoadEnum.Command;
    public string sceneValue = "";
    public float duration = 5f;
    public bool waitForUserAction = false;

    public SceneSettings.LoadEnum inAppLoadEnum = SceneSettings.LoadEnum.Command;
    public string inAppSceneValue = "UNLOCKED";
    public string inApp = "UNLOCK";

    void Start()
    {
        this.FireAction(duration, () =>
        {
            if (!waitForUserAction)
            {
                Load();
            }
        });
    }

    public void OnLoadSceneClick()
    {
        Load();
    }

    void Load()
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
}
