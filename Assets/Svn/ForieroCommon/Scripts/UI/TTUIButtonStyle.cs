using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class TTUIButtonStyle : ScriptableObject {
	public Sprite sprite;
	public Color color = Color.white;
	public Color colorOver = Color.blue;
	public Color colorDown = Color.grey;
	public float colorTime = 0.3f;
	public Ease colorEase = Ease.InOutSine;
}
