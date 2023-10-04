using System;
using System.Collections;
using ForieroEngine;
using ForieroEngine.Extensions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using PlayerPrefs = ForieroEngine.PlayerPrefs;

public class NewsLetter : MonoBehaviour
{

    public static GameObject newsLetter = null;

    public System.Action<bool> finished;
    public RectTransform background;
    public TTUIText newsLetterText;
    public InputField emailInputField;
    public Button signUpButton;

    public static bool visible = false;

    void OnDestroy()
    {
        visible = false;
        newsLetter = null;
        Lang.OnLanguageChange -= Lang_OnLanguageChange;
    }

    void Awake()
    {
        visible = true;
        Lang.OnLanguageChange += Lang_OnLanguageChange;
    }

    void Start()
    {
        newsLetterText.SetText(Lang.GetText("Foriero_Foriero", "NEWSLETTER_SIGNUP", "Signup for newsletters!"));
    }

    void Lang_OnLanguageChange()
    {
        newsLetterText.SetText(Lang.GetText("Foriero_Foriero", "NEWSLETTER_SIGNUP", "Signup for newsletters!"));
        TTUIText.RefreshAll();
    }

    public static int ShowAttemptCounts(string id)
    {
        return PlayerPrefs.GetInt(id, 0);
    }

    public static void Show(StoreSettings settings, string id, int showOnAttempt = 0, int allowedClose = 2)
    {
        int attempts = PlayerPrefs.GetInt("NEWSLETTER_" + id, 0);
        PlayerPrefs.SetInt("NEWSLETTER_" + id, ++attempts);
        if (attempts < showOnAttempt)
        {
            Debug.Log("Newsletter will show on attempt : " + showOnAttempt.ToString());
            return;
        }

        if (PlayerPrefs.GetInt("NEWSLETTER_SIGNED", 0) == 1)
        {
            Debug.Log("App already signed for newsletter!");
            return;
        }

        int closeCount = PlayerPrefs.GetInt("NEWSLETTER_CLOSE_COUNT", 0);
        if (closeCount >= allowedClose)
        {
            Debug.Log("Max allowed newsletter close : " + allowedClose.ToString());
            Debug.Log("User closed newwsletter " + closeCount.ToString() + " times.");
            return;
        }

        Debug.Log("NewsLetter : Show");
        if (newsLetter)
        {
            Debug.LogError("NewsLetter already exists!");
            return;
        }

        newsLetter = Resources.Load<GameObject>("PREFAB_NewsLetter");
        if (newsLetter == null)
        {
            Debug.LogError("Newsletter prefab not found!!!");
            return;
        }
        newsLetter = Instantiate(newsLetter, Vector3.zero, Quaternion.identity) as GameObject;
    }

    public static bool SignedUp()
    {
        return PlayerPrefs.GetInt("NEWSLETTER_SIGNEDUP", 0) == 1;
    }

    public void OnEmailInputFieldChanged()
    {
        signUpButton.interactable = System.Text.RegularExpressions.Regex.IsMatch(emailInputField.text, Foriero.emailPattern);
    }

    public void OnNewsLetterSignClick()
    {
        StartCoroutine(SignUp());
        background.gameObject.SetActive(false);
    }

    IEnumerator SignUp(Action finished = null, Action onError = null)
    {
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("email", emailInputField.text);

        UnityWebRequest www = UnityWebRequest.Post("http://www.foriero.com/unity/newsletter.php", wwwForm);

        yield return www.SendWebRequest();

        if (www.result.HasError())
        {
            Debug.Log(www.error);
            onError?.Invoke();
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            finished?.Invoke();
        }

        PlayerPrefs.SetInt("NEWSLETTER_SIGNEDUP", 1);

        Destroy(this.gameObject);
    }

    public void OnCloseClick()
    {
        int closeCount = PlayerPrefs.GetInt("NEWSLETTER_CLOSE_COUNT", 0);
        closeCount++;
        PlayerPrefs.SetInt("NEWSLETTER_CLOSE_COUNT", closeCount);
        Destroy(this.gameObject);
    }
}
