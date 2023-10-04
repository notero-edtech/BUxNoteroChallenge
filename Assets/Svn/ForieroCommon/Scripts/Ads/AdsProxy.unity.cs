using UnityEngine;
using System.Collections;

#if FORIERO_ADS && (UNITY_IOS || UNITY_ANDROID)

using UnityEngine.Advertisements;

namespace ForieroEngine.Ads
{
	public static partial class AdsProxy
	{
		static bool IsReadyInternal ()
		{
			if (Advertisement.isInitialized) {
				Debug.LogWarning ("Unity Ads ready : " + Advertisement.IsReady ().ToString ().ToUpper ());
				return Advertisement.IsReady ();
			} else {
				Debug.LogWarning ("Unity Ads is not initialized.");
				return false;
			}
		}

		static bool ShowAdInternal (System.Action<bool> finished)
		{
			string zoneID = null;

			if (!Advertisement.isInitialized) {
				Debug.LogWarning ("Unity Ads is not initialized."); 
				return false;
			} else if (!Advertisement.IsReady (zoneID)) {
				Debug.LogError ("Failed to show ad. Zone is not ready.");
				return false;
			} else {
				Debug.Log ("Showing ad.");
				Advertisement.Show (zoneID, new ShowOptions { 
					resultCallback = result => { 
						Debug.Log ("Show result: " + result.ToString ()); 
						if (finished != null)
							finished (result == ShowResult.Finished); 
					} 
				});
				return true;
			}
		}
	}
}
#endif
