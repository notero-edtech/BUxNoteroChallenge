using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static partial class UnityExtensions {
	
	#region MoveUpdate

	public static IEnumerator ScaleUpdate(this Transform aTransform,
	                                 float aDuration,
	                                 float aDelay,
	                                 float aSmoothTime,
	                                 Transform aTarget,
	                                 Vector3 anOffset,
	                                 int aConstraints,
	                                 System.Action onStarted,
	                                 System.Action onCompleted,
	                                 string aCategory,
	                                 string anId)
	{
		
		CancelSignal cs = null;
		cs = GetFreeCancelSignal();
		cs.id = anId;
		cs.category = aCategory;
		
		var time = 0f;
		var duration = aDuration;
		var delay = aDelay;
		var timed = aDuration > 0f;
		
		while(delay > 0)
		{
			delay-= Time.deltaTime;
			yield return null;
		}
		
		float scaleTime = aSmoothTime;
		float smoothOut = 0.8f;
		float smoothIn = 0.8f;
		Vector3[] vector3s = new Vector3[5];
		
		if(onStarted != null) onStarted();
						
		while(true)
		{
			if(timed){
				if(!cs.paused){
					time += Time.deltaTime;
					duration-=Time.deltaTime;
				}	
			}
			
			if(smoothIn > 0f){
				smoothIn-= Time.deltaTime;
				scaleTime = aSmoothTime + (smoothIn*2f);	
			}
			//init values:
			vector3s[0] = vector3s[1] = aTransform.localScale;
			
			//to values:
			vector3s[1]= aTarget.localScale + anOffset;
					
			if(!cs.paused){
				//calculate:
				vector3s[3].x=Mathf.SmoothDamp(vector3s[0].x,vector3s[1].x,ref vector3s[2].x,scaleTime);
				vector3s[3].y=Mathf.SmoothDamp(vector3s[0].y,vector3s[1].y,ref vector3s[2].y,scaleTime);
				vector3s[3].z=Mathf.SmoothDamp(vector3s[0].z,vector3s[1].z,ref vector3s[2].z,scaleTime);
			}
					
			//apply:
			vector3s[4]=vector3s[3];
			
			//axis restriction
			switch(aConstraints){
				case 1:
					vector3s[4].x=vector3s[0].x;
				break;
				case 2:
					vector3s[4].y=vector3s[0].y;
				break;
				case 4:
					vector3s[4].z=vector3s[0].z;
				break;
				case 3:
					vector3s[4].x=vector3s[0].x;
					vector3s[4].y=vector3s[0].y;
				break;
				case 5:
					vector3s[4].x=vector3s[0].x;
					vector3s[4].z=vector3s[0].z;
				break;
				case 6:
					vector3s[4].y=vector3s[0].y;
					vector3s[4].z=vector3s[0].z;
				break;
			}
			
			aTransform.localScale = vector3s[4];
						
			if(cs != null){
				if(cs.cancel == true) {
					smoothOut-= Time.deltaTime;
					time = aSmoothTime + ((1f-smoothOut)*3);
					if(smoothOut < 0f) break;
				}
			}
			
			if(timed) {
				if(duration > 0f) { 
					yield return null;
				} else {
					smoothOut-= Time.deltaTime;
					scaleTime = aSmoothTime + ((1f-smoothOut)*3);
					if(smoothOut < 0f){
						break;	
					} else {
						yield return null;	
					}
				}
			} else {
				yield return null;
			}
		}
		
		if(onCompleted != null) onCompleted();
		
		cs.id = "";
		cs.cancel = false;
		cs.paused = false;
		cs.free = true;
	}
	
	#endregion
	
}
