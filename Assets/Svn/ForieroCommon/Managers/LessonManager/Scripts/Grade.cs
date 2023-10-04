using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Grade : MonoBehaviour
{
	public Image backgroundImage;
	public Text gradeText;

	[HideInInspector]
	public Sprite normalSprite;
	public Sprite selectedSprite;

	public RectTransform rectTransform;

	[HideInInspector]
	public LessonManager lessonManager;
	[HideInInspector]
	public LessonManager.GradeItem gradeItem;

	void Awake ()
	{
		normalSprite = backgroundImage.sprite;
		rectTransform = transform as RectTransform;
	}

	public void OnGradeClick ()
	{
		lessonManager.OnGradeClick (this);
	}
}
