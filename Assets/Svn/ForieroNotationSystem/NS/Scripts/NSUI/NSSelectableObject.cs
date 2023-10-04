/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

namespace ForieroEngine.Music.NotationSystem.Classes
{
	public class NSSelectableObject : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public NSObject nsObject = null;

		public bool horizontal = true;
		public bool vertical = true;

		public IBeginDragHandler iBeginDragHandler;
		public IDragHandler iDragHandler;
		public IEndDragHandler iEndDragHandler;
		public IPointerDownHandler iPointerDownHandler;
		public IPointerUpHandler iPointerUpHandler;
		public IPointerClickHandler iPointerClickHandler;
		public IPointerEnterHandler iPointerEnterHandler;
		public IPointerExitHandler iPointerExitHandler;


		public void OnBeginDrag (PointerEventData eventData)
		{
			//		touchPosition = TouchPosition ();
			//		ns.updateMove = false;
			if (iBeginDragHandler != null) {
				iBeginDragHandler.OnBeginDrag (eventData);
			}
		}


		public void OnDrag (PointerEventData eventData)
		{
			//		if (!pinch && (Input.touchCount == 1 || Input.GetMouseButton (0))) {
			//			Vector2 diff = (touchPosition - TouchPosition ()) / fixedCanvas.scaleFactor;
			//
			//			if (!ns.hDragMove) {
			//				diff.x = 0;
			//			}
			//
			//			if (!ns.vDragMove) {
			//				diff.y = 0;
			//			}
			//
			//			movableCamera.anchoredPosition += diff;
			//			fixedCamera.anchoredPosition += new Vector2 (0, diff.y);
			//			(transform as RectTransform).anchoredPosition += new Vector2 (0, diff.y);
			//
			//			touchPosition = TouchPosition ();
			//		} else {
			//			pinch = true;
			//			touchPosition = TouchPosition ();
			//		}
			if (iDragHandler != null) {
				iDragHandler.OnDrag (eventData);
			}
		}

		public void OnEndDrag (PointerEventData eventData)
		{
			//		touchPosition = Vector2.zero;
			//		ns.updateMove = true;
			//		pinch = false;
			if (iEndDragHandler != null) {
				iEndDragHandler.OnEndDrag (eventData);
			}
		}

		public void OnPointerDown (PointerEventData eventData)
		{
			if (iPointerDownHandler != null) {
				iPointerDownHandler.OnPointerDown (eventData);
			}
		}

		public void OnPointerUp (PointerEventData eventData)
		{
			if (iPointerUpHandler != null) {
				iPointerUpHandler.OnPointerUp (eventData);
			}
		}

		public void OnPointerClick (PointerEventData eventData)
		{
			if (iPointerClickHandler != null) {
				iPointerClickHandler.OnPointerClick (eventData);
			}
		}

		public void OnPointerEnter (PointerEventData eventData)
		{
			if (iPointerEnterHandler != null) {
				iPointerEnterHandler.OnPointerEnter (eventData);
			}
		}

		public void OnPointerExit (PointerEventData eventData)
		{
			if (iPointerExitHandler != null) {
				iPointerExitHandler.OnPointerExit (eventData);
			}
		}
	}
}
