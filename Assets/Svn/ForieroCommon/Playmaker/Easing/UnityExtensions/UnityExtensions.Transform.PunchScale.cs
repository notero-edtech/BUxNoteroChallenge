using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static partial class UnityExtensions {
	
	#region MoveUpdate

	public static IEnumerator PunchScale(this Transform aTransform,
	                                 float aDuration,
	                                 float aDelay,
	                                 Vector3 aPunch,
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
		
		Vector3[] vector3s = new Vector3[3];
			
		//from values:
		vector3s[0] = aTransform.localScale;
				
		//amount:
		vector3s[1] = aPunch;
		float timeClamp01 = 0f;
		
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
							
			//calculate:
			timeClamp01 = aReverse ? 1f - Mathf.Clamp01(time/aDuration) : Mathf.Clamp01(time/aDuration);
			if(vector3s[1].x>0){
				vector3s[2].x = UnityExtensions.easePunch(vector3s[1].x,timeClamp01);
			}else if(vector3s[1].x<0){
				vector3s[2].x=-UnityExtensions.easePunch(Mathf.Abs(vector3s[1].x),timeClamp01); 
			}
			if(vector3s[1].y>0){
				vector3s[2].y=UnityExtensions.easePunch(vector3s[1].y,timeClamp01);
			}else if(vector3s[1].y<0){
				vector3s[2].y=-UnityExtensions.easePunch(Mathf.Abs(vector3s[1].y),timeClamp01); 
			}
			if(vector3s[1].z>0){
				vector3s[2].z=UnityExtensions.easePunch(vector3s[1].z,timeClamp01);
			}else if(vector3s[1].z<0){
				vector3s[2].z=-UnityExtensions.easePunch(Mathf.Abs(vector3s[1].z),timeClamp01); 
			}
			
			//apply:
			aTransform.localScale=vector3s[0]+vector3s[2];
									
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
