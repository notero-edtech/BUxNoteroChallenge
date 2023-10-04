using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static partial class UnityExtensions {
	
	#region LookUpdate
			
	public static IEnumerator LookUpdate(this Transform aTransform,
	                                 float aDuration,
	                                 float aDelay,
	                                 float aSmoothTime,
	                                 Transform aTarget,
	                                 Vector3 anOffset,
	                                 bool aFollowCursor,
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
		
		float lookTime = aSmoothTime;
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
				lookTime = aSmoothTime + (smoothIn*2f);	
			}
			//from values:
			vector3s[0] = aTransform.eulerAngles;
			
			if(aFollowCursor){ 
				if(Camera.main.orthographic)
					aTransform.LookAt(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, Camera.main.transform.position.z)));	
				else	
				 	aTransform.LookAt(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width - Input.mousePosition.x,Screen.height - Input.mousePosition.y, Camera.main.transform.position.z)));
			} else {
				if(aTarget == null) break;
				aTransform.LookAt(aTarget, Vector3.up);
			}
			
			//to values and reset look:
			vector3s[1]= aTransform.eulerAngles + anOffset;
			aTransform.eulerAngles=vector3s[0];
			
			if(!cs.paused){
				//calculate:
				vector3s[3].x=Mathf.SmoothDampAngle(vector3s[0].x,vector3s[1].x,ref vector3s[2].x,lookTime);
				vector3s[3].y=Mathf.SmoothDampAngle(vector3s[0].y,vector3s[1].y,ref vector3s[2].y,lookTime);
				vector3s[3].z=Mathf.SmoothDampAngle(vector3s[0].z,vector3s[1].z,ref vector3s[2].z,lookTime);
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
			
			//apply axis restriction:
			aTransform.eulerAngles=vector3s[4];
						
			if(cs != null){
				if(cs.cancel == true) {
					smoothOut-= Time.deltaTime;
					lookTime = aSmoothTime + ((1f-smoothOut)*3);
					if(smoothOut < 0f) break;	
				}
			}
			
			if(timed) {
				if(duration > 0f) { 
					yield return null;
				} else {
					smoothOut-= Time.deltaTime;
					lookTime = aSmoothTime + ((1f-smoothOut)*3);
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
