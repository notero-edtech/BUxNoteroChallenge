using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using NonSerializableAttribute = System.NonSerializedAttribute;

[RequireComponent(typeof(Button))]
public class TTUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    public TTUIButtonStates buttonStates;
    public bool stateButton = false;
    public bool useInitColor = false;
    public Image icon;
    public Text text;

    public Button.ButtonClickedEvent onClick;

    public Button.ButtonClickedEvent onPointerDown;
    public Button.ButtonClickedEvent onPointerUp;
    public Button.ButtonClickedEvent onPointerEnter;
    public Button.ButtonClickedEvent onPointerExit;

    public TTUIButtonStyle style;
    public TTUIButtonScale scale;

    [HideInInspector] [System.NonSerialized] public Color iconColor = Color.white;
    [HideInInspector] [System.NonSerialized] public Color iconColorOver = new Color(0f, 174f / 255f, 239f / 255f, 1f);
    [HideInInspector] [System.NonSerialized] public Color iconColorDown = Color.gray;
    [HideInInspector] [System.NonSerialized] public float iconColorTime = 0.3f;
    [HideInInspector] [System.NonSerialized] public Ease iconColorEase = Ease.InOutSine;

    [HideInInspector] [System.NonSerialized] public bool doScale = false;
    [HideInInspector] [System.NonSerialized] public Ease scaleEase = Ease.InOutSine;
    [HideInInspector] [System.NonSerialized] public float scaleFactor = 0.9f;
    [HideInInspector] [System.NonSerialized] public float scaleTime = 0.2f;
    [HideInInspector] [System.NonSerialized] public bool doShake = false;

    RectTransform rectTransform;

    //Vector3 position = Vector3.zero;
    //Vector3 rotation = Vector3.zero;

    //Vector3 localPosition = Vector3.zero;
    //Vector3 localRotation = Vector3.zero;
    //Vector3 localScale = Vector3.zero;

    bool isShaking = false;
    bool over = false;

    [HideInInspector]
    public Button button;
    [HideInInspector]
    public Image image;

    Color initialIconColor = Color.white;
    Color initialTextColor = Color.white;

    public Vector3 initScale;

    void Awake()
    {

        if (icon)
            initialIconColor = icon.color;
        if (text)
            initialTextColor = text.color;

        button = gameObject.GetComponent<Button>();
        image = gameObject.GetComponent<Image>();

        if (style)
        {
            iconColor = style.color;
            if (icon)
                icon.color = useInitColor ? initialIconColor : iconColor;
            if (text)
                text.color = useInitColor ? initialTextColor : iconColor;
            iconColorOver = style.colorOver;
            iconColorDown = style.colorDown;
            iconColorTime = style.colorTime;
            iconColorEase = style.colorEase;
        }

        if (scale)
        {
            doScale = scale.doScale;
            scaleEase = scale.scaleEase;
            scaleFactor = scale.scaleFactor;
            scaleTime = scale.scaleTime;
            doShake = scale.doShake;
        }

        rectTransform = transform as RectTransform;

        initScale = rectTransform.localScale;

        button = GetComponent<Button>();

        button.onClick.AddListener(() =>
        {
            onClick?.Invoke();
        });
        //position = rectTransform.position;
        //rotation = rectTransform.eulerAngles;

        //localPosition = rectTransform.localPosition;
        //localRotation = rectTransform.localEulerAngles;
        //localScale = rectTransform.localScale;

        buttonStates = GetComponent<TTUIButtonStates>();
        if (buttonStates)
            buttonStates.SetState(buttonStates.state, true);
    }

    public void SetState(int aState, bool changeStyle = true)
    {
        if (buttonStates)
            buttonStates.SetState(aState, changeStyle);
    }

    public void SetState(string aState, bool changeStyle = true)
    {
        if (buttonStates)
            buttonStates.SetState(aState, changeStyle);
    }

    public int GetState()
    {
        if (buttonStates)
        {
            return buttonStates.state;
        }
        else
        {
            return 0;
        }
    }

    public string GetStateName()
    {
        if (buttonStates)
        {
            return buttonStates.buttonStates[buttonStates.state].name;
        }
        else
        {
            return "";
        }
    }

    public void InvokeClick()
    {
        button.onClick.Invoke();
    }

    void OnEnable()
    {
        rectTransform.localScale = initScale;
        if (icon) icon.color = initialIconColor;
        if (text) text.color = initialTextColor;
        if (buttonStates) buttonStates.SetState(buttonStates.state, true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable)
            return;

        onPointerEnter?.Invoke();

        over = true;
        if (buttonStates && buttonStates.buttonState)
        {
            iconColor = buttonStates.buttonState.iconStyle.color;
            iconColorOver = buttonStates.buttonState.iconStyle.colorOver;
            iconColorTime = buttonStates.buttonState.iconStyle.colorTime;
            iconColorEase = buttonStates.buttonState.iconStyle.colorEase;
        }

        if (icon)
            icon.DOColor(iconColorOver, iconColorTime).SetEase(iconColorEase);
        if (text)
            text.DOColor(iconColorOver, iconColorTime).SetEase(iconColorEase);

        if (doShake && !isShaking)
        {
            isShaking = true;
            rectTransform.DOShakeScale(0.4f, 1, 90, 90)
                .OnComplete(() =>
                {
                    isShaking = false;
                });
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable)
            return;

        onPointerExit?.Invoke();

        over = false;
        if (buttonStates && buttonStates.buttonState)
        {
            iconColor = buttonStates.buttonState.iconStyle.color;
            iconColorOver = buttonStates.buttonState.iconStyle.colorOver;
            iconColorTime = buttonStates.buttonState.iconStyle.colorTime;
            iconColorEase = buttonStates.buttonState.iconStyle.colorEase;
        }

        if (icon)
            icon.DOColor(useInitColor ? initialIconColor : iconColor, iconColorTime).SetEase(iconColorEase);
        if (text)
            text.DOColor(useInitColor ? initialTextColor : iconColor, iconColorTime).SetEase(iconColorEase);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable)
            return;

        onPointerDown?.Invoke();

        if (doScale)
            rectTransform.DOScale(scaleFactor * initScale, scaleTime / 2f).SetEase(scaleEase);

        if (icon)
            icon.DOColor(iconColorDown, iconColorTime).SetEase(iconColorEase);
        if (text)
            text.DOColor(iconColorDown, iconColorTime).SetEase(iconColorEase);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable)
            return;

        onPointerUp?.Invoke();

        if (doScale)
            rectTransform.DOScale(initScale, scaleTime / 2f).SetEase(scaleEase);

        if (icon)
            icon.DOColor(over ? iconColorOver : (useInitColor ? initialIconColor : iconColor), iconColorTime).SetEase(iconColorEase);
        if (text)
            text.DOColor(over ? iconColorOver : (useInitColor ? initialIconColor : iconColor), iconColorTime).SetEase(iconColorEase);
    }
}
