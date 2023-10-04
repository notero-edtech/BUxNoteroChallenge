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
            public void OnInitializeFailed(InitializationFailureReason error)
            {
                if (!Foriero.debug) return;

                Debug.Log("STORE LISTENER : Billing failed to initialize!");
                switch (error)
                {
                    case InitializationFailureReason.AppNotKnown:
                        Debug.LogError("STORE LISTENER : Is your App correctly uploaded on the relevant publisher console?");
                        break;
                    case InitializationFailureReason.PurchasingUnavailable:
                        // Ask the user if billing is disabled in device settings.
                        Debug.Log("STORE LISTENER : Billing disabled!");
                        break;
                    case InitializationFailureReason.NoProductsAvailable:
                        // Developer configuration error; check product metadata.
                        Debug.Log("STORE LISTENER : No products available for purchase!");
                        break;
                }
            }
#endif
        }
    }
}
