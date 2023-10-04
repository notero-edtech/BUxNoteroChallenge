using UnityEngine;
using System.Collections;

public class TTUIFollowRectTransform : MonoBehaviour {

	public RectTransform rectTransform;
			
	// Update is called once per frame
	void Update () {
		(transform  as RectTransform).position = rectTransform.position;	
	}
}
