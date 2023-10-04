//#define DEVELOPMENT_BUILD
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine;
using UnityEngine.Analytics;

#if FORIERO_INAPP
using UnityEngine.Purchasing;
using ForieroEngine.Purchasing;
#endif

using PlayerPrefs = ForieroEngine.PlayerPrefs;

namespace ForieroEngine.Purchasing
{
    public static partial class Store
    {
        public enum StoreEnum
        {
            amazon = 10,
            samsung = 20,
            google = 30,
            osx = 40,
            ios = 50,
            wsa = 60,
            udp = 70,
            xiaomi = 80
        }

        public enum PurchaseEnum
        {
            Consumable,
            NonConsumable,
            Subscription
        }

        public enum ProjectVersion
        {
            None = 0,
            Free = 1,
            Lite = 2,
            Pro = 3
        }

        public class Product
        {
            public readonly string productIdentifier;
            public readonly PurchaseEnum purchaseType;

            public bool purchased
            {
                get
                {
#if FORIERO_INAPP
                    if (StoreListener.initialized)
                    {
                        if (PlayerPrefs.HasKey<bool>(productIdentifier))
                        {
                            return PlayerPrefs.GetBool(productIdentifier, false);
                        }
                        else
                        {
                            switch (purchaseType)
                            {
                                case PurchaseEnum.NonConsumable:
                                case PurchaseEnum.Subscription:
                                    if (storeListener != null && storeListener.m_Controller != null)
                                    {
                                        var p = storeListener.m_Controller.products.WithID(productIdentifier);
                                        return p == null ? false : p.hasReceipt;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                case PurchaseEnum.Consumable:
                                    return PlayerPrefs.GetBool(productIdentifier, false);
                            }

                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
#else
					return false;
#endif
                }
            }

            /// <summary>
            /// Product ID
            /// Receipt
            /// </summary>
            public Action<string, string> OnPurchased;
            /// <summary>
            /// Product ID
            /// Reason
            /// </summary>
            public Action<string, string> OnFailed;
            
            public Product(string productIdentifier, PurchaseEnum purchaseType)
            {
                this.productIdentifier = productIdentifier;
                this.purchaseType = purchaseType;
            }
        }

        public static List<Product> products = new List<Product>();
        public static bool initialized = false;
        public static ProjectVersion projectVersion = ProjectVersion.None;
        public static StoreSettings settings{ get { return StoreSettings.instance; } }

#if FORIERO_INAPP
        static StoreListener storeListener;
        static ConfigurationBuilder configBuilder;
        static StandardPurchasingModule purchaseModule;
        static IAmazonConfiguration amazonConfig;
        //static ISamsungAppsConfiguration samsungConfig;
        static IGooglePlayConfiguration googleConfig;        
        static IMicrosoftConfiguration microsoftConfig;       
        
        static void OnPromotionalPurchase(UnityEngine.Purchasing.Product product)
        {
            foreach (Store.Product p in Store.products)
            {
                if (p.productIdentifier.Equals(product.definition.id))
                {
                    PlayerPrefs.SetBool(p.productIdentifier, true);

                    Analytics.CustomEvent(Application.productName + "_" + Application.platform.ToString() + "_" + "INAPPPAID");

                    p?.OnPurchased(p.productIdentifier, product.receipt);                    
                }
            }
        }
#endif

        static void Init()
        {
            if (Foriero.debug) { Debug.Log("STORE |  Adding products"); }

            foreach (StoreSettings.PurchaseItem purchaseItem in settings.purchaseItems)
            {
                if (Foriero.debug) { Debug.Log("Store Product : " + purchaseItem.id); }
                products.Add(new Product(purchaseItem.id, purchaseItem.purchaseType));
            }

#if FORIERO_INAPP

            if (Foriero.debug) { Debug.Log("STORE |  ConfigurationBuilder"); }

            if (Application.platform == RuntimePlatform.Android) purchaseModule = StandardPurchasingModule.Instance(StoreSettings.instance.appStore);
            else purchaseModule = StandardPurchasingModule.Instance();
            
            configBuilder = ConfigurationBuilder.Instance(purchaseModule);
            configBuilder.useCatalogProvider = settings.useCatalogProvider;

            if (Application.platform == RuntimePlatform.Android)
            {
                switch (StoreSettings.instance.appStore)
                {
                    case AppStore.GooglePlay:
                        //configBuilder.Configure<IGooglePlayConfiguration>().SetPublicKey(settings.google.publicKey);
                        break;
                    case AppStore.AmazonAppStore:
                        //configBuilder.Configure<IAmazonConfiguration>().WriteSandboxJSON();
                        break;
                    case AppStore.MacAppStore:
                        //configBuilder.Configure<ISamsungAppsConfiguration>().SetMode(SamsungAppsMode.Production);
                        break;
                    case AppStore.UDP:
                        
                        break;
                }
            }
            else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.tvOS || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                configBuilder.Configure<IAppleConfiguration>().SetApplePromotionalPurchaseInterceptorCallback(OnPromotionalPurchase);
            }

            if (settings && !settings.useCatalogProvider)
            {
                foreach (StoreSettings.PurchaseItem purchaseItem in settings.purchaseItems)
                {
                    switch (purchaseItem.purchaseType)
                    {
                        case PurchaseEnum.Consumable:
                            configBuilder.AddProduct(purchaseItem.id, ProductType.Consumable, new IDs {
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.amazon), AmazonApps.Name },
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.google), GooglePlay.Name },
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.ios), AppleAppStore.Name },
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.osx), MacAppStore.Name },
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.wsa), WindowsStore.Name },
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.udp), UDP.Name },                            
                        });
                            break;
                        case PurchaseEnum.NonConsumable:
                            configBuilder.AddProduct(purchaseItem.id, ProductType.NonConsumable, new IDs {
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.amazon), AmazonApps.Name },
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.google), GooglePlay.Name },
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.ios), AppleAppStore.Name },
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.osx), MacAppStore.Name },
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.wsa), WindowsStore.Name },
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.udp), UDP.Name },                            
                        });
                            break;
                        case PurchaseEnum.Subscription:
                            configBuilder.AddProduct(purchaseItem.id, ProductType.Subscription, new IDs {
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.amazon), AmazonApps.Name },
                           // { settings.GetStoreInAppId (purchaseItem, StoreEnum.samsung), SamsungApps.Name },
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.google), GooglePlay.Name },
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.ios), AppleAppStore.Name },
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.osx), MacAppStore.Name },
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.wsa), WindowsStore.Name },
                            { settings.GetStoreInAppId (purchaseItem, StoreEnum.udp), UDP.Name },                            
                        });
                            break;
                    }
                }
            }

            InitDevelopmentBuild();

            storeListener = new StoreListener();

            try
            {
                UnityPurchasing.Initialize(storeListener, configBuilder);
            }
            catch (System.Exception e)
            {
                Debug.LogError("STORE | UnityPurchasing.Initialize : " + e.Message);
            }

            initialized = true;

            if (Foriero.debug) Debug.Log("STORE | Initialized and waiting for unity to finish initialization.");            
