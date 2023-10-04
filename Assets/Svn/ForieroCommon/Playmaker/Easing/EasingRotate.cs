// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Easing")]
	[Tooltip("Easing Rotate for TO,FROM and ADD rotation transitions.")]
	public class EasingRotate : FsmStateAction
	{
		public enum RotateEnum{
			RotateTo,
			RotateAdd,
			RotateFrom
		}
				
		public FsmOwnerDefault owner;
		[Tooltip("Easing identifier. Don't set it if you do not want to control this Easing with Easing Pause, Easing Resume or Easing Cancel.")]
		public FsmString id;
		public FsmString category;
		[Tooltip("Rotate Type.")]
		public RotateEnum rotateType;
		
		public FsmGameObject targetObject;
		[Tooltip("Target Vector or an offset from a Target Object.")]
		public FsmVector3 vectorValue;
		public Space space;
		
		[ActionSection("Time")]
		[Tooltip("Easing duration time.")]
		public FsmFloat duration;
		[Tooltip("Delayed start.")]
		public FsmFloat delay;
		
		[Tooltip("Easing Type Effect.")]
		public UnityExtensions.EaseType ease = UnityExtensions.EaseType.easeInOutSine;
		public UnityExtensions.LoopType loopType;
		[Tooltip("Reverse Easing process.")]
		public FsmBool reverse;
				
		[ActionSection("Events")]
		public FsmEvent startedEvent;
		public FsmEvent finishedEvent;
		//[Tooltip("If checked the easing will end when a state is exited.")]
		//public bool stopOnExit = true;
				
		string id_param = "";
		GameObject go;
		Vector3 vct = Vector3.zero;
		bool finished = false;
		
		public override void Reset (){
			base.Reset();
			id = new FsmString{UseVariable = true};
			category = "Rotate";
			targetObject = new FsmGameObject{UseVariable = true};
			vectorValue = new FsmVector3{UseVariable = true};
			duration  = new FsmFloat{ Value = 1f};
			delay = new FsmFloat{UseVariable = true};
			ease = UnityExtensions.EaseType.easeInOutSine;
			startedEvent = null;
			finishedEvent = null;
			rotateType = RotateEnum.RotateTo;
			space = Space.Self;
			loopType = UnityExtensions.LoopType.None;
			reverse = new FsmBool{UseVariable = true};
			go = null;
		}
		                   
		
		public override void OnEnter ()
		{
			base.OnEnter();
			finished = false;
			id_param = id.IsNone ? UnityExtensions.GUID() : id.Value;
			go = Fsm.GetOwnerDefaultTarget(owner);
			if(go){
				var originalEulers = Vector3.zero;
				switch(rotateType){
				case RotateEnum.RotateAdd :
					switch(space){
					case Space.Self : 
						vct = go.transform.localEulerAngles + (vectorValue.IsNone ? Vector3.zero : vectorValue.Value);
						originalEulers = go.transform.eulerAngles;
						
						vct = new Vector3(UnityExtensions.clerp(originalEulers.x,vct.x,1),UnityExtensions.clerp(originalEulers.y,vct.y,1),UnityExtensions.clerp(originalEulers.z,vct.z,1));	
					break;
					case Space.World : 
						vct = go.transform.eulerAngles + (vectorValue.IsNone ? Vector3.zero : vectorValue.Value);
						originalEulers = go.transform.eulerAngles;
						vct = new Vector3(UnityExtensions.clerp(originalEulers.x,vct.x,1),UnityExtensions.clerp(originalEulers.y,vct.y,1),UnityExtensions.clerp(originalEulers.z,vct.z,1));
					break;
					}
					
				break;
				case RotateEnum.RotateFrom:
					switch(space){
					case Space.Self:
						if(targetObject.IsNone){
							vct = go.transform.localEulerAngles;
							go.transform.localEulerAngles = vectorValue.IsNone ? Vector3.zero : vectorValue.Value;
							originalEulers = vectorValue.IsNone ? Vector3.zero : vectorValue.Value;
						    vct = new Vector3(UnityExtensions.clerp(originalEulers.x,vct.x,1),UnityExtensions.clerp(originalEulers.y,vct.y,1),UnityExtensions.clerp(originalEulers.z,vct.z,1));
						} else {
							vct = go.transform.localEulerAngles;
							go.transform.eulerAngles = targetObject.Value.transform.eulerAngles + (vectorValue.IsNone ? Vector3.zero : vectorValue.Value);
							originalEulers = go.transform.localEulerAngles;
						    vct = new Vector3(UnityExtensions.clerp(originalEulers.x,vct.x,1),UnityExtensions.clerp(originalEulers.y,vct.y,1),UnityExtensions.clerp(originalEulers.z,vct.z,1));
						}
					break;
					case Space.World:
						if(targetObject.IsNone){
							vct = go.transform.eulerAngles;
							go.transform.eulerAngles = vectorValue.IsNone ? Vector3.zero : vectorValue.Value;;
							originalEulers = vectorValue.IsNone ? Vector3.zero : vectorValue.Value;;
						   	vct = new Vector3(UnityExtensions.clerp(originalEulers.x,vct.x,1),UnityExtensions.clerp(originalEulers.y,vct.y,1),UnityExtensions.clerp(originalEulers.z,vct.z,1));
						} else {
							vct = go.transform.eulerAngles;
							go.transform.eulerAngles = targetObject.Value.transform.eulerAngles + (vectorValue.IsNone ? Vector3.zero : vectorValue.Value);
							originalEulers = go.transform.eulerAngles;
						   	vct = new Vector3(UnityExtensions.clerp(originalEulers.x,vct.x,1),UnityExtensions.clerp(originalEulers.y,vct.y,1),UnityExtensions.clerp(originalEulers.z,vct.z,1));
						}
					break;
					}
				break;
				case RotateEnum.RotateTo:
					switch(space){
					case Space.Self:
						if(targetObject.IsNone){
							vct = (vectorValue.IsNone ? Vector3.zero : vectorValue.Value);
							originalEulers = go.transform.localEulerAngles;
							vct = new Vector3(UnityExtensions.clerp(originalEulers.x,vct.x,1),UnityExtensions.clerp(originalEulers.y,vct.y,1),UnityExtensions.clerp(originalEulers.z,vct.z,1));
						} else {
							originalEulers = go.transform.eulerAngles;
							go.transform.eulerAngles = targetObject.Value.transform.eulerAngles + (vectorValue.IsNone ? Vector3.zero : vectorValue.Value);
							vct = go.transform.localEulerAngles;
							go.transform.eulerAngles = originalEulers;
							originalEulers = go.transform.localEulerAngles;
							vct = new Vector3(UnityExtensions.clerp(originalEulers.x,vct.x,1),UnityExtensions.clerp(originalEulers.y,vct.y,1),UnityExtensions.clerp(originalEulers.z,vct.z,1));
						}
					break;
					case Space.World:
						if(targetObject.IsNone){
							vct = vectorValue.IsNone ? Vector3.zero : vectorValue.Value;
							originalEulers = go.transform.eulerAngles;
							vct = new Vector3(UnityExtensions.clerp(originalEulers.x,vct.x,1),UnityExtensions.clerp(originalEulers.y,vct.y,1),UnityExtensions.clerp(originalEulers.z,vct.z,1));
						} else {
							vct = targetObject.Value.transform.eulerAngles + (vectorValue.IsNone ? Vector3.zero : vectorValue.Value);
							originalEulers = go.transform.eulerAngles;
							vct = new Vector3(UnityExtensions.clerp(originalEulers.x,vct.x,1),UnityExtensions.clerp(originalEulers.y,vct.y,1),UnityExtensions.clerp(originalEulers.z,vct.z,1));
						}
					break;
					}
				break;
				}
				       
				Fsm.Owner.StartCoroutine(go.transform.EaseRotation(
				                                                   	duration.IsNone ? 1f : duration.Value,
				                                                   	delay.IsNone ? 0f : delay.Value,
				                                                   	vct,
				                                                    space,
				                                                    (int)UnityExtensions.AxisConstraints.none,
				                                                   	UnityExtensions.GetEasingFunction(ease),
				                                                   	() => {if(startedEvent != null) Fsm.Event(startedEvent);},
																   	() => {if(finishedEvent != null) Fsm.Event(finishedEvent); finished = true; Finish();},
																	loopType,
																	reverse.IsNone ? false : reverse.Value,
																	category.IsNone ? "" : category.Value,
																	id_param));
			}
		}
		
		public override void OnExit (){
			base.OnExit();
			if(!finished) UnityExtensions.CancelAnimateTo(id_param, false, category.IsNone ? "" : category.Value);
		}	
	}
}