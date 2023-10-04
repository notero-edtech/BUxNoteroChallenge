/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;

public class NSTestSuiteButton : MonoBehaviour
{
    public enum ButtonType { Category, FileName, LoadXML, LoadCode, LoadMP3, ViewPNG, ViewXML }
    public enum XmlType { xml, mxl }
    public NSTestSuiteXML xml;

    public TextMeshProUGUI text;
    public string category;
    public string file;
    public XmlType xmlType;

    public string ItemName
    {
        get => text ? text.text : "";
        set { if (text) { text.text = value; } }
    }

    public ButtonType buttonType = ButtonType.Category;
    public void OnButtonClick() => xml.OnItemClick(this);
    
}
