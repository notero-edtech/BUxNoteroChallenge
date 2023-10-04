using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static partial class UnityExtensions {
			
	#region Vector3
	
	public static IEnumerator AnimateTo(this Vector3 aVector3,
	                                 Vector3 aTo,
	                                 float aDuration,
	                                 float aDelay,
	                                 System.Func<float, float, float, float> anEaseFunc,
	                                 System.Action<Vector3> aCallback,
	                                 System.Action onStarted,
	                                 System.Action onCompleted,
	                                 bool aReverse,
	                                 string aCategory,
	                                 string anId)
	{
		var fromValue = aVector3;
		var time = 0f;
		var timeClamp01 = 0f;
		var duration = aDuration;
		var delay = aDelay;
		
		CancelSignal cs = null;
		cs = GetFreeCancelSignal();
		cs.id = anId;
		cs.category = aCategory;
		
		while(delay > 0)
		{
			delay-= Time.deltaTime;
			yield return null;
		}
		
		if(onStarted != null) onStarted();
		
		while(duration > 0f)
		{
			if(cs != null){ 
			   	if(!cs.paused){
					time += Time.deltaTime;
					duration-=Time.deltaTime;
				}
			} else {
				time += Time.deltaTime;
				duration-=Time.deltaTime;
			}
			
			timeClamp01 = aReverse ? 1 - Mathf.Clamp01(time/aDuration) : Mathf.Clamp01(time/aDuration);
			aVector3.x = anEaseFunc(fromValue.x, aTo.x, timeClamp01);
			aVector3.y = anEaseFunc(fromValue.y, aTo.y, timeClamp01);
			aVector3.z = anEaseFunc(fromValue.z, aTo.z, timeClamp01);
			
			if(cs.cancel == true) break;
			
			aCallback(aVector3);
									
			if(duration > 0f) yield return null;
		}
		
		cs.id = "";
		cs.cancel = false;
		cs.paused = false;
		cs.free = true;	
		
		if(onCompleted != null) onCompleted();
	}
	
	#endregion

}
