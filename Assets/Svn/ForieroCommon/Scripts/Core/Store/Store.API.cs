using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if FORIERO_INAPP
using UnityEngine.Purchasing;
#endif

namespace ForieroEngine.Purchasing
{
    public static partial class Store
    {
        public static string inAppMessage = "";

        public static bool processing = false;

        public static string LocalizedPrice(string productIdentifier)
        {
#if FORIERO_INAPP
            if (Foriero.debug) { Debug.Log("STORE API : Checking purchased for " + productIdentifier); }

            foreach (Product p in products)
            {
                if (p.productIdentifier.Equals(productIdentifier))
                {
                    return storeListener.m_Controller.products.WithID(p.productIdentifier).metadata.localizedPrice.ToString(); ;
                }
            }
#endif
            return "$$";
        }

        public static bool Purchased(string productIdentifier, bool defaultValue = false)
        {
#if FORIERO_INAPP
            if (Foriero.debug) { Debug.Log("STORE API : Checking purchased for " + productIdentifier); }

            foreach (Product p in products)
            {
                if (p.productIdentifier.Equals(productIdentifier))
                {
                    return p.purchased;
                }
            }
            return false;
#else
			return defaultValue;
#endif
        }

        public static void CleanProductHooks(string productId = null)
        {
#if FORIERO_INAPP
            if (Foriero.debug) { Debug.Log("STORE API : Cleaning Product Hooks - " + productId); }

            foreach (Product p in products)
            {
                if (string.IsNullOrEmpty(productId))
                {
                    p.OnPurchased = null;
                    p.OnFailed = null;                    
                }
                else if (p.productIdentifier.Equals(productId))
                {
                    p.OnPurchased = null;
                    p.OnFailed = null;                    
                }
            }
#endif
        }

        /// <summary>
        /// Hooks the on product.
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <param name="OnPurchased">On purchased. (product id, receipt)</param>
        /// <param name="OnFailed">On failed.(product id, reason)</param>
        /// <param name="OnRestored">On restored.</param>
        public static void HookOnProduct(string productId, Action<string, string> OnPurchased, Action<string, string> OnFailed = null)
        {
#if FORIERO_INAPP
            if (Foriero.debug) { Debug.Log("STORE API : HookOnProduct - " + productId); }

            if (!initialized || !StoreListener.initialized)
            {
                if (Foriero.debug)
                {
                    Debug.LogError("Store API not initialized!" + System.Environment.NewLine + "initialized : " + initialized + System.Environment.NewLine + "StoreListener.initialized : " + StoreListener.initialized);
                }
                return;
            }

            bool exists = false;
            foreach (UnityEngine.Purchasing.Product product in storeListener.m_Controller.products.all)
            {
                if (product.definition.id.Equals(productId))
                {
                    if (Foriero.debug)
                    {
                        Debug.Log("Store API Hooking Product : " + productId);
                    }
                    foreach (Product p in products)
                    {
                        if (p.productIdentifier.Equals(productId))
                        {
                            p.OnPurchased = OnPurchased;
                            p.OnFailed = OnFailed;                            
                            exists = true;
                        }
                    }
                }
            }

            if (!exists)
            {
                if (Foriero.debug) { Debug.LogError("STORE API : Product does not exists - " + productId); }

                InAppMessage.Create();
                InAppMessage.SetMessageText("Product not found!");
                InAppMessage.SetButtonText("Close");
            }
#else
             InAppMessage.Create();
             InAppMessage.SetMessageText("InApp not initialised!");
             InAppMessage.SetButtonText("Close");
#endif
        }

        /// <summary>
        /// Purchases the product.
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <param name="OnPurchased">On purchased. (product id, receipt)</param>
        /// <param name="OnFailed">On failed.(product id, reason)</param>
        public static void PurchaseProduct(string productId, Action<string, string> OnPurchased = null, Action<string, string> OnFailed = null)
        {
#if FORIERO_INAPP
            if (processing)
            {
                if (Foriero.debug) { Debug.LogError("STORE API : Processing!!! Please wait or cancel transaction."); }
                return;
            }

            if (Foriero.debug) { Debug.Log("STORE API : PurchaseProduct - " + productId); }

            if (!initialized || !StoreListener.initialized)
            {
                if (Foriero.debug) { Debug.LogError("Store API not initialized!" + System.Environment.NewLine + "initialized : " + initialized + System.Environment.NewLine + "StoreListener.initialized : " + StoreListener.initialized); }
                return;
            }

            if (storeListener.m_Controller == null)
            {
                if (Foriero.debug) { Debug.LogError("m_Controller null"); }
                return;
            }

            bool exists = false;
            foreach (UnityEngine.Purchasing.Product product in storeListener.m_Controller.products.all)
            {
                if (product.definition.id.Equals(productId))
                {
                    if (Foriero.debug) { Debug.Log("STORE API : Purchase Product - " + productId); }

                    foreach (Product p in products)
                    {
                        if (p.productIdentifier.Equals(productId))
                        {
                            p.OnPurchased = OnPurchased;
                            p.OnFailed = OnFailed;
                            exists = true;
                        }
                    }

                    processing = true;

                    InAppMessage.Create();
                    InAppMessage.SetMessageText("Purchasing, please wait!");
                    InAppMessage.SetButtonText("Close");

                    storeListener.m_Controller.InitiatePurchase(product);
                }
            }

            if (!exists)
            {
                if (Foriero.debug) { Debug.LogError("STORE API : Product does not exists - " + productId); }

                InAppMessage.Create();
                InAppMessage.SetMessageText("Product not found!");
                InAppMessage.SetButtonText("Close");
            }
#else
             InAppMessage.Create();
             InAppMessage.SetMessageText("InApp not initialised!");
             InAppMessage.SetButtonText("Close");
#endif
        }

