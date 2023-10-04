using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Extensions;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TTUITextTMP : MonoBehaviour
{

    public enum FormatEnum
    {
        none,
        lower_case,
        upper_case
    }

    public enum ResizeEnum
    {
        none,
        parent,
        self,
        self_fit
    }

    public ResizeEnum resizeType = ResizeEnum.none;

    public Vector2 padding;
    public bool circleButton = false;
    public LangFontDefinition fontDefinition;
    public FormatEnum format = FormatEnum.none;

    [HideInInspector]
    public TextMeshProUGUI text;

    public bool fontScaleFactor = false;

    int originalFontSize = 0;

    public bool applyTranslation = true;
    public TTLangRecord langRecord;

    RectTransform parentTransform;

    public static List<TTUITextTMP> labels = new List<TTUITextTMP>();

    public static void RefreshAll()
    {
        foreach (TTUITextTMP label in labels)
        {
            label?.Refresh();
        }
    }

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();

        originalFontSize = (int)text.fontSize;

        if (!labels.Contains(this)) labels.Add(this);

        parentTransform = transform.parent as RectTransform;
    }

    // Use this for initialization
    void Start()
    {
        Refresh();
    }

    void OnDestroy()
    {
        labels.Remove(this);
    }

    void OnEnable()
    {
        if (needsToBeResized)
        {
            Resize();
        }
    }

    public void SetText(string setText)
    {
        text.text = setText;
        Refresh();
    }

    public void Refresh()
    {
        if (applyTranslation)
        {
            switch (format)
            {
                case FormatEnum.none:
                    text.text = Lang.GetText(langRecord.dictionary, langRecord.id, langRecord.defaultText);
                    break;
                case FormatEnum.lower_case:
                    text.text = Lang.GetText(langRecord.dictionary, langRecord.id, langRecord.defaultText).ToLower();
                    break;
                case FormatEnum.upper_case:
                    text.text = Lang.GetText(langRecord.dictionary, langRecord.id, langRecord.defaultText).ToUpper();
                    break;
            }
        }

        Resize();
    }

    bool needsToBeResized = false;

    void Resize()
    {
        if (gameObject.activeSelf == false || gameObject.activeInHierarchy == false)
        {
            needsToBeResized = true;
            return;
        }

        switch (resizeType)
        {
            case ResizeEnum.none:
                break;
            case ResizeEnum.parent:
                ResizeParent();
                break;
            case ResizeEnum.self:
                ResizeSelf();
                break;
            case ResizeEnum.self_fit:
                ResizeSelfFit();
                break;
        }

        this.FireAction(1, () =>
        {
            switch (resizeType)
            {
                case ResizeEnum.none:
                    break;
                case ResizeEnum.parent:
                    ResizeParent();
                    break;
                case ResizeEnum.self:
                    ResizeSelf();
                    break;
                case ResizeEnum.self_fit:
                    ResizeSelfFit();
                    break;
            }
            needsToBeResized = false;
        });
    }

    void ResizeSelf()
    {
        if (!parentTransform) return;

        float parentWidth = parentTransform.GetSize().x - padding.x * 2f;
        float parentHeight = parentTransform.GetSize().y - padding.y * 2f;
        float scaleFactor = 1f;

        if (text.overflowMode != TextOverflowModes.Overflow)
        {
            this.FireAction(1, () =>
            {
                if (parentHeight < (text.preferredHeight * text.textInfo.lineCount))
                {
                    scaleFactor = parentHeight / (text.preferredHeight * text.textInfo.lineCount);
                    scaleFactor *= 1.25f;
                }

                text.fontSize = Mathf.FloorToInt(originalFontSize * scaleFactor);
            });
        }
        else
        {
            if (parentWidth < text.preferredWidth)
            {
                scaleFactor = parentWidth / text.preferredWidth;
            }
            text.fontSize = Mathf.FloorToInt(originalFontSize * scaleFactor);
        }
    }

    void ResizeSelfFit()
    {
        if (!parentTransform) return;

        float parentWidth = parentTransform.GetSize().x - padding.x * 2f;
        float parentHeight = parentTransform.GetSize().y - padding.y * 2f;
        float scaleFactorWidth = text.preferredWidth / parentWidth;
        float scaleFactorHeight = text.preferredHeight / parentHeight;
        float scaleFactor = 1f;

        if (scaleFactorWidth > 1f && scaleFactorHeight > 1f)
        {
            scaleFactor = 1f / (scaleFactorWidth > scaleFactorHeight ? scaleFactorWidth : scaleFactorHeight);
        }
        else if (scaleFactorWidth > 1f)
        {
            scaleFactor = 1f / scaleFactorWidth;
        }
        else if (scaleFactorHeight > 1f)
        {
            scaleFactor = 1f / scaleFactorHeight;
        }
        else if (scaleFactorWidth < 1f && scaleFactorHeight < 1f)
        {
            scaleFactor = 1f / (scaleFactorWidth > scaleFactorHeight ? scaleFactorWidth : scaleFactorHeight);
        }
        else if (scaleFactorWidth < 1f)
        {
            scaleFactor = 1f / scaleFactorWidth;
        }
        else if (scaleFactorHeight < 1f)
        {
            scaleFactor = 1f / scaleFactorHeight;
        }

        text.fontSize = Mathf.FloorToInt(text.fontSize * scaleFactor);
    }

    void ResizeParent()
    {
        if (!parentTransform)
            return;
        if (circleButton)
        {
            parentTransform.SetSize(new Vector2(text.preferredWidth + padding.x * 2f, text.preferredWidth + padding.x * 2f));
        }
        else
        {
            parentTransform.SetSize(new Vector2(text.preferredWidth + padding.x * 2f, text.preferredHeight + padding.y * 2f));
        }
    }
}
