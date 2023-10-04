/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ForieroEngine.Music.SMuFL.GlyphNames;
using TMPro;

public class SMuFLText : MonoBehaviour
{
    [SerializeField]
    private string value;

    public List<GlyphNames> glyphNames = new List<GlyphNames>();

    public void Apply()
    {
        string s = "";
        foreach (GlyphNames gn in glyphNames)
        {
            s += ((char)(int)gn).ToString();
        }

        Text text = GetComponent<Text>();

        if (text)
        {
            text.text = s;
        }

        TextMeshPro tmp = GetComponent<TextMeshPro>();
        if (tmp)
        {
            tmp.text = s;
        }

        TextMeshProUGUI tmpUI = GetComponent<TextMeshProUGUI>();
        if (tmpUI)
        {
            tmpUI.text = s;
        }
    }
}
