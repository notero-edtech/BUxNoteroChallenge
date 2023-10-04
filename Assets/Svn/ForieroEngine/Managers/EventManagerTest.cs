using UnityEngine;
using ForieroEngine;

public class EventManagerTest : MonoBehaviour
{
	void Start()
	{
        EventManager.AddListener<EventManager.Test>(B);
        EventManager.TriggerEvent(new EventManager.Test());
	}

	void B(EventManager.Test t)
	{
		Debug.Log("WOW : " + t.i);
	}

	void OnDestroy()
	{
        EventManager.RemoveListener<ForieroEngine.EventManager.Test>(B);
	}
}
