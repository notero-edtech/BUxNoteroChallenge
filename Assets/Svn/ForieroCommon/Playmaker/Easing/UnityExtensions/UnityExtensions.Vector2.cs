using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static partial class UnityExtensions {
		
	#region Vector2
	
	public static IEnumerator AnimateTo(this Vector2 aVector2,
	                                 Vector2 aTo,
	                                 float aDuration,
	                                 float aDelay,
	                                 System.Func<float, float, float, float> anEaseFunc,
	                                 System.Action<Vector2> aCallback,
	                                 System.Action onStarted,
	                                 System.Action onCompleted,
	                                 bool aReverse,
	                                 string aCategory,
	                                 string anId)
	{
		var fromValue = aVector2;
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
			aVector2.x = anEaseFunc(fromValue.x, aTo.x, timeClamp01);
			aVector2.y = anEaseFunc(fromValue.y, aTo.y, timeClamp01);
			
			if(cs.cancel == true) break;
			
			aCallback(aVector2);
						
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
