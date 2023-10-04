using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class TTUIButtonScale : ScriptableObject {
	public bool doScale = true;
	public Ease scaleEase = Ease.InOutSine;
	public float scaleFactor = 0.9f;
	public float scaleTime = 0.2f;
	public bool doShake = false;
}
