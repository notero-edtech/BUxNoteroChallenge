using ForieroEngine;
using UnityEngine;
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
            public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureReason reason)
            {
                if (!Store.processing)
                {
                    if (Foriero.debug) { Debug.Log("User likely canceled transaction!"); }
                }

                if (Foriero.debug) { Debug.Log("STORE LISTENER : Purchase failed for " + product.definition.id + " " + reason); }

                //// Detailed debugging information
                //Debug.Log("Store specific error code: " + m_TransactionHistoryExtensions.GetLastStoreSpecificPurchaseErrorCode());
                //if (m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription() != null)
                //{
                //    Debug.Log("Purchase failure description message: " + m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription().message);
                //}

                //if (m_IsUnityChannelSelected)
                //{
                //    var extra = m_UnityChannelExtensions.GetLastPurchaseError();
                //    var purchaseError = JsonUtility.FromJson<Unitychan>(extra);

                //    if (purchaseError != null && purchaseError.purchaseInfo != null)
                //    {
                //        // Additional information about purchase failure.
                //        var purchaseInfo = purchaseError.purchaseInfo;
                //        Debug.LogFormat(
                //            "UnityChannel purchaseInfo: productCode = {0}, gameOrderId = {1}, orderQueryToken = {2}",
                //            purchaseInfo.productCode, purchaseInfo.gameOrderId, purchaseInfo.orderQueryToken);
                //    }

                //    // Determine if the user already owns this item and that it can be added to
                //    // their inventory, if not already present.
                //    if (r == PurchaseFailureReason.DuplicateTransaction)
                //    {
                //        // Unlock `item` in inventory if not already present.
                //        Debug.Log("Duplicate transaction detected, unlock this item");
                //    }
                //}

                m_PurchaseInProgress = false;

                foreach (Store.Product p in Store.products)
                {
                    if (p.productIdentifier.Equals(product.definition.id))
                    {
                        p?.OnFailed?.Invoke(p.productIdentifier, reason.ToString());
                    }
                }

                Store.processing = false;

                InAppMessage.SetMessageText("Sorry, item purchase failed!");
                InAppMessage.SetButtonText("Close");
                InAppMessage.Destroy(2f);
            }
#endif
        }
    }
}
