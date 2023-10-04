using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ForieroEngine;
using ForieroEngine.Extensions;
using DG.Tweening;

// you need to address 14393 //
using CI.WSANative.Store;

public class TrialWall : MonoBehaviour
{
	public Text messageText;
    public GameObject tvOffPrefab;

	public static GameObject PREFAB_Trial_wall;

	public static GameObject trialWall = null;

	public static TrialWall singleton = null;

	static bool purchasing = false;

	static System.Action finished;

	public static bool IsTrial ()
	{
		bool result = true;
#if UNITY_EDITOR
		result = true;
#elif UNITY_WSA
        try {    
            var license = WSANativeStore.GetAppLicense ();
		    result = license.IsTrial;
            Debug.Log("IsTrial : " + license.IsTrial.ToString() + " IsActive : " + license.IsActive.ToString());
        } catch (System.Exception e) {
            result = true;
            Debug.LogError("IsTrialExpired : " + e.Message);
        }
#endif
		return result;
	}

	public static bool IsTrialExpired ()
	{
		bool result = true;
#if UNITY_EDITOR
		result = true;
#elif UNITY_WSA
        try{
		    var license = WSANativeStore.GetAppLicense ();
		    result = license.IsTrial && !license.IsActive;
            Debug.Log("IsTrialExpired : " + result.ToString() + " IsTrial : " + license.IsTrial.ToString() + " IsActive : " + license.IsActive.ToString());
        } catch (System.Exception e) {
            result = true;
            Debug.LogError("IsTrialExpired : " + e.Message);
        }
#endif
        return result;
	}

	public static void Create (System.Action finishedCallback)
	{
		finished = finishedCallback;

		if (Foriero.debug) {
			Debug.Log ("Trial Wall");
		}

		if (trialWall) {
			if (Foriero.debug) {
				Debug.LogError ("Trial Wall already exists!");
			}
			return;
		}

		if (PREFAB_Trial_wall == null) {
			PREFAB_Trial_wall = Resources.Load<GameObject> ("PREFAB_Trial_Wall");
		}

		if (PREFAB_Trial_wall == null) {
			if (Foriero.debug) {
				Debug.LogError ("Trial Wall not found!");
			}
			return;
		}

		trialWall = Instantiate (PREFAB_Trial_wall, Vector3.zero, Quaternion.identity) as GameObject;
	}

	void Awake ()
	{
		singleton = this;
	}

	void OnDestroy ()
	{
		singleton = null;
	}

	public void OnTrialPurchaseClick ()
	{
#if UNITY_EDITOR
        return;
#elif UNITY_WSA
		if (purchasing) return;

		purchasing = true;

		WSANativeStore.RequestPurchase (StoreSettings.instance.wsa.storeId, (receipt) => {
            Debug.Log (receipt);
            purchasing = false;

			if (IsTrial ()) {
				messageText.text = "Purchase failed. Please try again.";
			} else {
				Destroy (this.gameObject);
		        finished?.Invoke();
                finished = null;
            }
		});
#endif
    }

    public void OnQuitClick ()
	{
        Instantiate(tvOffPrefab);
	}
}
