using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class TTUIDataPrivacyButton : Button
{
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void OpenNewWindow(string url);
#endif

    bool urlOpened = false;

    TTUIDataPrivacyButton()
    {
        onClick.AddListener(OpenDataPrivacyUrl);
    }

    void OnFailure(string reason)
    {
        interactable = true;
        Debug.LogWarningFormat("Failed to get data privacy url: {0}", reason);
    }

    void OpenUrl(string url)
    {
        interactable = true;
        urlOpened = true;

#if UNITY_WEBGL && !UNITY_EDITOR
        OpenNewWindow(url);
#else
        Application.OpenURL(url);
#endif
    }

    void OpenDataPrivacyUrl()
    {
        ParentalLock.Show(4, (passed) =>
        {
            if (passed)
            {
                interactable = false;
                DataPrivacy.FetchPrivacyUrl(OpenUrl, OnFailure);
            }
        });
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && urlOpened)
        {
            urlOpened = false;
            // Immediately refresh the remote config so new privacy settings can be enabled
            // as soon as possible if they have changed.
            RemoteSettings.ForceUpdate();
        }
    }
}