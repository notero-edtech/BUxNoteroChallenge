using System.Collections;
using ForieroEngine.Music.NotationSystem.Extensions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[Singleton]
public class NSTestSuiteXMLViewer : MonoBehaviour, ISingleton
{
    public static NSTestSuiteXMLViewer Instance => Singleton<NSTestSuiteXMLViewer>.instance;

    public RectTransform contentRt;
    public TextMeshProUGUI textTmp;

    public void SetText(string text)
    {
        StartCoroutine(SetTextInternal(text));
        IEnumerator SetTextInternal(string t)
        {
            textTmp.text = t;
            yield return null;
            contentRt.SetSize(contentRt.GetSize().x, textTmp.preferredHeight);
            textTmp.rectTransform.SetSize(contentRt.GetSize().x, textTmp.preferredHeight);
        }
    }
}