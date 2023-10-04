using DG.Tweening;
using ForieroEngine;
using UnityEngine;
using UnityEngine.UI;
using ForieroEngine.Extensions;
using ForieroEngine.Purchasing;

public class GradePurchase : MonoBehaviour
{
	public Image backgroundImage;
	public RectTransform purchaseRect;

	public float time = 0.5f;

	public Ease scaleInEase;
	public Ease scaleOutEase;

	public System.Action finished;

	public void OnPurchaseClick()
	{
		ParentalLock.Show(4, (ok) =>
		{
			if (ok)
			{
				Store.PurchaseProduct(LessonManager.selectedGradeItem.id,
					(id, receipt) =>
					{
						Hide();
					},
					(id, reason) =>
					{
						Hide();
					});
			}
		});
	}

	public void OnRestoreClick()
	{
		//Store.HookOnProduct(LessonManager.selectedGradeItem.id,
		//	(id, receipt) =>
		//	{
		//		Hide();
		//	},
		//	(id, reason) =>
		//	{
		//		Hide();
		//	},
		//	(id) =>
		//	{
		//		Hide();
		//	}
		//);
	}

	public void Show()
	{
		SM.PlayFX("lesson_info_show");

		this.gameObject.SetActive(true);

		purchaseRect.localScale = Vector3.zero;

		purchaseRect.DOScale(Vector3.one, time).SetEase(scaleInEase);

		backgroundImage.color = Color.black.Alfa(0f);

		backgroundImage.DOColor(Color.black.Alfa(1f), time);
	}

	public void Hide()
	{
		SM.PlayFX("lesson_info_hide");

		purchaseRect.DOScale(Vector3.zero, time).SetEase(scaleOutEase);

		backgroundImage.DOColor(Color.black.Alfa(0f), time)
			.OnComplete(() =>
			{
				finished();
				this.gameObject.SetActive(false);
			});
	}
}
