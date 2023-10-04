#if FORIERO_INAPP
using UnityEngine.Purchasing;
#endif

namespace ForieroEngine.Purchasing
{
    public static partial class Store
    {
        partial class StoreListener : IStoreListener
        {
            public static bool initialized = false;
#if FORIERO_INAPP
            // Unity IAP objects
            public IStoreController m_Controller;
            public IExtensionProvider m_ExtensionsProvider;

            public IAppleExtensions m_AppleExtensions;           
            //public ISamsungAppsExtensions m_SamsungExtensions;
            public IUDPExtensions m_UDPExtensions;
            public IGooglePlayStoreExtensions m_GooglePlayStoreExtensions;
            public IAmazonExtensions m_AmazonExtensions;
            public IMicrosoftExtensions m_MicrosoftExtensions;            
            public ITransactionHistoryExtensions m_TransactionHistoryExtensions;

#pragma warning disable 0414
            private bool m_IsGooglePlayStoreSelected;
#pragma warning restore 0414
            private bool m_IsSamsungAppsStoreSelected;            
            private bool m_IsUDPSelected;

            private bool m_PurchaseInProgress;

            public IExtensionProvider ExtensionProvider
            {
                get { return m_ExtensionsProvider; }
            }
#endif
        }

#if !FORIERO_INAPP
    public interface IStoreListener{

    }
#endif
    }
}