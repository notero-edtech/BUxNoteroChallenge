using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ForieroEngine;
using ForieroEngine.Extensions;
using DG.Tweening;
using TMPro;

public class UIStatusMessage : MonoBehaviour
{
    public enum Anchor
    {
        Top,
        Bottom,
        Undefined = int.MaxValue
    }

    public enum MessageEnum
    {
        Success,
        Failure,
        Normal,
        Warning,
        Error
    }

    public Color successTextColor = Color.green;
    public Color failureTextColor = Color.red;
    public Color normalTextColor = Color.white;
    public Color warningTextColor = Color.yellow;
    public Color errorTextColor = Color.red;

    public string successSound = "STATUS_MESSAGE_SUCCESS";
    public string failureSound = "STATUS_MESSAGE_FAILURE";
    public string normalSound = "STATUS_MESSAGE_NORMAL";
    public string warningSound = "STATUS_MESSAGE_WARNING";
    public string errorSound = "STATUS_MESSAGE_ERROR";

    public float easeTime = 0.3f;
    public Ease ease = Ease.InOutSine;
     
    [HideInInspector]
    public Anchor anchoredTo = Anchor.Top;

    [HideInInspector]
    public RectTransform rectTransform;
    [HideInInspector]
    public Image image;
    [HideInInspector]
    public TextMeshProUGUI text;

    static Canvas canvas = null;
    static UIStatusMessage statusMessageTop = null;
    static UIStatusMessage statusMessageBottom = null;

    public static Anchor anchorTo = Anchor.Top;

    static GameObject PREFAB_Canvas;
    static GameObject PREFAB_Message;

    Tween tweenPosition;
    Tweener tweenerSize;
    Tween tweenText;

    void Awake()
    {
        image = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        rectTransform = this.transform as RectTransform;
        text.color = normalTextColor;
    }

    void OnDestroy()
    {
        KillTweens();

        if (this == statusMessageTop) statusMessageTop = null;
        if (this == statusMessageBottom) statusMessageBottom = null;
    }

    void KillTweens()
    {
        tweenPosition?.Kill();
        tweenPosition = null;

        tweenerSize?.Kill();
        tweenerSize = null;

        tweenText?.Kill();
        tweenText = null;
    }

    Color GetMessageColor(MessageEnum messageEnum)
    {
        Color c = normalTextColor;
        switch (messageEnum)
        {
            case MessageEnum.Success:
                c = successTextColor;
                break;
            case MessageEnum.Failure:
                c = failureTextColor;
                break;
            case MessageEnum.Normal:
                c = normalTextColor;
                break;
            case MessageEnum.Warning:
                c = warningTextColor;
                break;
            case MessageEnum.Error:
                c = errorTextColor;
                break;
        }
        return c;
    }

    void PlayMessageSound(MessageEnum messageEnum)
    {
        switch (messageEnum)
        {
            case MessageEnum.Success:
                SM.PlayFX(successSound);
                break;
            case MessageEnum.Failure:
                SM.PlayFX(failureSound);
                break;
            case MessageEnum.Normal:
                SM.PlayFX(normalSound);
                break;
            case MessageEnum.Warning:
                SM.PlayFX(warningSound);
                break;
            case MessageEnum.Error:
                SM.PlayFX(errorSound);
                break;
        }
    }

    public void ShowMessageInternal(string text, float showTime, MessageEnum messageEnum)
    {
        KillTweens();

        Vector2 inVector = new Vector2(0, 0) * (anchoredTo == Anchor.Bottom ? 1f : -1f);

        tweenText = this.text.DOColor(this.text.color.A(0), easeTime / 2f).OnComplete(() =>
        {
            this.text.text = text;
            tweenerSize = DOVirtual.Float(rectTransform.GetWidth(), this.text.preferredWidth + 40f, easeTime / 2f, rectTransform.SetWidth);
            tweenText = this.text.DOColor(GetMessageColor(messageEnum).A(1), easeTime / 2f);
            PlayMessageSound(messageEnum);
        });

        tweenPosition = rectTransform.DOAnchorPos(inVector, easeTime)
                            .SetEase(ease)
                            .OnComplete(() => { if (showTime > 0) DestroyInternal(showTime); }
                            );
    }

