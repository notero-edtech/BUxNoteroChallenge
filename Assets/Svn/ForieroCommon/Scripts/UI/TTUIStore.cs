using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using ForieroEngine.Purchasing;

public class TTUIStore : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public StoreSettings storeSettings;
    public StoreAction storeAction = StoreAction.OpenProjectWebsite;

    public enum StoreAction
    {
        OpenStore,
        OpenProjectWebsite,
        OpenAppWebsite
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (storeSettings)
        {
            ParentalLock.Show(4, passed =>
            {
                if (passed)
                {
                    switch (storeAction)
                    {
                        case StoreAction.OpenProjectWebsite:
                            if (ForieroDebug.UI) Debug.Log("OpenProjectWebsite : " + storeSettings.projectWWW);
                            Application.OpenURL(storeSettings.projectWWW);
                            break;
                        case StoreAction.OpenAppWebsite:
                            if (ForieroDebug.UI) Debug.Log("OpenAppWebsite : " + storeSettings.appWWW);
                            Application.OpenURL(storeSettings.appWWW);
                            break;
                        case StoreAction.OpenStore:
                            if (ForieroDebug.UI) Debug.Log("OpenStore : " + "");
                            Store.OpenStore();
                            break;
                    }
                }
            });

        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
