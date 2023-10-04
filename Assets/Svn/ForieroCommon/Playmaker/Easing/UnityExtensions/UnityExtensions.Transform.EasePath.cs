using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static partial class UnityExtensions {
	
	#region MoveUpdate

	public static IEnumerator EasePath(this Transform aTransform,
	                                 float aDuration,
	                                 float aDelay,
	                                 float aSmoothTime,
	                                 float aLookAhead,
	                                 Vector3[] aPath,
	                                 bool aMoveToPath,
	                                 bool anOrientToPath,
	                                 Space aSpace,
	                                 System.Func<float, float, float, float> anEaseFunc,
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
		
		var suppliedPath = aPath;
		
		//do we need to plot a path to get to the beginning of the supplied path?		
		bool plotStart;
		int offset;
		if( aTransform.position != suppliedPath[0]){
			if(aMoveToPath){
				plotStart=true;
				offset=3;	
			}else{
				plotStart=false;
				offset=2;
			}
		}else{
			plotStart=false;
			offset=2;
		}				

		//build calculated path:
		Vector3[] vector3s = new Vector3[suppliedPath.Length+offset];
		if(plotStart){
			vector3s[1]= aTransform.position;
			offset=2;
		}else{
			offset=1;
		}		
		
		//populate calculate path;
		Array.Copy(suppliedPath,0,vector3s,offset,suppliedPath.Length);
		
		//populate start and end control points:
		//vector3s[0] = vector3s[1] - vector3s[2];
		vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
		vector3s[vector3s.Length-1] = vector3s[vector3s.Length-2] + (vector3s[vector3s.Length-2] - vector3s[vector3s.Length-3]);
		
		//is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
		if(vector3s[1] == vector3s[vector3s.Length-2]){
			Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
			Array.Copy(vector3s,tmpLoopSpline,vector3s.Length);
			tmpLoopSpline[0]=tmpLoopSpline[tmpLoopSpline.Length-3];
			tmpLoopSpline[tmpLoopSpline.Length-1]=tmpLoopSpline[2];
			vector3s=new Vector3[tmpLoopSpline.Length];
			Array.Copy(tmpLoopSpline,vector3s,tmpLoopSpline.Length);
		}
		
		//create Catmull-Rom path:
		CRSpline path = new CRSpline(vector3s);
				
		Vector3 preUpdate = Vector3.zero;
		Vector3 postUpdate = Vector3.zero;
		float timeClamp01 = 0f;
		float t = 0f;
		
		//LOOK UPDATE INIT
		int aConstraints = 0;
		float smoothTime = 0f;
//		float smoothOut = 0.8f;
		float smoothIn = 0.8f;
		float tLook = 1f;
		Vector3[] vector3sLook = new Vector3[5];
		Vector3 lookTarget = Vector3.zero;
				
		if(onStarted != null) onStarted();
		
		while(duration >= 0f)
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
			
			preUpdate = aTransform.position;
			timeClamp01 = aReverse ? 1f - Mathf.Clamp01(time/aDuration) : Mathf.Clamp01(time/aDuration);
			t = anEaseFunc(0,1,timeClamp01);
			
			
			//clamp easing equation results as "back" will fail since overshoots aren't handled in the Catmull-Rom interpolation:
			switch(aSpace){
			case Space.Self: aTransform.localPosition=path.Interp(Mathf.Clamp(t,0,1));break;
			case Space.World: aTransform.position=path.Interp(Mathf.Clamp(t,0,1));break;	
			}
		
			//need physics?
			
			if(aTransform.GetComponent<Rigidbody>()){
				postUpdate = aTransform.position;
				aTransform.position = preUpdate;
				aTransform.GetComponent<Rigidbody>().MovePosition(postUpdate);
			}
		
			if(anOrientToPath){
				
				tLook = anEaseFunc(0,1, Mathf.Min(1f, aReverse ? timeClamp01-aLookAhead : timeClamp01 + aLookAhead)); 
				
				//locate new leading point with a clamp as stated above:
				//Vector3 lookDistance = path.Interp(Mathf.Clamp(tLook,0,1)) - transform.position;
				lookTarget = path.Interp(Mathf.Clamp(tLook,0,1));
								
				if(smoothIn > 0f){
					smoothIn-= Time.deltaTime;
					smoothTime = aSmoothTime + (smoothIn*2f);	
				}
				//from values:
				vector3sLook[0] = aTransform.eulerAngles;
				
				aTransform.LookAt(lookTarget, Vector3.up);
				
				//to values and reset look:
				vector3sLook[1]= aTransform.eulerAngles;
				aTransform.eulerAngles=vector3sLook[0];
				
				//calculate:
				vector3sLook[3].x=Mathf.SmoothDampAngle(vector3sLook[0].x,vector3sLook[1].x,ref vector3sLook[2].x,smoothTime);
				vector3sLook[3].y=Mathf.SmoothDampAngle(vector3sLook[0].y,vector3sLook[1].y,ref vector3sLook[2].y,smoothTime);
				vector3sLook[3].z=Mathf.SmoothDampAngle(vector3sLook[0].z,vector3sLook[1].z,ref vector3sLook[2].z,smoothTime);
			
				//apply:
				aTransform.eulerAngles=vector3sLook[3];
				
				//axis restriction:
				vector3sLook[4]= aTransform.eulerAngles;
				switch(aConstraints){
					case 1:
						vector3sLook[4].x=vector3sLook[0].x;
					break;
					case 2:
						vector3sLook[4].y=vector3sLook[0].y;
					break;
					case 4:
						vector3sLook[4].z=vector3sLook[0].z;
					break;
					case 3:
						vector3sLook[4].x=vector3sLook[0].x;
						vector3sLook[4].y=vector3sLook[0].y;
					break;
					case 5:
						vector3sLook[4].x=vector3sLook[0].x;
						vector3sLook[4].z=vector3sLook[0].z;
					break;
					case 6:
						vector3sLook[4].y=vector3sLook[0].y;
						vector3sLook[4].z=vector3sLook[0].z;
					break;
				}
				
				//apply axis restriction:
				aTransform.eulerAngles=vector3sLook[4];
			}
								
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
