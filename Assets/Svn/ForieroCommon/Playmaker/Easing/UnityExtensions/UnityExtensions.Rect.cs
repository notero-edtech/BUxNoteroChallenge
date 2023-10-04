using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static partial class UnityExtensions {
				
	#region Rect
	
	public static IEnumerator AnimateTo(this Rect aRect,
	                                 Rect aTo,
	                                 float aDuration,
	                                 float aDelay,
	                                 System.Func<float, float, float, float> anEaseFunc,
	                                 System.Action<Rect> aCallback,
	                                 System.Action onStarted,
	                                 System.Action onCompleted,
	                                 bool aReverse,
	                                 string aCategory,
	                                 string anId)
	{
		var fromValue = aRect;
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
			aRect.x = anEaseFunc(fromValue.x, aTo.x, timeClamp01);
			aRect.y = anEaseFunc(fromValue.y, aTo.y, timeClamp01);
			aRect.width = anEaseFunc(fromValue.width, aTo.width, timeClamp01);
			aRect.height = anEaseFunc(fromValue.height, aTo.height, timeClamp01);
			
			if(cs.cancel == true) break;				
			
			aCallback(aRect);
						
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
