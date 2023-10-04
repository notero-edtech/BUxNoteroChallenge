using UnityEngine;
using System.Collections;

public static partial class UnityExtensions {
	public static IEnumerator WaitAndFire(this MonoBehaviour aMonoBehaviour, float aTime, System.Action onFire, string aCategory = "", string anId = ""){
		CancelSignal cs = null;
		cs = GetFreeCancelSignal();
		cs.id = anId;
		cs.category = aCategory;
		var callEvent = true;
		while(aTime >= 0f){
			if(cs.cancel == true){
				callEvent = false;
				break;
			}
			else 
				yield return null;
			aTime-= Time.deltaTime;
			
		}
		if(callEvent) if(onFire != null) onFire();
		cs.id = "";
		cs.cancel = false;
		cs.paused = false;
		cs.free = true;
		yield break;
	}
}
