using DG.Tweening;
using ForieroEngine.Purchasing;
using UnityEngine;
using UnityEngine.UI;

public class InAppMessage : MonoBehaviour
{
    public RectTransform messagePanel;

    public Text messageText;

    public Text buttonText;

    public Vector2 inVector;
    public Vector2 outVector;
    public float easeTime;
    public Ease ease;

    public void OnButtonClick()
    {
        DestroyInternal(0);
    }

    public static GameObject PREFAB_InApp_Message;

    public static GameObject inAppMessage = null;

    public static InAppMessage singleton = null;

    Tween tween;

    public static void Create()
    {
        Debug.Log("InApp Message Wall");
        if (inAppMessage)
        {
            Debug.LogError("InApp Message Wall already exists!");
            return;
        }

        if (PREFAB_InApp_Message == null)
        {
            PREFAB_InApp_Message = Resources.Load<GameObject>("PREFAB_InApp_Message_Wall");
        }

        if (PREFAB_InApp_Message == null)
        {
            Debug.LogError("InApp Message Wall not found!");
            return;
        }

        inAppMessage = Instantiate(PREFAB_InApp_Message, Vector3.zero, Quaternion.identity) as GameObject;
    }

    public void DestroyInternal(float delay)
    {
        Store.CleanProductHooks("");
        Store.processing = false;
        tween = messagePanel.DOAnchorPos(outVector, easeTime).SetDelay(delay).SetEase(ease).OnComplete(() =>
        {
            Destroy(this.gameObject);
        });
    }

    public static void Destroy(float delay)
    {
        if (singleton) singleton.DestroyInternal(delay);
    }

    public static void SetMessageText(string text)
    {
        if (singleton) singleton.messageText.text = text;
    }

    public static void SetButtonText(string text)
    {
        if (singleton) singleton.buttonText.text = text;
    }

    void Awake()
    {
        singleton = this;
        messagePanel.anchoredPosition = outVector;
        tween = messagePanel.DOAnchorPos(inVector, easeTime).SetEase(ease);
    }

    void OnDestroy()
    {
        Store.CleanProductHooks("");
        Store.processing = false;
        singleton = null;
        tween?.Kill();
    }
}
