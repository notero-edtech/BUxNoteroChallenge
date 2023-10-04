/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem.Classes.Sealed;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ForieroEngine.Music.NotationSystem.Extensions;

namespace ForieroEngine.Music.NotationSystem.Classes
{

    /// <summary>
    /// Using Bravura Text font
    /// </summary>
    public class NSObjectText : NSObject, INSColorable
    {
        INSText _itext;

        Text _text;
        TextMeshProUGUI _textMeshPRO;

        public INSText text
        {
            get
            {
                if (_itext == null)
                {
                    switch (NSSettingsStatic.textMode)
                    {
                        case TextMode.Text:
                            _text = rectTransform.gameObject.GetComponent<Text>();
                            _itext = new NSText(_text) as INSText;
                            break;
                        case TextMode.TextMeshPRO:
                            _textMeshPRO = rectTransform.gameObject.GetComponent<TextMeshProUGUI>();
                            _itext = new NSTextMeshPRO(_textMeshPRO) as INSText;
                            break;
                    }
                }
                return _itext;
            }
        }

        public override void Commit()
        {
            base.Commit();

            text.SetRaycastTarget(base.selectable);
        }

        public override void Reset()
        {
            base.Reset();

            text.SetAlignment(TextAnchor.MiddleCenter);

            switch (NSSettingsStatic.textMode)
            {
                case TextMode.Text: if (_text.font != NSSystemsSettings.TextFont) { _text.font = NSSystemsSettings.TextFont; } break;
                case TextMode.TextMeshPRO: if (_textMeshPRO.font != NSSystemsSettings.TextFontTMP) { _textMeshPRO.font = NSSystemsSettings.TextFontTMP; } break;
            }
            if (text.GetFontSize() != NSSettingsStatic.textFontSize) { text.SetFontSize(NSSettingsStatic.textFontSize); }
            if (!string.IsNullOrEmpty(text.GetText())) { text.SetText(""); }
            rectTransform.SetSize(Vector2.zero);
        }

        #region INSColorable implementation

        public void SetColor(Color color) => text.SetColor(color);
        public void SetAlpha(float alpha) => text.SetColor(text.GetColor().A(alpha));
        public Color GetColor() => text.GetColor();


        #endregion
    }

}
