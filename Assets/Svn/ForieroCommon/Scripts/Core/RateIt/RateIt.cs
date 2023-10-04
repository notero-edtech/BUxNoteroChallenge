using System;
using ForieroEngine.Purchasing;
using UnityEngine;

using Debug = UnityEngine.Debug;
using PlayerPrefs = ForieroEngine.PlayerPrefs;

public class RateIt : MonoBehaviour
{
    public static GameObject rateIt = null;

    public Action<bool> finished;
    public TTUIText rateItText;
    public TTUITextTMP rateItTextTMP;

    void OnDestroy()
    {
        rateIt = null;
        Lang.OnLanguageChange -= Lang_OnLanguageChange;
    }

    void Awake()
    {
        Lang.OnLanguageChange += Lang_OnLanguageChange;
    }

    void Start()
    {
        rateItText?.SetText(GetRateItText());
        rateItTextTMP?.SetText(GetRateItText());
    }

    string GetRateItText()
    {
        return Lang.GetText(
            "Foriero", "rate_it_ratings", "Your ratings help us add more new features.") +
            System.Environment.NewLine +
            Lang.GetText("Foriero", "rate_it_thank_you", "Thank you for your support! \n (We read every review)."
        );
    }

    void Lang_OnLanguageChange()
    {
        rateItText?.SetText(GetRateItText());
        TTUIText.RefreshAll();
        rateItTextTMP?.SetText(GetRateItText());
        TTUITextTMP.RefreshAll();
    }

    public static int ShowAttemptCounts(string id)
    {
        return PlayerPrefs.GetInt(id, 0);
    }

    public static void Show(StoreSettings settings, string id, int showOnAttempt = 0, int allowedClose = 2)
    {
        int attempts = PlayerPrefs.GetInt("RATEIT_" + id, 0);
        PlayerPrefs.SetInt("RATEIT_" + id, ++attempts);
        if (attempts < showOnAttempt)
        {
            Debug.Log("Rate It will show on attempt : " + showOnAttempt.ToString());
            return;
        }

        if (PlayerPrefs.GetInt("RATED", 0) == 1)
        {
            Debug.Log("App already rated!");
            return;
        }

        int closeCount = PlayerPrefs.GetInt("RATE_CLOSE_COUNT", 0);
        if (closeCount >= allowedClose)
        {
            Debug.Log("Max allowed Rate Now close : " + allowedClose.ToString());
            Debug.Log("User closed Rate Now " + closeCount.ToString() + " times.");
            return;
        }

        Debug.Log("Rate it : Show");
        if (rateIt)
        {
            Debug.LogError("Rate it already exists!");
            return;
        }

        PlayerPrefs.SetInt("RATEIT_" + id, 0);

#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        NativeReviewRequest.RequestReview();
#elif UNITY_IOS && !UNITY_EDITOR
        NativeReviewRequest.RequestReview();
#elif NETFX_CORE || (ENABLE_IL2CPP && UNITY_WSA_10_0)
        NativeReviewRequest.RequestReview();
#else
        if (ForieroSettings.instance.PREFAB_Rate_It == null)
        {
            Debug.LogError("RateIt prefab not found!!!");
            return;
        }

        rateIt = Instantiate(ForieroSettings.instance.PREFAB_Rate_It, Vector3.zero, Quaternion.identity) as GameObject;
#endif

    }

    public void OnRateNowClick()
    {
        ParentalLock.Show(4, (ok) =>
        {
            if (ok)
            {
                if (Store.OpenStore())
                {
                    PlayerPrefs.SetInt("RATED", 1);
                }

                Destroy(this.gameObject);
            }
        });
    }

    public void OnCloseClick()
    {
        int closeCount = PlayerPrefs.GetInt("RATE_CLOSE_COUNT", 0);
        closeCount++;
        PlayerPrefs.SetInt("RATE_CLOSE_COUNT", closeCount);
        Destroy(this.gameObject);
    }
}
