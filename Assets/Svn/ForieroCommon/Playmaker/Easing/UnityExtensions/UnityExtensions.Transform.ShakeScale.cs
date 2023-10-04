using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static partial class UnityExtensions {
	
	#region MoveUpdate

	public static IEnumerator ShakeScale(this Transform aTransform,
	                                 float aDuration,
	                                 float aDelay,
	                                 Vector3 aShake,
	                                 System.Action onStarted,
	                                 System.Action onCompleted,
	                                 LoopType aLoopType,
	                                 bool aReverse,    
	                                 string aCategory,
	                                 string anId)
	{
		var time = 0f;
		var duration = aDuration;
		var delay = aDelay;
		var pingpong = 0;
		
		CancelSignal cs = null;
		cs = GetFreeCancelSignal();
		cs.id = anId;
		cs.category = aCategory;
		
		while(delay > 0)
		{
			delay-= Time.deltaTime;
			yield return null;
		}
		
		//values holder [0] root value, [1] amount, [2] generated amount:
		Vector3[] vector3s=new Vector3[3];
		
		//root:
		vector3s[0] = aTransform.localScale;		
						
		//amount:
		vector3s[1] = aShake;
		bool impact = true;
		float diminishingControl = 1f;
		
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
			
			//impact:
			if (impact == true) {
				aTransform.localScale=vector3s[1];
				impact = false;
			}
			
			//reset:
			aTransform.localScale=vector3s[0];
			
			//generate:
			diminishingControl = aReverse ? Mathf.Clamp01(time/aDuration) : 1 - Mathf.Clamp01(time/aDuration);
			vector3s[2].x= UnityEngine.Random.Range(-vector3s[1].x*diminishingControl, vector3s[1].x*diminishingControl);
			vector3s[2].y= UnityEngine.Random.Range(-vector3s[1].y*diminishingControl, vector3s[1].y*diminishingControl);
			vector3s[2].z= UnityEngine.Random.Range(-vector3s[1].z*diminishingControl, vector3s[1].z*diminishingControl);
	
			//apply:
			aTransform.localScale+=vector3s[2];
									
			if(cs.cancel == true) break;
														
			if(duration > 0f) yield return null;
			if(duration < 0){
				switch(aLoopType){
				case LoopType.None:
				break;
				case LoopType.Loop:
					duration = aDuration;
					time = 0f;
					delay = aDelay;
					while(delay > 0)
					{
						delay-= Time.deltaTime;
						yield return null;
					}
				break;
				case LoopType.Pingpong:
					duration = aDuration;
					time = 0f;
					aReverse = !aReverse;
					delay = aDelay;
					pingpong++;
					if(pingpong % 2 == 0){
						while(delay > 0)						
						{
							delay-= Time.deltaTime;
							yield return null;
						}
					}
				break;
				}
			}
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
