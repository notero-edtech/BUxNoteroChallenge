using UnityEngine;
using UnityEngine.EventSystems;

public class TTUIStoreEnabled : MonoBehaviour
{
    public bool steam = true;

    void Start()
    {
#if PUBLISHING_PLATFORM_STEAM
        this.gameObject.SetActive(steam);
#endif
    }
}
