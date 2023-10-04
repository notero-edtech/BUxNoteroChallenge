using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using ForieroEngine.Extensions;

public class TTUITooltipObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

	public string text = "";
	public float delay = 1f;
	public Vector3 offset = Vector3.zero;
	public bool visible = false;
	public float alfa = 0.8f;

	public Texture2D tooltipBackground;
	public Font font;
	public Color textColor;
	public int fontSize;

	GUIStyle stlTooltip = new GUIStyle();
	Rect rect;
	Vector2 size;

	void Start()
	{
		if (font) stlTooltip.font = font;
		stlTooltip.fontSize = fontSize;
		stlTooltip.fontStyle = FontStyle.Bold;
		stlTooltip.normal.background = tooltipBackground;
		stlTooltip.normal.textColor = textColor;
		stlTooltip.alignment = TextAnchor.MiddleCenter;
		stlTooltip.border.bottom = 5;
		stlTooltip.border.top = 5;
		stlTooltip.border.left = 5;
		stlTooltip.border.right = 5;

		stlTooltip.padding.bottom = 0;
		stlTooltip.padding.top = 0;
		stlTooltip.padding.left = 0;
		stlTooltip.padding.right = 0;

		stlTooltip.margin.bottom = 0;
		stlTooltip.margin.top = 0;
		stlTooltip.margin.left = 0;
		stlTooltip.margin.right = 0;
		stlTooltip.stretchWidth = true;
		StartCoroutine(Show());
	}

	IEnumerator Show()
	{
		yield return new WaitForSeconds(delay);
		visible = true;
	}

	void OnDisable()
	{
		Destroy(this);
	}

	void OnGUI()
	{
		if (visible)
		{
			var x = Event.current.mousePosition.x;
			var y = Event.current.mousePosition.y;

			Vector2 size = stlTooltip.CalcSize(new GUIContent(text));

			size.x += 10;
			size.y += 0;

			if (x - size.x / 2f - 10f < 0) x = size.x / 2f + 10;
			if (x - size.x / 2f + size.x + 10f > Screen.width) x = Screen.width - size.x / 2f - 10;
			size.y += 10f;
			y += 20;

			if (y + size.y > Screen.height) y = Event.current.mousePosition.y - size.y - 10;
			//GUI.color = GUI.color.Alfa(alfa);
			//GUI.contentColor = GUI.contentColor.Alfa(alfa);
			GUI.backgroundColor = GUI.backgroundColor.Alfa(alfa);
			GUI.Box(new Rect(x - size.x / 2f, y, size.x, size.y), text, stlTooltip);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{

	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Destroy(this);
	}

	public void OnPointerDown(PointerEventData eventData)
	{

	}

	public void OnPointerUp(PointerEventData eventData)
	{

	}
}
