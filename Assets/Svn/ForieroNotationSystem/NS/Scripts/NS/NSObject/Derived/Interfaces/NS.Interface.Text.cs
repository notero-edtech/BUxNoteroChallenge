/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ForieroEngine.Music.NotationSystem.Classes.Sealed
{
    public class NSText : INSText
    {
        public NSText(Text text)
        {
            this.text = text;
        }

        public readonly Text text;

        #region INSText implementation

        public Color GetColor()
        {
            return text.color;
        }

        public void SetColor(Color color)
        {
            if (text.color != color)
            {
                text.color = color;
            }
        }

        public string GetText()
        {
            return text.text;
        }

        public void SetText(string text)
        {
            this.text.text = text;
        }

        public TextAnchor GetAlignment()
        {
            return text.alignment;
        }

        public void SetAlignment(TextAnchor alignment)
        {
            text.alignment = alignment;
        }

        public float GetPreferredWidth()
        {
            return text.preferredWidth;
        }

        public float GetPreferredHeight()
        {
            return text.preferredHeight;
        }

        public bool GetRaycastTarget()
        {
            return text.raycastTarget;
        }

        public void SetRaycastTarget(bool raycastTarget)
        {
            text.raycastTarget = raycastTarget;
        }

        public int GetFontSize()
        {
            return text.fontSize;
        }

        public void SetFontSize(int fontSize)
        {
            text.fontSize = fontSize;
        }

        #endregion

    }

    public class NSTextMeshPRO : INSText
    {
        #region INSText implementation

        public NSTextMeshPRO(TextMeshProUGUI text)
        {
            this.text = text;
        }

        public readonly TextMeshProUGUI text;

        public Color GetColor()
        {
            return text.color;
        }

        public void SetColor(Color color)
        {
            if (text.color != color)
            {
                text.color = color;
            }
        }

        public string GetText()
        {
            return text.text;
        }

        public void SetText(string text)
        {
            this.text.text = text;
        }

        public TextAnchor GetAlignment()
        {
            TextAnchor result = TextAnchor.MiddleCenter;
            switch (text.alignment)
            {
                case TextAlignmentOptions.Bottom:
                    result = TextAnchor.LowerCenter;
                    break;
                case TextAlignmentOptions.BottomLeft:
                    result = TextAnchor.LowerLeft;
                    break;
                case TextAlignmentOptions.BottomRight:
                    result = TextAnchor.LowerRight;
                    break;
                case TextAlignmentOptions.Top:
                    result = TextAnchor.UpperCenter;
                    break;
                case TextAlignmentOptions.TopLeft:
                    result = TextAnchor.UpperLeft;
                    break;
                case TextAlignmentOptions.TopRight:
                    result = TextAnchor.UpperRight;
                    break;
                case TextAlignmentOptions.Center:
                    result = TextAnchor.MiddleCenter;
                    break;
                case TextAlignmentOptions.Left:
                    result = TextAnchor.MiddleLeft;
                    break;
                case TextAlignmentOptions.Right:
                    result = TextAnchor.MiddleRight;
                    break;
            }
            return result;
        }

        public void SetAlignment(TextAnchor alignment)
        {
            switch (alignment)
            {
                case TextAnchor.LowerCenter:
                    text.alignment = TextAlignmentOptions.Bottom;
                    break;
                case TextAnchor.LowerLeft:
                    text.alignment = TextAlignmentOptions.BottomLeft;
                    break;
                case TextAnchor.LowerRight:
                    text.alignment = TextAlignmentOptions.BottomRight;
                    break;
                case TextAnchor.UpperCenter:
                    text.alignment = TextAlignmentOptions.Top;
                    break;
                case TextAnchor.UpperLeft:
                    text.alignment = TextAlignmentOptions.TopLeft;
                    break;
                case TextAnchor.UpperRight:
                    text.alignment = TextAlignmentOptions.TopRight;
                    break;
                case TextAnchor.MiddleCenter:
                    text.alignment = TextAlignmentOptions.Center;
                    break;
                case TextAnchor.MiddleLeft:
                    text.alignment = TextAlignmentOptions.Left;
                    break;
                case TextAnchor.MiddleRight:
                    text.alignment = TextAlignmentOptions.Right;
                    break;
            }
        }

        public float GetPreferredWidth()
        {
            return text.preferredWidth;
        }

        public float GetPreferredHeight()
        {
            return text.preferredHeight;
        }

        public bool GetRaycastTarget()
        {
            return text.raycastTarget;
        }

        public void SetRaycastTarget(bool raycastTarget)
        {
            text.raycastTarget = raycastTarget;
        }

        public int GetFontSize()
        {
            return (int)text.fontSize;
        }

        public void SetFontSize(int fontSize)
        {
            text.fontSize = (float)fontSize;
        }

        #endregion


    }
}
