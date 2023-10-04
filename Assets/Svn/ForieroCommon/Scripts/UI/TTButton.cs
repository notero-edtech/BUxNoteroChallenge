using UnityEngine;
using System.Collections;
using DG.Tweening;
using HedgehogTeam.EasyTouch;

public class TTButton : MonoBehaviour
{

	public GameObject target;
	public string method;

	public float animationTime = 0.2f;
	public float animationScaleFactor = 0.9f;

	public bool sound = true;

	Vector3 size;

	void OnEnable ()
	{
		SubscribeEvents ();
	}

	void OnDisable ()
	{
		UnsubscribeEvents ();
	}

	void OnDestroy ()
	{
		UnsubscribeEvents ();
	}

	void Start ()
	{
		size = transform.localScale;
	}

	void SubscribeEvents ()
	{

		EasyTouch.On_TouchDown += HandleOn_TouchDown;
		EasyTouch.On_TouchUp += HandleOn_TouchUp;
	}

	void HandleOn_TouchDown (Gesture gesture)
	{
		if (gesture.pickedObject == this.gameObject) {
			AnimateDown ();
		}
	}

	void HandleOn_TouchUp (Gesture gesture)
	{
		if (gesture.pickedObject == this.gameObject) {
			AnimateUp ();
			Debug.Log ("TTButton clicked : " + this.gameObject.name);
			if (sound)
				SM.PlayFX ("click");
			if (target) {
				target.SendMessage (method);
			}
		}
	}

	void UnsubscribeEvents ()
	{
		EasyTouch.On_TouchDown -= HandleOn_TouchDown;
		EasyTouch.On_TouchUp -= HandleOn_TouchUp;
	}

	Tweener tweener = null;

	void AnimateDown ()
	{
		if (tweener != null)
			tweener.Kill ();
		transform.DOScale (size * animationScaleFactor, animationTime)
			.SetEase (Ease.InOutSine);
		            
	}

	void AnimateUp ()
	{
		if (tweener != null)
			tweener.Kill ();
		transform.DOScale (size, animationTime)
			.SetEase (Ease.InOutSine);
	}
}

