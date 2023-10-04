using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static partial class UnityExtensions {
	
	#region Float
	
	public static IEnumerator AnimateTo(this float aFloat,
	                                 float aTo,
	                                 float aDuration,
	                                 float aDelay,
	                                 System.Func<float, float, float, float> anEaseFunc,
	                                 System.Action<float> aCallback,
	                                 System.Action onStarted,
	                                 System.Action onCompleted,
	                                 bool aReverse,
	                                 string aCategory,
	                                 string anId)
	{
		var fromValue = aFloat;
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
			aFloat = anEaseFunc(fromValue, aTo, timeClamp01);
			
			if(cs.cancel == true) break;
			
			aCallback(aFloat);
				
			if(duration > 0f) yield return null;
		}
		
		if(onCompleted != null) onCompleted();
		
		cs.id = "";
		cs.cancel = false;
		cs.paused = false;
		cs.free = true;
		yield break;
	}
	
	#endregion
	
}
