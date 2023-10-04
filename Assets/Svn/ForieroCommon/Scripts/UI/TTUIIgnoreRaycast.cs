using UnityEngine;
using UnityEngine.UI;

public class TTUIIgnoreRaycast : MonoBehaviour, ICanvasRaycastFilter
{
	public bool IsRaycastLocationValid (Vector2 screenPoint, Camera eventCamera)
	{
		return this.gameObject.activeSelf;
	}
}