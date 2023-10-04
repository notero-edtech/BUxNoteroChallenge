#if NETFX_CORE || (ENABLE_IL2CPP && UNITY_WSA_10_0)
using System;
using Windows.Services.Store;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
#endif

using System.Runtime.InteropServices;

public partial class NativeReviewRequest
{
    public static void RequestReview()
    {
#if UNITY_EDITOR

#elif UNITY_IOS
        UnityEngine.iOS.Device.RequestStoreReview();
#elif UNITY_STANDALONE_OSX
        requestReview();
#elif NETFX_CORE || (ENABLE_IL2CPP && UNITY_WSA_10_0)
        requestReview();
#elif UNITY_ANDROID
       
#endif
    }

#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
    [DllImport ("nativereview")] private static extern void requestReview();
#elif NETFX_CORE || (ENABLE_IL2CPP && UNITY_WSA_10_0)
    public static async Task<bool> requestReview()
    {
        StoreSendRequestResult result = await StoreRequestHelper.SendRequestAsync(
            StoreContext.GetDefault(), 16, String.Empty);

        if (result.ExtendedError == null)
        {
            JObject jsonObject = JObject.Parse(result.Response);
            if (jsonObject.SelectToken("status").ToString() == "success")
            {
                // The customer rated or reviewed the app.
                return true;
            }
        }

        // There was an error with the request, or the customer chose not to
        // rate or review the app.
        return false;
    }
#endif
}
