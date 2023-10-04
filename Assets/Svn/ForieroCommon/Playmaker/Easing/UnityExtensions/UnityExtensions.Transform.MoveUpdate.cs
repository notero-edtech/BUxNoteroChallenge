using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static partial class UnityExtensions {
	
	#region MoveUpdate

	public static IEnumerator MoveUpdate(this Transform aTransform,
	                                 float aDuration,
	                                 float aDelay,    
	                                 float aSmoothTime,
	                                 Transform aTarget,
	                                 Vector3 anOffset,
	                                 Space aSpace,
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
		
		float moveTime = aSmoothTime;
		float smoothOut = 0.8f;
		float smoothIn = 0.8f;
		Vector3[] vector3s = new Vector3[5];
		Vector3 preUpdate = aTransform.position;
		Vector3 postUpdate = aTransform.position;
		
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
				moveTime = aSmoothTime + (smoothIn*2f);	
			}
			//init values:
			switch(aSpace){
			case Space.Self:
				vector3s[0] = aTransform.localPosition;
			break;
			case Space.World:
				vector3s[0] = aTransform.position;	
			break;
			}
			
			vector3s[1]= aTarget.position + anOffset;			
			if(!cs.paused){
				//calculate:
				vector3s[3].x=Mathf.SmoothDamp(vector3s[0].x,vector3s[1].x,ref vector3s[2].x,moveTime);
				vector3s[3].y=Mathf.SmoothDamp(vector3s[0].y,vector3s[1].y,ref vector3s[2].y,moveTime);
				vector3s[3].z=Mathf.SmoothDamp(vector3s[0].z,vector3s[1].z,ref vector3s[2].z,moveTime);
			}
				
			vector3s[4] = vector3s[3];
						
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
			
			switch(aSpace){
			case Space.Self:
				aTransform.localPosition = vector3s[4];
			break;
			case Space.World:
				aTransform.position=vector3s[4];		
			break;
			}
						
			//need physics?
			if(aTransform.GetComponent<Rigidbody>() != null){
				postUpdate = aTransform.position;
				aTransform.position = preUpdate;
				aTransform.GetComponent<Rigidbody>().MovePosition(postUpdate);
			}
						
			if(timed) {
				if(duration > 0f) { 
					yield return null;
				} else {
					smoothOut-= Time.deltaTime;
					moveTime = aSmoothTime + ((1f-smoothOut)*3);
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
