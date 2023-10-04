using ForieroEngine;
using Debug = UnityEngine.Debug;

#if FORIERO_INAPP
using UnityEngine.Purchasing;
#endif

namespace ForieroEngine.Purchasing
{
    public static partial class Store
    {
        partial class StoreListener : IStoreListener
        {
#if FORIERO_INAPP
            public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
            {
                if (Foriero.debug) Debug.Log("STORE LISTENER : OnInitialized");
                
                m_Controller = controller;
                m_ExtensionsProvider = extensions;

                m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
                //m_SamsungExtensions = extensions.GetExtension<ISamsungAppsExtensions>();
                m_AmazonExtensions = extensions.GetExtension<IAmazonExtensions>();
                m_GooglePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();                
                m_MicrosoftExtensions = extensions.GetExtension<IMicrosoftExtensions>();                
                m_TransactionHistoryExtensions = extensions.GetExtension<ITransactionHistoryExtensions>();
                m_UDPExtensions = extensions.GetExtension<IUDPExtensions>();

                // On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.
                // On non-Apple platforms this will have no effect; OnDeferred will never be called.
                m_AppleExtensions?.RegisterPurchaseDeferredListener(OnDeferred);
                // m_AppleExtensions?.RefreshAppReceipt((s) =>
                // {
                //     if(Foriero.debug) Debug.Log($"STORE LISTENER | OnInitialized : AppReceipt : {s}");
                // }, () =>
                // {
                //     Debug.LogError($"STORE LISTENER | OnInitialized : RefreshAppReceipt Failed!!!");
                // });

                foreach (var item in controller.products.all)
                {
                    if (item.availableToPurchase)
                    {
                        // Set all these products to be visible in the user's App Store
                        m_AppleExtensions?.SetStorePromotionVisibility(item, AppleStorePromotionVisibility.Show);
                    }
                }

                foreach (var item in controller.products.all)
                {
                    if (item.availableToPurchase)
                    {
                        if (Foriero.debug)
                        {
                            Debug.Log("STORE LISTENER : " + System.Environment.NewLine +
                            string.Join(" - ",
                                new[] {
                            item.metadata.localizedTitle,
                            item.metadata.localizedDescription,
                            item.metadata.isoCurrencyCode,
                            item.metadata.localizedPrice.ToString (),
                            item.metadata.localizedPriceString
                                }));
                        }
                    }
                }

                // Populate the product menu now that we have Products
                for (int t = 0; t < m_Controller.products.all.Length; t++)
                {
                    var unityProduct = m_Controller.products.all[t];
                    foreach (var product in Store.products)
                    {
                        var description = $"{unityProduct.metadata.localizedTitle} - {unityProduct.metadata.localizedPriceString}";
                        if (Foriero.debug) Debug.Log($"STORE LISTENER | Product : {description}");
                    }
                }
                
                initialized = true;
                if (Foriero.debug) { Debug.Log("STORE LISTENER | Initialized"); }
            }
#endif
        }
    }
}