#endif
        }

        static void InitDevelopmentBuild()
        {
#if FORIERO_INAPP && (DEVELOPMENT_BUILD || UNITY_EDITOR)

            if (Foriero.debug) Debug.LogWarning("STORE | Enabling Fake InApp ( build is marked as development build )");
            purchaseModule.useFakeStoreUIMode = settings.fakeStoreUIMode;
#endif

#if FORIERO_INAPP && DEVELOPMENT_BUILD && UNITY_WSA

            microsoftConfig = configBuilder.Configure<IMicrosoftConfiguration> ();
			if (microsoftConfig != null) {
				if (Foriero.debug) Debug.Log ("STORE | Microsoft Configuration - Enabling useMockBillingSystem");				
				microsoftConfig.useMockBillingSystem = true;
			}

#elif FORIERO_INAPP && DEVELOPMENT_BUILD && UNITY_ANDROID

             if(StoreSettings.instance.appStore == AppStore.AmazonAppStore){
                amazonConfig = configBuilder.Configure<IAmazonConfiguration> ();
			    if (amazonConfig != null) {
				    if (Foriero.debug) Debug.Log ("STORE | Amazon Configuration - Writing SandboxJSON");
				    amazonConfig.WriteSandboxJSON (configBuilder.products);
			    }
            }
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void AutoInit()
        {
            System.Diagnostics.Stopwatch stopWatch = ForieroDebug.CodePerformance ? System.Diagnostics.Stopwatch.StartNew() : null;
#if FORIERO_INAPP

            if (Foriero.debug) Debug.Log("STORE | AutoInit - INAPP Defined");

            if (initialized)
            {
                if (Foriero.debug) Debug.LogWarning("STORE | Already initialized!");
                return;
            }
            else if (storeListener != null)
            {
                if (Foriero.debug) Debug.LogWarning("STORE | StoreListener already added in scene!");
                return;
            }
            else
            {
                if (settings.initialize)
                {                    
                    Init();
                }
                else
                {
                    if (Foriero.debug) Debug.LogWarning("STORE | StoreSettings are set as not to initialize!!!");
                }
            }

#else
            if (ForieroDebug.InAppPurchases) { Debug.LogWarning ("STORE | AutoInit - INAPP NOT Defined"); }
#endif

            if (!PlayerPrefs.HasKey<bool>("Installed"))
            {
                Analytics.CustomEvent(Application.productName + "_" + Application.platform.ToString() + "_" + "INSTALLED");
                PlayerPrefs.SetBool("Installed", true);
            }

            Analytics.CustomEvent(Application.productName + "_" + Application.platform.ToString() + "_" + "LAUNCHED");
            if (ForieroDebug.CodePerformance) Debug.Log("METHOD STOPWATCH (Storet - AfterSceneLoad): " + stopWatch?.Elapsed.ToString());
        }
    }
}