        public static void RestoreProduct(string productId, Action<string, string> OnPurchased)
        {
            HookOnProduct(productId, (id, receipt)=> { if (id.Equals(productId, StringComparison.Ordinal)) OnPurchased(id, receipt); }, null);
            RestoreProducts();
        }

        public static void RestoreProducts()
        {
            if (processing)
            {
                if (Foriero.debug) { Debug.LogError("STORE API : Processing!!! Please wait or cancel transaction."); }
                return;
            }

            if (Foriero.debug) { Debug.Log("STORE API : RestoreProducts"); }
            if (!initialized || !StoreListener.initialized)
            {
                if (Foriero.debug) { Debug.LogError("STORE API : Not initialized!"); }
                return;
            }

            processing = true;

            InAppMessage.Create();
            InAppMessage.SetMessageText("Restoring purchases, please wait!");
            InAppMessage.SetButtonText("Close");
#if FORIERO_INAPP
            if (!Application.isEditor)
            {
                // WSA //
                if (Application.platform == RuntimePlatform.WSAPlayerX86 || Application.platform == RuntimePlatform.WSAPlayerX64 || Application.platform == RuntimePlatform.WSAPlayerARM)
                {
                    storeListener.m_MicrosoftExtensions?.RestoreTransactions();
                    storeListener.OnTransactionsRestored(true);
                }
                // iOS, OSX, tvOS //
                else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.tvOS)
                {
                    storeListener.m_AppleExtensions?.RestoreTransactions(storeListener.OnTransactionsRestored);
                }
                // GooglePlay - is calling automatically ProcessPurchase //
                else if (Application.platform == RuntimePlatform.Android && purchaseModule.appStore == AppStore.GooglePlay)
                {
                    storeListener.m_GooglePlayStoreExtensions.RestoreTransactions(storeListener.OnTransactionsRestored);
                }
                // SamsungApps //
                // else if (Application.platform == RuntimePlatform.Android && purchaseModule.appStore == AppStore.SamsungApps)
                // {
                //     storeListener.m_SamsungExtensions?.RestoreTransactions(storeListener.OnTransactionsRestored);
                // }
                // AmazonAppStore is calling automatically ProcessPurchase //
                else if (Application.platform == RuntimePlatform.Android && purchaseModule.appStore == AppStore.AmazonAppStore)
                {
                    //storeListener.m_AmazonExtensions?.
                }
                // UDP //
                else if(Application.platform == RuntimePlatform.Android && purchaseModule.appStore == AppStore.UDP)
                {
                    //storeListener.m_UDPExtensions.
                }               
                else
                {
                    Debug.LogWarning(Application.platform.ToString() + " is not a supported platform for the Codeless IAP restore button");
                }
            }
#endif

#if FORIERO_INAPP && UNITY_EDITOR
            storeListener.OnTransactionsRestored(true);
#endif
        }

        public static bool OpenStoreInBrowser(StoreEnum store, StoreSettings s = null)
        {
            s = s ?? settings;

            var url = GetBrowserLink(store, s);

            if (string.IsNullOrEmpty(url))
            {
                Debug.Log("Open Web Store url is NULL!");
                return false;
            }
            else
            {
                Debug.Log("Open Web Store : " + url);
                Application.OpenURL(url);
                return true;
            }
        }

