using UnityEngine;
using UnityEngine.EventSystems;

public class CreateEventSystem : MonoBehaviour
{
    void Start()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
    }
}
