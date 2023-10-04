using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TTUILanguageSelectorItem : MonoBehaviour
{
	public TTUILanguageSelector langSelector;
	public Lang.LanguageCode lang;
	public Text text;

	public void OnClick ()
	{
		langSelector.OnLangItemClick (this);
	}
}
