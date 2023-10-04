using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class TTUITooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

	public bool doTooltip = false;
	public TTLangRecord langRecord;
	public float tooltipDelay = 1f;

	public TTUITooltipStyle style;
	public LangFontDefinition fontDefinition;

	TTUITooltipObject tooltip;

	public void OnPointerEnter (PointerEventData eventData){
		if (!doTooltip) return;
		tooltip = gameObject.AddComponent<TTUITooltipObject>() as TTUITooltipObject;
		tooltip.tooltipBackground = style.tooltipBackground;
		tooltip.font = fontDefinition.GetFont(Lang.selectedLanguage);
		tooltip.textColor = style.textColor;
		tooltip.fontSize = style.fontSize;
		tooltip.text = Lang.GetText(langRecord.dictionary, langRecord.id, langRecord.defaultText);
		tooltip.delay = tooltipDelay;	
	}

	public void OnPointerExit (PointerEventData eventData){
		
	}

	public void OnPointerDown (PointerEventData eventData){
		if(tooltip) Destroy (tooltip);
	}

	public void OnPointerUp (PointerEventData eventData){
		
	}
}
