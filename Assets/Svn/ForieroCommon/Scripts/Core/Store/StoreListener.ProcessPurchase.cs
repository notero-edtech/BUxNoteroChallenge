using ForieroEngine;
using UnityEngine;
using UnityEngine.Analytics;
using Debug = UnityEngine.Debug;
using PlayerPrefs = ForieroEngine.PlayerPrefs;

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
            public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
            {

                if (!Store.processing)
                {
                    if (Foriero.debug) { Debug.Log("User likely canceled transaction!"); }
                }

                if (Foriero.debug) { Debug.Log("STORE LISTENER : Purchase ok for " + e.purchasedProduct.definition.id + " " + e.purchasedProduct.receipt); }

                m_PurchaseInProgress = false;

                if (!ReceiptValidation()) return PurchaseProcessingResult.Complete;

                foreach (Store.Product p in Store.products)
                {
                    if (p.productIdentifier.Equals(e.purchasedProduct.definition.id))
                    {
                        PlayerPrefs.SetBool(p.productIdentifier, true);

                        Analytics.CustomEvent(Application.productName + "_" + Application.platform.ToString() + "_" + "INAPPPAID");

                        p?.OnPurchased?.Invoke(p.productIdentifier, e.purchasedProduct.receipt);
                    }
                }

                Store.processing = false;

                InAppMessage.SetMessageText("Congratulations, item purchased.");
                InAppMessage.SetButtonText("Close");
                InAppMessage.Destroy(2f);

                return PurchaseProcessingResult.Complete;
            }
#endif
        }
    }
}

