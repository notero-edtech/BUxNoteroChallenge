using UnityEngine;
using System.Collections.Generic;
using ForieroEngine;
using ForieroEngine.Purchasing;
using ForieroEngine.Settings;

#if FORIERO_INAPP
using UnityEngine.Purchasing;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public class StoreSettings : Settings<StoreSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/Store", false, -1000)] public static void StoreSettingsMenu() => Select();  
#endif

    [System.Serializable]
    public class StoreOverride
    {
        public bool overrideId = false;
        public bool prependBundleId = false;
        public string id;
    }

    [System.Serializable]
    public class PurchaseItem
    {
        public Store.PurchaseEnum purchaseType = Store.PurchaseEnum.NonConsumable;
        public bool prependBundleId = true;
        public string id = "";

        public StoreOverride osx;
        public StoreOverride ios;
        public StoreOverride samsung;
        public StoreOverride amazon;        
        public StoreOverride google;
        public StoreOverride wsa;
        public StoreOverride udp;

        public string name;
        public string description;
        public bool foldout;
    }

    public bool initialize = true;
    public bool useCatalogProvider = false;
    public string projectWWW = "";
    public string appName;
    public string appWWW = "";
    public string companyName = "Foriero";
    public float version;
    public float build;

    [HideInInspector]
    public string _appStore = "GooglePlay";

#if FORIERO_INAPP
    public AppStore appStore
    {
        get { return (AppStore)System.Enum.Parse(typeof(AppStore), _appStore); }
        set { _appStore = value.ToString(); }
    }
