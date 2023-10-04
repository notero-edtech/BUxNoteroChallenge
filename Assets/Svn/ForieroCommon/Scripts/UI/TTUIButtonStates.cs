using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TTUIButton))]
public class TTUIButtonStates : MonoBehaviour
{

    [HideInInspector]
    public TTUIButton button;

    public TTUIButtonState disabledState;

    public int state = 0;

    public List<TTUIButtonState> buttonStates = new List<TTUIButtonState>();

    Image background;
    Image icon;
    Text text;

    public bool disabled = false;

    public TTUIButtonState buttonState
    {
        get
        {
            return buttonStates[state];
        }
    }

    public void Disable()
    {
        SetButtonState(disabledState);
        //SetButtonStateOver(disabledState);
        disabled = true;
    }

    public void Enable()
    {
        disabled = false;
        if (state >= 0 && state < buttonStates.Count) SetButtonState(buttonStates[state]);
    }

    public void SetState(int aState, bool changeStyle = true)
    {
        state = aState;
        if (buttonStates.Count == 0) return;
        if (state >= buttonStates.Count) state = 0;
        if (changeStyle) SetButtonState(buttonStates[state]);
    }

    public void SetState(string stateName, bool changeStyle = true)
    {
        for (int i = 0; i < buttonStates.Count; i++)
        {
            if (stateName.Equals(buttonStates[i].name))
            {
                SetState(i, changeStyle);
            }
        }
    }

    void Awake()
    {
        button = GetComponent<TTUIButton>();
        background = GetComponent<Image>();
        icon = button.icon;
        text = button.text;
    }

    void Start()
    {
        button = GetComponent<TTUIButton>();
        if (button.stateButton) SetButtonState(buttonStates[state]);
    }

    //Tweener bacgroundTweener;
    //Tweener iconTweener;
    //Tweener textTweener;

    void SetButtonState(TTUIButtonState aButtonState)
    {

        //if (bacgroundTweener != null) bacgroundTweener.Kill();
        if (!aButtonState) return;

        if (background && aButtonState.backgroundStyle)
        {
            background.sprite = aButtonState.backgroundStyle.sprite ?? background.sprite;
            background.color = aButtonState.backgroundStyle.color;
        }

        //if (iconTweener != null) iconTweener.Kill();

        if (icon && aButtonState.iconStyle)
        {
            icon.sprite = aButtonState.iconStyle.sprite ?? icon.sprite;
            icon.color = aButtonState.iconStyle.color;
        }

        // if (textTweener != null) textTweener.Kill();

        if (text != null)
        {
            text.color = aButtonState.iconStyle.color;
        }
    }

    void SetButtonStateOver(TTUIButtonState aButtonState)
    {
        //if (bacgroundTweener != null) bacgroundTweener.Kill();
        if (!aButtonState) return;

        if (background && aButtonState.backgroundStyle)
        {
            background.sprite = aButtonState.backgroundStyle.sprite ?? background.sprite;
            background.color = aButtonState.backgroundStyle.colorOver;
        }

        //if (iconTweener != null) iconTweener.Kill();

        if (icon && aButtonState.iconStyle)
        {
            icon.sprite = aButtonState.iconStyle.sprite ?? icon.sprite;
            icon.color = aButtonState.iconStyle.colorOver;
        }

        //if (textTweener != null) textTweener.Kill();

        if (text != null)
        {
            text.color = aButtonState.iconStyle.colorOver;
        }
    }
}
