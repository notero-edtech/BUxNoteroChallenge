using System;
using System.Collections;
using ForieroEngine.Extensions;
using ForieroEngine.Purchasing;
using ForieroEngine.Threading.Unity;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using PlayerPrefs = ForieroEngine.PlayerPrefs;

public class Referrals : MonoBehaviour
{
#if UNITY_EDITOR
    [Button] void GenerateReferralId() => referralId = Guid.NewGuid().ToString();
#endif

    [ReadOnly] public string referralId = Guid.NewGuid().ToString();
    public string inAppId = "";

    [SerializeField, Range(0, 100)] int discount = 80;
    [SerializeField, Range(0, 100)] int referralsNeeded = 5;
           
    public TextMeshProUGUI friendsText;
    public TextMeshProUGUI discountText;
    public TextMeshProUGUI explanationText;
    public Button copyMyIdButton;
    public Button setFriendIdButton;
    public Button restoreButton;
    public Button purchaseButton;
    public Button closeButton;
    public TMP_InputField myIdInput;
    public TMP_InputField friendIdInput;
            
    IEnumerator Start()
    {
        explanationText.text = explanationText.text.Replace("X", referralsNeeded.ToString());
        myIdInput.text = SystemInfo.deviceUniqueIdentifier;
        friendsText.text = $"Friends: 0/{referralsNeeded}";
        discountText.text = $"Discount: {discount}%";
        yield return Refresh();
        AddListeners();        
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }

    void AddListeners()
    {
        copyMyIdButton.onClick.AddListener(OnCopyMyIdButton);
        setFriendIdButton.onClick.AddListener(OnSetFriendIdClick);
        purchaseButton.onClick.AddListener(OnPurchaseClick);
        restoreButton.onClick.AddListener(OnRestoreClick);
        closeButton.onClick.AddListener(OnCloseClick);
        myIdInput.onValueChanged.AddListener(OnMyIDChange);
    }

    void RemoveListeners()
    {
        copyMyIdButton.onClick.RemoveListener(OnCopyMyIdButton);
        setFriendIdButton.onClick.RemoveListener(OnSetFriendIdClick);
        purchaseButton.onClick.RemoveListener(OnPurchaseClick);
        restoreButton.onClick.RemoveListener(OnRestoreClick);
        closeButton.onClick.RemoveListener(OnCloseClick);
        myIdInput.onValueChanged.RemoveListener(OnMyIDChange);
    }

    public void OnMyIDChange(string s)
    {
        myIdInput.text = SystemInfo.deviceUniqueIdentifier;
    }

    void OnPurchaseClick()
    {
        ParentalLock.Show(4, (success) =>
        {
            if (success)
            {
                Store.PurchaseProduct(inAppId, PurchaseSuccess, PurchaseFailed);
            }
        });
    }

    void PurchaseSuccess(string id, string receipt)
    {
        if(id.Equals(inAppId, StringComparison.Ordinal)) this.gameObject.SetActive(false);
    }

    void PurchaseFailed(string id, string reason) => Debug.LogError($"Purchase Failed ({id}): {reason}");
        
    void OnRestoreClick()
    {
        ParentalLock.Show(4, (success) =>
        {
            if (success)
            {
                Store.RestoreProduct(inAppId, PurchaseSuccess);                
            }           
        });        
    }


    void OnCopyMyIdButton()
    {
        GUIUtility.systemCopyBuffer =
            "Download : " + Store.GetStoreLink() + System.Environment.NewLine +
            "Friend Id : " + SystemInfo.deviceUniqueIdentifier + System.Environment.NewLine;            

        UIStatusMessage.ShowMessage("MyID copied. Please share it with your friends.");
    }

    void OnSetFriendIdClick()
    {
        if (string.IsNullOrEmpty(friendIdInput.text)) return;

        friendIdInput.interactable = false;
        setFriendIdButton.interactable = false;

        StartCoroutine(SetFriendId());
    }

    IEnumerator SetFriendId()
    {
        yield return Server.SetReferralFriendIdEnumerator(referralId, friendIdInput.text);
        yield return Refresh();
    }

    void OnCloseClick() => this.gameObject.SetActive(false);

    IEnumerator Refresh()
    {
        yield return Server.GetReferralFriendIdEnumerator(referralId, (friendId) =>
        {
            if (string.IsNullOrEmpty(friendId))
            {
                friendIdInput.interactable = true;
                setFriendIdButton.interactable = true;
            }
            else
            {
                friendIdInput.text = friendId;
                friendIdInput.interactable = false;
                setFriendIdButton.interactable = false;
            }
        });

        yield return Server.GetReferralCountEnumerator(referralId, (c) =>
        {
            friendsText.text = $"Friends: {c}/{referralsNeeded}";
            if (c >= referralsNeeded) purchaseButton.interactable = true;
        });
    }

    public static class Server
    {
        static readonly string baseUrl = "https://backend.foriero.com/unity/referrals/referrals.php";
        public static void SetReferralFriendId(string referralId, string friendId) => MainThreadDispatcher.Instance.StartCoroutine(SetReferralFriendIdEnumerator(referralId, friendId));
        public static IEnumerator SetReferralFriendIdEnumerator(string referralId, string friendId)
        {
            WWWForm form = new WWWForm();
            form.AddField("cmd", "set");
            form.AddField("referral_id", referralId.ToString());
            form.AddField("my_id", SystemInfo.deviceUniqueIdentifier);
            form.AddField("friend_id", friendId.ToString());

            using (UnityWebRequest www = UnityWebRequest.Post(baseUrl, form))
            {            
                yield return www.SendWebRequest();

                if (www.result.HasError())
                {
                    Debug.Log(www.error);
                }

                if(www.downloadHandler.text.Contains("TRUE")) PlayerPrefs.SetString(referralId, friendId);
            }            
        }

        public static void GetReferralFriendId(string referralId, Action<string> onFriendId) => MainThreadDispatcher.Instance.StartCoroutine(GetReferralFriendIdEnumerator(referralId, onFriendId));
        public static IEnumerator GetReferralFriendIdEnumerator(string referralId, Action<string> onFriendId)
        {
            WWWForm form = new WWWForm();
            form.AddField("cmd", "get");
            form.AddField("referral_id", referralId);
            form.AddField("my_id", SystemInfo.deviceUniqueIdentifier);

            using (UnityWebRequest www = UnityWebRequest.Post(baseUrl, form))
            {
                yield return www.SendWebRequest();

                if (www.result.HasError())
                {
                    Debug.Log(www.error);
                }
                else
                {
                    onFriendId?.Invoke(www.downloadHandler.text);
                }
            }
        }

        public static void GetReferralCount(string referralId, Action<int> onCount) => MainThreadDispatcher.Instance.StartCoroutine(GetReferralCountEnumerator(referralId, onCount));
        public static IEnumerator GetReferralCountEnumerator(string referralId, Action<int> onCount)
        {
            WWWForm form = new WWWForm();
            form.AddField("cmd", "count");
            form.AddField("referral_id", referralId);
            form.AddField("my_id", SystemInfo.deviceUniqueIdentifier);

            using (UnityWebRequest www = UnityWebRequest.Post(baseUrl, form))
            {
                yield return www.SendWebRequest();

                if (www.result.HasError())
                {
                    Debug.Log(www.error);
                }
                else
                {
                    int.TryParse(www.downloadHandler.text, out var count);
                    onCount?.Invoke(count);
                }
            }
        }
    }
}