#endif
    
    PurchaseItem Find(string id)
    {
        foreach (PurchaseItem item in purchaseItems)
        {
            if (item.id == id)
                return item;
        }
        return null;
    }

    public string GetStoreInAppId(string inAppId, Store.StoreEnum store)
    {
        PurchaseItem item = Find(inAppId);

        if (item == null)
        {
            Debug.LogError("InApp item not found! : " + inAppId);
            return string.Empty;
        }
        else
        {
            return GetStoreInAppId(item, store);
        }
    }

    private string Dot(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return "";
        }

        return s.EndsWith(".") ? "" : ".";
    }

    public string GetStoreInAppId(PurchaseItem item, Store.StoreEnum store)
    {
        string result = "";

        switch (store)
        {
            case Store.StoreEnum.amazon:
                if (item.amazon.overrideId)
                {
                    result = !item.amazon.prependBundleId ? item.amazon.id : amazon.bundleId + Dot(amazon.bundleId) + item.amazon.id;
                }
                else
                {
                    result = !item.prependBundleId ? item.id : amazon.bundleId + Dot(amazon.bundleId) + item.id;
                }
                break;
            case Store.StoreEnum.samsung:
                if (item.samsung.overrideId)
                {
                    result = !item.samsung.prependBundleId ? item.samsung.id : samsung.bundleId + Dot(samsung.bundleId) + item.samsung.id;
                }
                else
                {
                    result = !item.prependBundleId ? item.id : samsung.bundleId + Dot(samsung.bundleId) + item.id;
                }
                break;
            case Store.StoreEnum.google:
                if (item.google.overrideId)
                {
                    result = !item.google.prependBundleId ? item.google.id : google.bundleId + Dot(google.bundleId) + item.google.id;
                }
                else
                {
                    result = !item.prependBundleId ? item.id : google.bundleId + Dot(google.bundleId) + item.id;
                }
                break;
            case Store.StoreEnum.udp:
                if (item.udp.overrideId)
                {
                    result = !item.udp.prependBundleId ? item.udp.id : udp.bundleId + Dot(udp.bundleId) + item.udp.id;
                }
                else
                {
                    result = !item.prependBundleId ? item.id : udp.bundleId + Dot(udp.bundleId) + item.id;
                }
                break;
            case Store.StoreEnum.ios:
                if (item.ios.overrideId)
                {
                    result = !item.ios.prependBundleId ? item.ios.id : ios.bundleId + Dot(ios.bundleId) + item.ios.id;
                }
                else
                {
                    result = !item.prependBundleId ? item.id : ios.bundleId + Dot(ios.bundleId) + item.id;
                }
                break;
            case Store.StoreEnum.osx:
                if (item.osx.overrideId)
                {
                    result = !item.osx.prependBundleId ? item.osx.id : osx.bundleId + Dot(osx.bundleId) + item.osx.id;
                }
                else
                {
                    result = !item.prependBundleId ? item.id : osx.bundleId + Dot(osx.bundleId) + item.id;
                }
                break;
            case Store.StoreEnum.wsa:
                if (item.wsa.overrideId)
                {
                    result = !item.wsa.prependBundleId ? item.wsa.id : wsa.packageIdentityName + Dot(wsa.packageIdentityName) + item.wsa.id;
                }
                else
                {
                    result = !item.prependBundleId ? item.id : wsa.packageIdentityName + Dot(wsa.packageIdentityName) + item.id;
                }
                break;
        }

        if (Foriero.debug)
        {
            Debug.Log(store.ToString().ToUpper() + " : " + result);
        }

        return result;
    }

    public string GetBundleId()
    {
#if UNITY_IOS
		return ios.bundleId;
#elif UNITY_ANDROID
        switch (instance._appStore)
        {
            case "GooglePlay": return google.bundleId;
            case "AmazonAppStore": return amazon.bundleId;
            case "SamsungApps": return samsung.bundleId;
            default:
                Debug.Log("Not supported Android platform!");
                return "";
        }
#elif UNITY_WSA
		return wsa.storeId;
#elif UNITY_STANDALONE_OSX
        return osx.bundleId;
#else
        Debug.Log("Not supported platform!");
        return "";
#endif
    }

    public string GetBundleId(Store.StoreEnum store)
    {
        string result = "";

        switch (store)
        {
            case Store.StoreEnum.amazon:
                result = amazon.bundleId;
                break;
            case Store.StoreEnum.samsung:
                result = samsung.bundleId;
                break;
            case Store.StoreEnum.google:
                result = google.bundleId;
                break;
            case Store.StoreEnum.ios:
                result = ios.bundleId;
                break;
            case Store.StoreEnum.osx:
                result = osx.bundleId;
                break;
            case Store.StoreEnum.wsa:
                result = wsa.storeId;
                break;
            case Store.StoreEnum.udp:
                result = udp.bundleId;
                break;
        }

        return result;
    }

    [System.Serializable]
    public class OSXVars
    {
        // OSX //
        [HideInInspector]
        public bool foldout = false;
        [Tooltip("com.foriero.osx.[appname]")]
        public string bundleId = "com.foriero.osx";
        [Tooltip("Manualy entered SKU. Go to itunes store.")]
        public string SKU;
        [Tooltip("Automatically generated app id from apple. Go to itunes store.")]
        public string appleId;
    }

    [System.Serializable]
    public class AMAZONVars
    {
        // GOOGLE //
        [HideInInspector]
        public bool foldout = false;
        [Tooltip("com.foriero.amazon.[appname]")]
        public string bundleId = "com.foriero.amazon";
        [TextArea]
        public string publicKey;
    }

    [System.Serializable]
    public class SAMSUNGVars
    {
        // GOOGLE //
        [HideInInspector]
        public bool foldout = false;
        [Tooltip("com.foriero.samsung.[appname]")]
        public string bundleId = "com.foriero.samsung";
        [TextArea]
        public string publicKey;
    }

    [System.Serializable]
    public class GOOGLEVars
    {
        // GOOGLE //
        [HideInInspector]
        public bool foldout = false;
        [Tooltip("com.foriero.google.[appname]")]
        public string bundleId = "com.foriero.google";
        [TextArea]
        public string publicKey;
    }

    [System.Serializable]
    public class UDPVars
    {
        // GOOGLE //
        [HideInInspector]
        public bool foldout = false;
        [Tooltip("com.foriero.udp.[appname]")]
        public string bundleId = "com.foriero.udp";
        [TextArea]
        public string cliendId;
    }

    [System.Serializable]
    public class IOSVars
    {
        // IOS //
        [HideInInspector]
        public bool foldout = false;
        [Tooltip("com.foriero.ios.[appname]")]
        public string bundleId = "com.foriero.ios";
        [Tooltip("Manualy entered SKU. Go to itunes store.")]
        public string SKU;
        [Tooltip("Automatically generated app id from apple. Go to itunes store.")]
        public string appleId;
    }

    [System.Serializable]
    public class WSAVars
    {
        // WSA //
        [HideInInspector]
        public bool foldout = false;
        [Tooltip("Mixed string")]
        public string storeId = "";
        [Tooltip("Package Identity Name")]
        public string packageIdentityName = "";

        [Tooltip("Package Family Name")]
        public string packageFamilyName = "";
    }

    public OSXVars osx;
    public IOSVars ios;
    public AMAZONVars amazon;
    public SAMSUNGVars samsung;
    public GOOGLEVars google;
    public UDPVars udp;
    public WSAVars wsa;

    [HideInInspector]
    public string _fakeStoreUIMode = "Default";

#if FORIERO_INAPP
    public FakeStoreUIMode fakeStoreUIMode
    {
        get{ return (FakeStoreUIMode)System.Enum.Parse(typeof(FakeStoreUIMode), _fakeStoreUIMode); }
        set{ _fakeStoreUIMode = value.ToString(); }
    }
#endif

    public List<PurchaseItem> purchaseItems = new List<PurchaseItem>();
    
#if UNITY_EDITOR
   public override void Apply()
    {
        if (this == instance) return;
        base.Apply();
    }
#endif
}
