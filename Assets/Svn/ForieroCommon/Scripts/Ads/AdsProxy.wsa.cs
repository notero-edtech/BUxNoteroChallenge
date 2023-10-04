using UnityEngine;
using System.Collections;
using ForieroEngine;

#if FORIERO_ADS && (UNITY_WSA)
using CI.WSANative.Advertising;

namespace ForieroEngine.Ads
{
	public static partial class AdsProxy
	{
		static bool wsaAdReady = false;
		static bool wsaAdCompleted = false;
		static bool wsaAdCanceled = false;
		static bool wsaAdError = false;
		
		static System.Action<bool> finished;

		static bool WSANativeInit ()
		{
	        if (Foriero.debug)
	        {
	            Debug.Log("AdsProxy : WSANativeInit");
	        }

	        WSANativeInterstitialAd.AdReady = WSANativeAdReady;
			WSANativeInterstitialAd.Completed = WSANativeAdCompleted;
			WSANativeInterstitialAd.Cancelled = WSANativeAdCanceled;
			WSANativeInterstitialAd.ErrorOccurred = WSANativeAdError;
			WSANativeInterstitialAd.Initialise (WSAInterstitialAdType.Microsoft,
				SystemInfo.deviceType == DeviceType.Handheld ? AdsProxySettings.instance.wsaMobileAppId : AdsProxySettings.instance.wsaPcTabletAppId, 
				SystemInfo.deviceType == DeviceType.Handheld ? AdsProxySettings.instance.wsaMobileAdId : AdsProxySettings.instance.wsaPcTabletAdId
			);

	        RequestAd();

			return true;
		}

		static void RequestAd ()
		{
			wsaAdReady = false;
			wsaAdCompleted = false;
			wsaAdCanceled = false;
			wsaAdError = false;

			finished = null;

			WSANativeInterstitialAd.RequestAd (WSAInterstitialAdType.Microsoft);
		}

		static void WSANativeAdReady (WSAInterstitialAdType t)
		{
	        if (Foriero.debug)
	        {
	            Debug.Log("AdsProxy Callback : WSANativeAdReady");
	        }

	        wsaAdReady = true;
		}

		static void WSANativeAdCompleted(WSAInterstitialAdType t)
		{
	        if (Foriero.debug)
	        {
	            Debug.Log("AdsProxy Callback : WSANativeAdCompleted");
	        }

	        wsaAdCompleted = true;

			if (finished != null) {
				finished (true);
			}

			RequestAd ();
		}

		static void WSANativeAdCanceled(WSAInterstitialAdType t)
		{
	        if (Foriero.debug)
	        {
	            Debug.Log("AdsProxy Callback : WSANativeAdCanceled");
	        }


	        wsaAdCanceled = true;

			if (finished != null) {
				finished (false);
			}

			RequestAd ();
		}

		static void WSANativeAdError(WSAInterstitialAdType t)
		{
	        if (Foriero.debug)
	        {
	            Debug.Log("AdsProxy Callback : WSANativeAdError");
	        }


	        wsaAdError = true;

			if (finished != null) {
				finished (false);
			}
		}

		static bool IsReadyInternal(WSAInterstitialAdType t)
		{
	        if (Foriero.debug)
	        {
	            Debug.Log("AdsProxy : IsReadyInternal " + wsaAdReady.ToString());
	        }

	        return wsaAdReady;
		}

		static bool ShowAdInternal (System.Action<bool> finishedCallback)
		{
	        if (Foriero.debug)
	        {
	            Debug.Log("AdsProxy : ShowAdInternal ");
	        }

	        if (initialized && !wsaAdError && wsaAdReady) {
				finished = finishedCallback;
				WSANativeInterstitialAd.ShowAd (WSAInterstitialAdType.Microsoft);
				return true;
			} else {

	            if (Foriero.debug)
	            {
	                Debug.LogError("AdsProxy : ShowAdInternal, Initialized  " + initialized.ToString() + " wsaAdError " + wsaAdError.ToString() + " wsaAdReady " + wsaAdReady.ToString());
	            }

	            return false;
			}
		}
	}
}

#endif