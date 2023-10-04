using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Extensions;

using DG.Tweening;

public class TTUILanguageSelector : MonoBehaviour
{

    public bool usePlayerManager = false;

    public RectTransform foldoutTransform;
    public RectTransform languageTransform;
    public RectTransform buttonContainer;
    public Image foldoutImage;
    public Text foldoutText;

    public RectTransform buttonUp;
    public RectTransform buttonDown;

    public RectTransform languagesRect;

    public Button.ButtonClickedEvent languageChange;

    public GameObject PREFAB_LangButton;

    List<TTUILanguageSelectorItem> langButtons = new List<TTUILanguageSelectorItem>();

    [HideInInspector]
    public Button.ButtonClickedEvent OnShowAvailableLanguages;

    [HideInInspector]
    public Button.ButtonClickedEvent OnHideAvailableLanguages;

    List<Transform> langButtonTransforms = new List<Transform>();

    string UppercaseFirst(string s)
    {
        // Check for empty string.
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        // Return char and concat substring.
        return char.ToUpper(s[0]) + s.Substring(1);
    }

    void Start()
    {
        Initialize();
    }

    float buttonHeight = 0;

    void Initialize()
    {
        languagesRect.gameObject.SetActive(false);

        //int selectedIndex = 0;

        langButtonTransforms.Add(buttonUp);

        int langIterator = 0;

        for (int i = 0; i < LangSettings.instance.supportedLanguages.Count; i++)
        {
            if (!LangSettings.instance.supportedLanguages[i].included)
                continue;

            if (Lang.selectedLanguage == LangSettings.instance.supportedLanguages[i].langCode)
            {
                //  selectedIndex = i;
            }

            GameObject go = Instantiate<GameObject>(PREFAB_LangButton);
            go.transform.SetParent(buttonContainer, false);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            TTUILanguageSelectorItem item = go.GetComponent<TTUILanguageSelectorItem>();
            item.lang = LangSettings.instance.supportedLanguages[i].langCode;
            item.text.text = UppercaseFirst(Lang.GetLanguageString(LangSettings.instance.supportedLanguages[i].langCode));
            item.langSelector = this;
            RectTransform rt = item.transform as RectTransform;
            buttonHeight = rt.GetHeight();
            rt.anchoredPosition = new Vector2(0, -buttonHeight * langIterator);
            go.SetActive(false);
            langButtons.Add(item);

            if (Lang.selectedLanguage == LangSettings.instance.supportedLanguages[i].langCode)
            {
                foldoutText.text = item.text.text;
            }

            langButtonTransforms.Add(go.transform);

            langIterator++;
        }

        langButtonTransforms.Add(buttonDown);

        buttonContainer.SetSize(new Vector2(buttonContainer.GetSize().x, buttonHeight * langIterator));
    }

    public void OnLangItemClick(TTUILanguageSelectorItem aItem)
    {
        Lang.selectedLanguage = aItem.lang;
        foldoutText.text = aItem.text.text;
        languageChange.Invoke();
        OnFoldOutClick();
    }

    bool foldout = false;

    public void OnFoldOutClick()
    {
        foldout = !foldout;

        if (foldout)
        {
            In();
        }
        else
        {
            Out();
        }

        foldoutImage.rectTransform.localEulerAngles = new Vector3(0, 0, foldout ? 90 : -90);
    }

    public float effectDuration = 0.3f;
    public float effectDelay = 0.15f;
    public Ease effectEaseIn = Ease.InOutBack;
    public Ease effectEaseOut = Ease.InBack;

    //bool isIn = false;

    void In()
    {
        OnShowAvailableLanguages.Invoke();
        DOTween.Kill("LANGUAGE_OUT");
        languagesRect.gameObject.SetActive(true);
        float delay = 0;
        for (int i = 0; i < langButtonTransforms.Count; i++)
        {
            Transform item = langButtonTransforms[i];
            item.gameObject.SetActive(foldout);

            if (i != 0 && i != langButtonTransforms.Count - 1)
            {
                RectTransform rt = item as RectTransform;

                if (rt.anchoredPosition.y + buttonContainer.anchoredPosition.y - buttonHeight <= 0f && rt.anchoredPosition.y + buttonContainer.anchoredPosition.y >= -5f * buttonHeight)
                {
                    // is visible //
                }
                else
                {
                    item.localScale = Vector3.one;
                    continue;
                }
            }

            item.localScale = Vector3.zero;

            delay += effectDelay;

            item.DOScale(Vector3.one, effectDuration)
            .OnStart(() =>
            {
                SM.PlayFX("pop");
            })
            .SetEase(effectEaseIn)
            .SetDelay(delay)
            .SetId("LANGUAGE_IN");
        }

        //isIn = true;
    }

    int tweenCount = 0;

    void Out()
    {
        DOTween.Kill("LANGUAGE_IN");
        tweenCount = 0;
        float indexer = 0;
        int count = 0;

        for (int i = 0; i < langButtonTransforms.Count; i++)
        {
            Transform item = langButtonTransforms[i];

            if (i != 0 && i != langButtonTransforms.Count - 1)
            {
                RectTransform rt = item as RectTransform;

                if (rt.anchoredPosition.y + buttonContainer.anchoredPosition.y - buttonHeight <= 0f && rt.anchoredPosition.y + buttonContainer.anchoredPosition.y >= -5f * buttonHeight)
                {
                    count++;
                }
            }
        }

        count += 2;

        for (int i = 0; i < langButtonTransforms.Count; i++)
        {
            Transform item = langButtonTransforms[i];

            if (i != 0 && i != langButtonTransforms.Count - 1)
            {
                RectTransform rt = item as RectTransform;

                if (rt.anchoredPosition.y + buttonContainer.anchoredPosition.y - buttonHeight <= 0f && rt.anchoredPosition.y + buttonContainer.anchoredPosition.y >= -5f * buttonHeight)
                {
                    // is visible //
                }
                else
                {
                    item.localScale = Vector3.zero;
                    continue;
                }
            }

            indexer++;

            item.DOScale(Vector3.zero, effectDuration)
            .SetEase(effectEaseOut)
            .SetDelay(effectDelay * (count - indexer))
            .OnStart(() =>
            {
                tweenCount++;
                SM.PlayFX("pop");
            })
            .OnComplete(() =>
            {
                item.gameObject.SetActive(foldout);
                tweenCount--;
                if (tweenCount == 0)
                {
                    languagesRect.gameObject.SetActive(false);
                    OnHideAvailableLanguages.Invoke();
                }
            })
            .SetId("LANGUAGE_OUT");
        }

        //isIn = false;
    }

    public void OnButtonUp()
    {
        buttonContainer.DOAnchorPos(new Vector2(buttonContainer.anchoredPosition.x, buttonContainer.anchoredPosition.y - buttonHeight), 0.2f);
    }

    public void OnButtonDown()
    {
        buttonContainer.DOAnchorPos(new Vector2(buttonContainer.anchoredPosition.x, buttonContainer.anchoredPosition.y + buttonHeight), 0.2f);
    }
}
