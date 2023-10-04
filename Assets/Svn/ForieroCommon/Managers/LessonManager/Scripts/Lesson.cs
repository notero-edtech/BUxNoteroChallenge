using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Lesson : MonoBehaviour
{
	public Button lessonButton;
	public Image lessonImage;
	public Text lessonText;
	public Image lockImage;
	public Image infoImage;
	public Image completedImage;
	public Image disabledImage;

	public bool destroying = false;

	public bool interactable {
		get {
			return !disabledImage.gameObject.activeSelf;
		}

		set {
			if (disabledImage.gameObject.activeSelf != !value) {
				disabledImage.gameObject.SetActive (!value);
			}
		}
	}

	public bool completed {
		get {
			return completedImage.gameObject.activeSelf;
		}

		set {
			if (value != completedImage.gameObject.activeSelf) {
				completedImage.gameObject.SetActive (value);
			}
		}
	}

	public bool locked {
		get {
			return lockImage.gameObject.activeSelf;
		}

		set {
			if (value != lockImage.gameObject.activeSelf) {
				lockImage.gameObject.SetActive (value);
			}
		}
	}

	public RectTransform rectTransform;

	[HideInInspector]
	public LessonManager lessonManager;
	[HideInInspector]
	public LessonManager.GradeItem gradeItem;
	[HideInInspector]
	public LessonManager.LessonItem lessonItem;

	void Awake ()
	{
		rectTransform = transform as RectTransform;
		interactable = true;
		completed = false;
		locked = false;
	}

	public void OnLessonClick ()
	{
		lessonManager.OnLessonClick (this);
	}

	public void OnLessonInfoClick ()
	{
		lessonManager.OnLessonInfoClick (this);
	}

	void Update ()
	{
		if (!destroying) {
			var range = lessonImage.canvas.pixelRect.width / 4f;

			var distance = (range - Mathf.Abs (Mathf.Clamp (transform.position.x - lessonImage.canvas.pixelRect.width / 2f, -range, range))) / range * 0.1f;

			transform.localScale = new Vector3 (1f + distance, 1f + distance, 1f + distance); 
		}
	}
}
