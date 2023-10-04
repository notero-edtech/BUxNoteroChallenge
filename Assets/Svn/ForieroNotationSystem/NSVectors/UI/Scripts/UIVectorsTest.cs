/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVectorsTest : MonoBehaviour
{
	public Canvas canvas;
	public CanvasScaler canvasScaler;

	public Slider sliderX;
	public Slider sliderY;

	public RectTransform rt;

	public float pxSpeedX = 1f;
	public float pxSpeedY = 1f;

	float signX = 1f;
	float signY = 1f;

	// Use this for initialization
	void Start ()
	{
		rt = transform as RectTransform;
	}
	 	
	// Update is called once per frame
	void Update ()
	{
		rt.anchoredPosition = new Vector2 (rt.anchoredPosition.x + Time.deltaTime * pxSpeedX * signX, rt.anchoredPosition.y + Time.deltaTime * pxSpeedY * signY);

		if (rt.anchoredPosition.x > rt.rect.width / 2f) {
			signX = -1;
		}

		if (rt.anchoredPosition.x < -rt.rect.width / 2f) {
			signX = 1;
		}

		if (rt.anchoredPosition.y > rt.rect.height / 2f) {
			signY = -1;
		}

		if (rt.anchoredPosition.y < -rt.rect.height / 2f) {
			signY = 1;
		}
	}

	public void OnSpeedXChanged ()
	{
		pxSpeedX = sliderX.value;
	}

	public void OnSpeedYChanged ()
	{
		pxSpeedY = sliderY.value;
	}
}
