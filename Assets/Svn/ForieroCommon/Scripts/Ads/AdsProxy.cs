using ForieroEngine;
using UnityEngine;

#if FORIERO_ADS && (UNITY_IOS || UNITY_ANDROID)
using UnityEngine.Advertisements; 
#endif

namespace ForieroEngine.Ads
{
	public static partial class AdsProxy
	{
#pragma warning disable 414

#if DEVELOPMENT_BUILD
    static readonly bool testMode = true;
#else
		static readonly bool testMode = false;
#endif

		static string id = "";

#pragma warning restore 414

		public static bool initialized = false;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void Init()
		{
			System.Diagnostics.Stopwatch stopWatch =
				ForieroDebug.CodePerformance ? System.Diagnostics.Stopwatch.StartNew() : null;

			if (AdsProxySettings.instance.initialize)
			{
#if FORIERO_ADS && (UNITY_ANDROID || UNITY_IOS || UNITY_WSA)

			switch (Application.platform) {
			case RuntimePlatform.Android:
				id = AdsProxySettings.instance.androidId;
				break;
			case RuntimePlatform.IPhonePlayer:
			case RuntimePlatform.tvOS:
				id = AdsProxySettings.instance.iosId;
				break;
			case RuntimePlatform.WSAPlayerARM:
			case RuntimePlatform.WSAPlayerX64:
			case RuntimePlatform.WSAPlayerX86:
                    // it is resolved in WSANativeInit //
                    id = "";
				break;
			}

#endif

#if FORIERO_ADS && (UNITY_ANDROID || UNITY_IOS)

			Debug.LogWarning ("Initialize UnityAds : " + id);
			Advertisement.Initialize (id, testMode);

			if (Advertisement.isInitialized) {
				initialized = true;
				Debug.LogWarning ("Unity Ads is already initialized.");
			} else if (!Advertisement.isSupported) {
				initialized = false;
				Debug.LogError ("Unity Ads is not supported under the current build platform.");
			}

#endif

#if FORIERO_ADS && (UNITY_WSA)

			initialized = WSANativeInit();

#endif

			}
			else
			{
				initialized = false;
			}

			if (ForieroDebug.CodePerformance)
				Debug.Log("METHOD STOPWATCH (AdsProxy - AfterSceneLoad): " + stopWatch?.Elapsed.ToString());
		}

		public static bool Show(System.Action<bool> finished)
		{
#if FORIERO_ADS && (UNITY_ANDROID || UNITY_IOS || UNITY_WSA)
		return ShowAdInternal (finished);
#else
			return false;
#endif
		}

		public static bool IsReady()
		{
#if FORIERO_ADS && (UNITY_ANDROID || UNITY_IOS || UNITY_WSA)
		return IsReadyInternal (CI.WSANative.Advertising.WSAInterstitialAdType.Microsoft);
#else
			return false;
#endif
		}
	}
}