    static UIStatusMessage CreateInternal(Anchor anchor = Anchor.Undefined)
    {
        if (!PREFAB_Canvas) PREFAB_Canvas = ForieroSettings.instance.statusMessage.prefabCanvas == null ? Resources.Load<GameObject>("PREFAB_Status_Message_Canvas") : ForieroSettings.instance.statusMessage.prefabCanvas;
        if (!PREFAB_Message) PREFAB_Message = ForieroSettings.instance.statusMessage.prefabMessage == null ? Resources.Load<GameObject>("PREFAB_Status_Message") : ForieroSettings.instance.statusMessage.prefabMessage;

        if (!PREFAB_Canvas) Debug.LogError("Missing UIStatusMessage Canvas PREFAB");
        if (!PREFAB_Message) Debug.LogError("Missing UIStatusMessage PREFAB");

        if (!canvas)
        {
            canvas = (Instantiate(PREFAB_Canvas, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<Canvas>();
            canvas.sortingOrder = ForieroSettings.instance.statusMessage.sortingOrder;
        }

        UIStatusMessage m = null;

        anchor = anchor == Anchor.Undefined ? anchorTo : anchor;

        switch (anchor)
        {
            case Anchor.Top:
                if (!statusMessageTop)
                {
                    statusMessageTop = (Instantiate(PREFAB_Message, Vector3.zero, Quaternion.identity, canvas.transform) as GameObject).GetComponent<UIStatusMessage>();
                    statusMessageTop.rectTransform.SetPivotAndAnchors(new Vector2(0.5f, 1f));
                    statusMessageTop.anchoredTo = Anchor.Top;
                    statusMessageTop.rectTransform.anchoredPosition = new Vector2(0, statusMessageTop.rectTransform.GetHeight());
                }

                m = statusMessageTop;
                break;
            case Anchor.Bottom:
                if (!statusMessageBottom)
                {
                    statusMessageBottom = (Instantiate(PREFAB_Message, Vector3.zero, Quaternion.identity, canvas.transform) as GameObject).GetComponent<UIStatusMessage>();
                    statusMessageBottom.rectTransform.SetPivotAndAnchors(new Vector2(0.5f, 0f));
                    statusMessageBottom.anchoredTo = Anchor.Bottom;
                    statusMessageBottom.rectTransform.anchoredPosition = new Vector2(0, -statusMessageBottom.rectTransform.GetHeight());               
                }

                m = statusMessageBottom;
                break;
        }

        return m;
    }

    void DestroyInternal(float delay)
    {
        KillTweens();

        Vector2 outVector = new Vector2(0, rectTransform.GetHeight()) * (anchoredTo == Anchor.Bottom ? -1f : 1f);

        tweenPosition = rectTransform.DOAnchorPos(outVector, easeTime).SetDelay(delay).SetEase(ease).OnComplete(() =>
        {
            if (this == statusMessageTop) statusMessageTop = null;
            if (this == statusMessageBottom) statusMessageBottom = null;

            Destroy(this.gameObject);

            if (!statusMessageTop && !statusMessageBottom)
            {
                Destroy(canvas.gameObject);
                canvas = null;
            }
        });
    }

    public static void Destroy(float delay, Anchor anchor = Anchor.Undefined)
    {
        switch (anchor)
        {
            case Anchor.Top:
                statusMessageTop?.DestroyInternal(delay);
                statusMessageTop = null;
                break;
            case Anchor.Bottom:
                statusMessageBottom?.DestroyInternal(delay);
                statusMessageBottom = null;
                break;
            case Anchor.Undefined:
                statusMessageTop?.DestroyInternal(delay);
                statusMessageTop = null;
                statusMessageBottom?.DestroyInternal(delay);
                statusMessageBottom = null;
                break;
        }
    }

    public static void ShowMessage(string text, float showTime = 5f, MessageEnum messageEnum = MessageEnum.Normal, Anchor anchor = Anchor.Undefined)
    {
        var m = CreateInternal(anchor);
        m?.ShowMessageInternal(text, showTime, messageEnum);
    }
}
