/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem.Classes.Sealed;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ForieroEngine.Music.NotationSystem.Extensions;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSObjectSMuFL : NSObject, INSColorable
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
                case TextMode.Text: if (_text.font != NSSystemsSettings.SMuFLFont) { _text.font = NSSystemsSettings.SMuFLFont; } break;
                case TextMode.TextMeshPRO: if (_textMeshPRO.font != NSSystemsSettings.SMuFLFontTMP) { _textMeshPRO.font = NSSystemsSettings.SMuFLFontTMP; } break;
            }
            if (text.GetFontSize() != NSSettingsStatic.smuflFontSize) { text.SetFontSize(NSSettingsStatic.smuflFontSize); }
            text.SetText("");
            rectTransform.SetSize(Vector2.zero);
        }

        #region INSColorable implementation

        public void SetColor(Color color) => text.SetColor(color);
        public void SetAlpha(float alpha) => text.SetColor(text.GetColor().A(alpha));
        public Color GetColor() => text.GetColor();

        #endregion
    }
}