        public static string GetBrowserLink(StoreEnum store, StoreSettings s = null)
        {
            s = s ?? settings;

            Debug.Log("STORE API : OpenStoreInBrowser " + store.ToString());
            string url = "";
            switch (store)
            {
                case StoreEnum.amazon:
                    url = "";
                    break;
                case StoreEnum.google:
                    url = ("https://play.google.com/store/apps/details?id=" + settings.google.bundleId);
                    break;
                case StoreEnum.samsung:
                    url = "";
                    break;
                case StoreEnum.osx:
                    url = ("https://itunes.apple.com/app/pages/id" + settings.osx.appleId + "?mt=12");
                    break;
                case StoreEnum.ios:
                    url = ("https://itunes.apple.com/app/pages/id" + settings.ios.appleId + "?mt=8&uo=4");
                    break;
                case StoreEnum.wsa:
                    url = ("https://www.microsoft.com/store/apps/" + settings.wsa.storeId);
                    break;
                case StoreEnum.udp:
                    //url = ("https://www.microsoft.com/store/apps/" + settings.wsa.storeId);
                    break;
            }

            return url;
        }

        public static string GetStoreLink(StoreEnum store, StoreSettings s = null)
        {
            s = s ?? settings;

            string url = "";
            switch (store)
            {
                case StoreEnum.amazon:
#if UNITY_ANDROID
                    url = "";
#else
                    url = "";
#endif
                    break;
                case StoreEnum.google:
#if UNITY_ANDROID
                    url = ("market://details?id=" + s.google.bundleId);
#else
                    url = ("https://play.google.com/store/apps/details?id=" + settings.google.bundleId);
#endif
                    break;
                case StoreEnum.samsung:
#if UNITY_ANDROID
                    url = "";
#else
                    url = "";
#endif
                    break;
                case StoreEnum.osx:
#if UNITY_STANDALONE_OSX
                    url = ("macappstore://itunes.apple.com/app/pages/id" + settings.osx.appleId + "?mt=12");
#else
                    url = ("https://itunes.apple.com/app/pages/id" + s.osx.appleId + "?mt=12");
#endif
                    break;
                case StoreEnum.ios:
#if UNITY_IOS
                    url = ("itms-apps://itunes.apple.com/app/pages/id" + settings.ios.appleId + "?mt=8&uo=4");
#else
                    url = ("https://itunes.apple.com/app/pages/id" + s.ios.appleId + "?mt=8&uo=4");
#endif
                    break;
                case StoreEnum.wsa:
#if UNITY_WSA
				url = ("ms-windows-store://pdp/?ProductId=" + settings.wsa.storeId);
#else
                    url = ("https://www.microsoft.com/store/apps/" + settings.wsa.storeId);
#endif
                    break;
                case StoreEnum.udp:
#if UNITY_ANDROID
                    //url = ("market://details?id=" + s.google.bundleId);
#else
                    //url = ("https://play.google.com/store/apps/details?id=" + settings.google.bundleId);
#endif
                    break;
            }
            return url;
        }

        public static bool OpenStore(StoreEnum store, StoreSettings s = null)
        {
            s = s ?? settings;

            string url = GetStoreLink(store, s);

            if (string.IsNullOrEmpty(url))
            {
                Debug.Log("Open Store url is NULL!");
                return false;
            }
            else
            {
                Debug.Log("Open Store : " + url);
                Application.OpenURL(url);
                return true;
            }
        }

        public static string GetStoreLink(StoreSettings s = null)
        {
            s = s ?? settings;

#if UNITY_IOS
            return GetStoreLink(StoreEnum.ios, s);
#elif UNITY_ANDROID
            switch (s._appStore)
            {
                case "AmazonAppStore": return GetStoreLink(StoreEnum.amazon, s);                    
                case "SamsungApps": return GetStoreLink(StoreEnum.samsung, s);
                case "GooglePlay": return GetStoreLink(StoreEnum.google, s); 
                case "UDP": return GetStoreLink(StoreEnum.udp, s); 
                default: return "";
                    
            }
#elif UNITY_WSA || UNITY_STANDALONE_WIN
			return GetStoreLink(StoreEnum.wsa, s);
#elif UNITY_STANDALONE_OSX
            return GetStoreLink(StoreEnum.osx, s);
#else
            return "";
#endif
        }

        public static bool OpenStore(StoreSettings s = null)
        {
            s = s ?? settings;

#if UNITY_IOS
            return OpenStore(StoreEnum.ios, s);
#elif UNITY_ANDROID
            switch (s._appStore)
            {
                case "AmazonAppStore":
                    return OpenStore(StoreEnum.amazon, s);
                    break;
                case "SamsungApps":
                    return OpenStore(StoreEnum.samsung, s);
                    break;
                case "GooglePlay":
                    return OpenStore(StoreEnum.google, s);
                    break;
                case "UDP":
                    return OpenStore(StoreEnum.udp, s);
                    break;
                default:
                    Application.OpenURL(s.projectWWW);
                    return false;
                    break;
            }
#elif UNITY_WSA || UNITY_STANDALONE_WIN
			return OpenStore(StoreEnum.wsa, s);
#elif UNITY_STANDALONE_OSX
            return OpenStore(StoreEnum.osx, s);
#else
            Application.OpenURL(s.projectWWW);
            return false;
#endif
        }
    }
}
