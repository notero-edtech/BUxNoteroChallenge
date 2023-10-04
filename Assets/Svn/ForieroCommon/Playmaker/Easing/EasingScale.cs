// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Easing")]
	[Tooltip("Easing Scale for TO,FROM and ADD position transitions.")]
	public class EasingScale : FsmStateAction
	{
		public enum ScaleEnum{
			ScaleTo,
			ScaleAdd,
			ScaleFrom
		}
		
		public FsmOwnerDefault owner;
		[Tooltip("Easing identifier. Don't set it if you do not want to control this Easing with Easing Pause, Easing Resume or Easing Cancel.")]
		public FsmString id;
		public FsmString category;
		[Tooltip("Scaly Type.")]
		public ScaleEnum scaleType;
		public FsmGameObject targetObject;
		[Tooltip("Target Vector of an offset from a Target Object.")]
		public FsmVector3 vectorValue;
		
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
			category = "Scale";
			targetObject = new FsmGameObject{UseVariable = true};
			vectorValue = new FsmVector3{UseVariable = true};
			duration  = new FsmFloat{ Value = 1f};
			delay = new FsmFloat{UseVariable = true};
			ease = UnityExtensions.EaseType.easeInOutSine;
			startedEvent = null;
			finishedEvent = null;
			scaleType = ScaleEnum.ScaleTo;
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
				switch(scaleType){
				case ScaleEnum.ScaleAdd:
					vct = go.transform.localScale + (vectorValue.IsNone ? Vector3.zero : vectorValue.Value);
				break;
				case ScaleEnum.ScaleFrom:
					vct = go.transform.localScale;
					if(targetObject.IsNone){
						go.transform.localScale = (vectorValue.IsNone ? Vector3.zero : vectorValue.Value);
					} else {
						go.transform.localScale = targetObject.Value.transform.localScale + (vectorValue.IsNone ? Vector3.zero : vectorValue.Value);
					}
				break;
				case ScaleEnum.ScaleTo :
					if(targetObject.IsNone){
						vct = (vectorValue.IsNone ? Vector3.zero : vectorValue.Value);
					} else {
						vct = targetObject.Value.transform.localScale + (vectorValue.IsNone ? Vector3.zero : vectorValue.Value);
					}
				break;
				}
				Fsm.Owner.StartCoroutine(go.transform.EaseScale(
				                                                   	duration.IsNone ? 1f : duration.Value,
				                                                   	delay.IsNone ? 0f : delay.Value,
				                                                   	vct,
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