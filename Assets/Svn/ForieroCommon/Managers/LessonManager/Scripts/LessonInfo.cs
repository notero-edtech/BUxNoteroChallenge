using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using DG.Tweening;

using ForieroEngine.Extensions;

public class LessonInfo : MonoBehaviour
{
	public Image backgroundImage;
	public Image lessonIconImage;
	public Text gradeLessonText;
	public Text lessonNameText;
	public Text lessonDescriptionText;

	public float time = 0.5f;

	public Ease scaleInEase;
	public Ease scaleOutEase;

	public RectTransform infoRect;

	public RectTransform contentRect;

	public void Show (Sprite lessonSprite, string gradeLesson, string lessonName, string lessonDescription)
	{
		SM.PlayFX ("lesson_info_show");

		backgroundImage.sprite = lessonSprite;
		gradeLessonText.text = gradeLesson;
		lessonNameText.text = lessonName;

		this.gameObject.SetActive (true);

		infoRect.localScale = Vector3.zero;

		infoRect.DOScale (Vector3.one, time).SetEase (scaleInEase);

		backgroundImage.color = Color.black.Alfa (0f);

		backgroundImage.DOColor (Color.black.Alfa (1f), time);

		lessonDescriptionText.text = lessonDescription;

		contentRect.SetSize (new Vector2 (contentRect.GetSize ().x, lessonDescriptionText.preferredHeight));

		contentRect.anchoredPosition = Vector2.zero;
	}

	public void Hide ()
	{
		SM.PlayFX ("lesson_info_hide");

		infoRect.DOScale (Vector3.zero, time).SetEase (scaleOutEase);

		backgroundImage.DOColor (Color.black.Alfa (0f), time)
			.OnComplete (() => {
			this.gameObject.SetActive (false);
		});
	}
}
