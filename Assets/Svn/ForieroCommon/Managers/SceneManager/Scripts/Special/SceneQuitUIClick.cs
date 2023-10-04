using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;


public class SceneQuitUIClick : MonoBehaviour, IPointerUpHandler
{

	#region IPointerUpHandler implementation

	GameObject go;

	public void OnPointerUp (PointerEventData eventData)
	{
		if (!go) {
			go = Resources.Load<GameObject> ("TVOFF");
			Instantiate (go);
			SM.StopAllMusic (0.2f);
		}
	}

	#endregion
}
