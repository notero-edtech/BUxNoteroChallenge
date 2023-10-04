using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using ForieroEngine.Extensions;
using ForieroEngine.Threading.Unity;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

using UnityEngine.UI;
using PlayerPrefs = UnityEngine.PlayerPrefs;

using UnityEngine.AddressableAssets;

public class VoiceOverDownloader : MonoBehaviour
{
    public bool skipIfSelected = true;

    public Text loadingText;
    public Text loadingErrorsText;
    public Image loadingBackground;
    public Image loadingForeground;

    public Button continueButton;

    public Button languageButton;

    public TTUILanguageSelector languageSelector;

    void Start()
    {

        languageSelector.OnShowAvailableLanguages.AddListener(() =>
        {
            continueButton.gameObject.SetActive(false);
        });

        languageSelector.OnHideAvailableLanguages.AddListener(() =>
        {
            continueButton.gameObject.SetActive(true);
        });

        if (skipIfSelected)
        {
            if (PlayerPrefs.HasKey("DOWNLOAD_LANGUAGE"))
            {
                loadingBackground.gameObject.SetActive(true);
                languageButton.interactable = false;
                loadingText.text = "";
                continueButton.gameObject.SetActive(false);
            }
                    
            DOVirtual.DelayedCall(0.5f, () =>
            {
                if (PlayerPrefs.HasKey("DOWNLOAD_LANGUAGE"))
                {
                    OnContinueClick();
                }
            });
        }

        Lang.OnLanguageChange += () =>
        {
            TTUIText.RefreshAll();
        };
    }
        
    DownloadLanguageClass downloadLanguage;
    float percentage = 0;

    void Update()
    {
        if (downloadLanguage == null) return;

        loadingForeground.rectTransform.SetSize(new Vector2(loadingBackground.rectTransform.GetSize().x * percentage, loadingForeground.rectTransform.GetSize().y));
        loadingText.text = $"{(int)(percentage * 100)}%";              
    }

    public void OnContinueClick()
    {
        PlayerPrefs.SetInt("DOWNLOAD_LANGUAGE", 1);
        PlayerPrefs.Save();

        if (Lang.debug)
        {
            Debug.Log("OnContinueClick");
        }

        loadingBackground.gameObject.SetActive(true);
        languageButton.interactable = false;
        loadingText.text = "";
        continueButton.gameObject.SetActive(false);
        //combo box selection//
        downloadLanguage = new DownloadLanguageClass(Lang.selectedLanguage);

        downloadLanguage.Download(
            (p)=>percentage = p,
            (e)=> {
                loadingErrorsText.gameObject.SetActive(true);                
                loadingErrorsText.text = e;
            },
            () =>
            {
                downloadLanguage = null;
                SceneSettings.LoadSceneByCommand("NEXT");
            });
    }

    public class DownloadLanguageClass
    {                      
        public Lang.LanguageCode langCode;
                
        public DownloadLanguageClass(Lang.LanguageCode langCode)
        {
            this.langCode = langCode;            
        }

        public void Download(Action<float> percentage, Action<string> error, Action finished, string[] dictionaries = null, string[] actors = null)
        {            
            MainThreadDispatcher.Instance.StartCoroutine(DownloadLanguage(percentage, error, finished));
        }
               
        IEnumerator DownloadLanguage(Action<float> percentage, Action<string> error, Action finished)
        {            
            if (Lang.debug) Debug.Log($"Lang | DOWNLOADING LANGUAGE : {langCode}");
        
            var keys = LangSettings.instance.dictionaries
                .Where(d => d.stored == LangSettings.LangDictionary.Storage.OnServer)
                .Select(d => $"{langCode}.{LangSettings.instance.guid}.{d.aliasName}")
                .ToList() ;

            var locationsAsync = Addressables.LoadResourceLocationsAsync(keys.Cast<object>().ToList(), Addressables.MergeMode.Union);
            yield return locationsAsync;

            if(locationsAsync.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("Lang | Loading Resources Keys Failed!");
                yield break;
            }

            var locations = locationsAsync.Result;
            
            AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(locations);
            yield return getDownloadSize;
            Debug.Log($"Lang | Download size : {getDownloadSize.Result / 1000000f} MB");

            if (getDownloadSize.Result > 0)
            {
                var dependeciesAsync = Addressables.DownloadDependenciesAsync(locations);
                while (!dependeciesAsync.IsDone)
                {
                    percentage?.Invoke(dependeciesAsync.PercentComplete);
                    yield return null;
                }

                switch (dependeciesAsync.Status)
                {
                    case AsyncOperationStatus.None:
                    case AsyncOperationStatus.Succeeded:
                        Debug.Log("Lang | Download complete.");
                        percentage?.Invoke(1f);
                        break;
                    case AsyncOperationStatus.Failed:
                        error?.Invoke($"Lang | Download failed : {langCode}");
                        break;
                }                
            } else
            {
                Debug.Log("Lang | Language most likely cached from previous download.");
                percentage?.Invoke(1f);
            }

            yield return null;
            finished?.Invoke();
        }
    }
}
