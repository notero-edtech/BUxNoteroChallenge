using ForieroEngine;
using UnityEngine;
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
            public void OnTransactionsRestored(bool success)
            {
#if FORIERO_INAPP
                Debug.Log("STORE LISTENER : Transactions restored");

                if (!Store.processing) { if (Foriero.debug) { Debug.Log("STORE LISTENER : User likely canceled transaction!"); } }

                if (success)
                {
                    if (Foriero.debug) { Debug.Log("STORE LISTENER : Restore transactions succeeded!"); }

                    InAppMessage.SetMessageText("Congratulations, items restored!");
                    InAppMessage.SetButtonText("Close");

                    foreach (UnityEngine.Purchasing.Product item in m_Controller.products.all)
                    {
                        foreach (Store.Product p in Store.products)
                        {
                            if (p.purchaseType == Store.PurchaseEnum.Consumable) continue;

                            if (item.definition.id.Equals(p.productIdentifier, System.StringComparison.Ordinal))
                            {
                                if (item.hasReceipt || Application.isEditor)
                                {
                                    PlayerPrefs.SetBool(p.productIdentifier, true);
                                    p?.OnPurchased?.Invoke(p.productIdentifier, item.receipt);
                                }
                                else
                                {
                                    if (Foriero.debug) { Debug.LogError("STORE LISTENER : InApp product does not have receipt : " + p.productIdentifier); }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Foriero.debug) { Debug.LogError("STORE LISTENER : Restore transactions failed!"); }

                    InAppMessage.SetMessageText("Sorry, restore transactions failed!");
                    InAppMessage.SetButtonText("Close");
                }

                Store.processing = false;

                InAppMessage.Destroy(2f);
#endif
            }

            /// <summary>
            /// iOS Specific.
            /// This is called as part of Apple's 'Ask to buy' functionality,
            /// when a purchase is requested by a minor and referred to a parent
            /// for approval.
            /// 
            /// When the purchase is approved or rejected, the normal purchase events
            /// will fire.
            /// </summary>
            /// <param name="item">Item.</param>
#if FORIERO_INAPP
            private void OnDeferred(UnityEngine.Purchasing.Product product)
            {
                Debug.Log("STORE LISTENER : Purchase deferred for " + product.definition.id);

                //		InAppMessage.SetMessageText ("Sorry, item deffered!");
                //		InAppMessage.SetButtonText ("Close");
                //
                //		InAppMessage.Destroy (2f);
            }
#endif
        }
    }
}
