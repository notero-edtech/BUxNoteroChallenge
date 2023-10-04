using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TTUISetCanvasCamera : MonoBehaviour {

	public Canvas canvas;

	void Awake(){
		if(!canvas) canvas = GetComponent<Canvas>();
		canvas.worldCamera = Camera.main;
	}
}
