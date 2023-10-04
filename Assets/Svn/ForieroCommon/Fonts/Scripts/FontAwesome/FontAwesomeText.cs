using UnityEngine;
using UnityEngine.UI;
using FontAwesome;

[RequireComponent (typeof(Text))]
public class FontAwesomeText : MonoBehaviour
{
	public FontAwesomeIconEnum icon;

	[HideInInspector]
	public Text text;

	void Awake ()
	{
		text = GetComponent<Text> ();
	}

	public void SetIcon (FontAwesomeIconEnum iconType)
	{
		if (text) {
			text.text = iconType.ToFontAwesomeString ();
		}
	}
}