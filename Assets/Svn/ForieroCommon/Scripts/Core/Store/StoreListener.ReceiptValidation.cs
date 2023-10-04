using UnityEngine;

#if FORIERO_INAPP
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
#endif

namespace ForieroEngine.Purchasing
{
    public static partial class Store
    {
        partial class StoreListener : IStoreListener
        {
#if FORIERO_INAPP && RECEIPT_VALIDATION
    private CrossPlatformValidator validator;

    bool ReceiptValidation(){

        if (m_IsGooglePlayStoreSelected || (m_IsUnityChannelSelected && m_FetchReceiptPayloadOnPurchase) ||
            Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer ||
            Application.platform == RuntimePlatform.tvOS) {
            try {
                var result = validator.Validate(e.purchasedProduct.receipt);
                Debug.Log("Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in result) {
                    Debug.Log(productReceipt.productID);
                    Debug.Log(productReceipt.purchaseDate);
                    Debug.Log(productReceipt.transactionID);

                    GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                    if (null != google) {
                        Debug.Log(google.purchaseState);
                        Debug.Log(google.purchaseToken);
                    }

                    UnityChannelReceipt unityChannel = productReceipt as UnityChannelReceipt;
                    if (null != unityChannel) {
                        Debug.Log(unityChannel.productID);
                        Debug.Log(unityChannel.purchaseDate);
                        Debug.Log(unityChannel.transactionID);
                    }

                    AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
                    if (null != apple) {
                        Debug.Log(apple.originalTransactionIdentifier);
                        Debug.Log(apple.subscriptionExpirationDate);
                        Debug.Log(apple.cancellationDate);
                        Debug.Log(apple.quantity);
                    }

                    // For improved security, consider comparing the signed
                    // IPurchaseReceipt.productId, IPurchaseReceipt.transactionID, and other data
                    // embedded in the signed receipt objects to the data which the game is using
                    // to make this purchase.
                }
            } catch (IAPSecurityException ex) {
                Debug.Log("Invalid receipt, not unlocking content. " + ex);
                return false;
            }
        }

        return true;
    }
#elif FORIERO_INAPP
            bool ReceiptValidation()
            {
                return true;
            }
#endif
        }
    }
}
