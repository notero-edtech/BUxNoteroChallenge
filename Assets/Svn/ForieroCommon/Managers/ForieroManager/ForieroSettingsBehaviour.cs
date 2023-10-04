using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ForieroSettingsBehaviour : MonoBehaviour {

    ForieroSettings settings => ForieroSettings.instance;
    ForieroSettings.IForieroHand iForieroHand;
    AsyncOperationHandle<IResourceLocator> addressablesHandle;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    IEnumerator Start()
    {
        if (settings.addressables)
        {
            addressablesHandle = Addressables.InitializeAsync();
            yield return addressablesHandle;
        }

        InstantiateVoiceOverDownloader();
    }

    void Update()
    {
        #if ENABLE_INPUT_SYSTEM
        if(Mouse.current != null && Mouse.current.rightButton.wasReleasedThisFrame)
        #else 
        if (Input.GetMouseButtonUp(1))
        #endif
        {
#if !UNITY_EDITOR
            if (ForieroSettings.instance.handCursor.onlyInEditor) return;
#endif
            if (iForieroHand == null && ForieroSettings.instance.handCursor.handCursor)
            {
                InstantiateHand();
            }

            StartCoroutine(ShowHide());
        }
    }

    IEnumerator ShowHide()
    {
        if (iForieroHand == null) yield break;
        iForieroHand.DoToggle();
    }

    void InstantiateHand()
    {
        var go = Instantiate(ForieroSettings.instance.handCursor.handCursor);
        go.GetComponent<Canvas>().sortingOrder = ForieroSettings.instance.handCursor.sortingOrder;
        if (ForieroSettings.instance.handCursor.dontDestroyOnLoad) DontDestroyOnLoad(go);
        iForieroHand = go.GetComponentInChildren<ForieroSettings.IForieroHand>();
    }

    void InstantiateVoiceOverDownloader()
    {
        if (ForieroSettings.instance.voiceOverDownloader.downloader)
        {
            var go = Instantiate(ForieroSettings.instance.voiceOverDownloader.downloader);
            go.GetComponent<Canvas>().sortingOrder = ForieroSettings.instance.voiceOverDownloader.sortingOrder;                        
        }
    }
}
