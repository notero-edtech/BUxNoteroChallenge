// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Easing")]
	[Tooltip("Smooth look TO or FROM a vector3.")]
	public class EasingLook : FsmStateAction
	{
		public enum LookEnum{
			LookTo,
			LookFrom
		}
		
		public FsmOwnerDefault owner;
		[Tooltip("Easing identifier. Don't set it if you do not want to control this Easing with Easing Pause, Easing Resume or Easing Cancel.")]
		public FsmString id;
		public FsmString category;
		[Tooltip("Look Type.")]
		public LookEnum lookType;
		public FsmGameObject targetObject;
		[Tooltip("Vector3 target.")]
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
				
		string id_param = "";
		GameObject go;
		Vector3 vct = Vector3.zero;
		bool finished = false;
		
		public override void Reset (){
			base.Reset();
			id = new FsmString{UseVariable = true};
			category = "Look";
			targetObject = new FsmGameObject{UseVariable = true};
			vectorValue = new FsmVector3{UseVariable = true};
			duration  = new FsmFloat{ Value = 1f};
			delay = new FsmFloat{UseVariable = true};
			ease = UnityExtensions.EaseType.easeInOutSine;
			startedEvent = null;
			finishedEvent = null;
			lookType = LookEnum.LookTo;
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
				switch(lookType){
				case LookEnum.LookFrom:
					if(targetObject.IsNone){
						vct = go.transform.eulerAngles;
						go.transform.LookAt(vectorValue.IsNone ? go.transform.forward : vectorValue.Value, Vector3.up);
						var originalEulers = go.transform.eulerAngles;
						vct = new Vector3(UnityExtensions.clerp(originalEulers.x,vct.x,1),UnityExtensions.clerp(originalEulers.y,vct.y,1),UnityExtensions.clerp(originalEulers.z,vct.z,1));
					} else {
						vct = go.transform.eulerAngles;
						go.transform.LookAt(targetObject.Value.transform, Vector3.up);
						go.transform.eulerAngles+= vectorValue.IsNone ? Vector3.zero : vectorValue.Value;
						var originalEulers = go.transform.eulerAngles;
						vct = new Vector3(UnityExtensions.clerp(originalEulers.x,vct.x,1),UnityExtensions.clerp(originalEulers.y,vct.y,1),UnityExtensions.clerp(originalEulers.z,vct.z,1));
					}
				break;
				case LookEnum.LookTo:
					if(targetObject.IsNone){
						var originalEulers = go.transform.eulerAngles;
						go.transform.LookAt(vectorValue.IsNone ? go.transform.forward : vectorValue.Value, Vector3.up);
						vct = go.transform.eulerAngles;
						vct = new Vector3(UnityExtensions.clerp(originalEulers.x,vct.x,1),UnityExtensions.clerp(originalEulers.y,vct.y,1),UnityExtensions.clerp(originalEulers.z,vct.z,1));
						go.transform.eulerAngles = originalEulers;
					} else {
						var originalEulers = go.transform.eulerAngles;
						go.transform.LookAt(targetObject.Value.transform, Vector3.up);
						go.transform.eulerAngles+= vectorValue.IsNone ? Vector3.zero : vectorValue.Value;
						vct = go.transform.eulerAngles;
						vct = new Vector3(UnityExtensions.clerp(originalEulers.x,vct.x,1),UnityExtensions.clerp(originalEulers.y,vct.y,1),UnityExtensions.clerp(originalEulers.z,vct.z,1));
						go.transform.eulerAngles = originalEulers;
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