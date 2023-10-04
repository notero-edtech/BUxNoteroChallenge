// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Easing")]
	[Tooltip("Easing Move for TO,FROM and ADD position transitions.")]
	public class EasingMove : FsmStateAction
	{
		public enum MoveEnum{
			MoveTo,
			MoveAdd,
			MoveFrom
		}
		
		public FsmOwnerDefault owner;
		[Tooltip("Easing identifier. Don't set it if you do not want to control this Easing with Easing Pause, Easing Resume or Easing Cancel.")]
		public FsmString id;
		public FsmString category;
		[Tooltip("Move Type.")]
		public MoveEnum moveType;
		public FsmGameObject targetObject;
		[Tooltip("Target vector or an offset from a Target Object.")]
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
			category = "Move";
			targetObject = new FsmGameObject{UseVariable = true};
			vectorValue = new FsmVector3{UseVariable = true};
			duration  = new FsmFloat{ Value = 1f};
			delay = new FsmFloat{UseVariable = true};
			ease = UnityExtensions.EaseType.easeInOutSine;
			startedEvent = null;
			finishedEvent = null;
			moveType = MoveEnum.MoveTo; 
			loopType = UnityExtensions.LoopType.None;
			reverse = new FsmBool{UseVariable = true};
			space = Space.Self;
			go = null;
		}
		                   
		
		public override void OnEnter ()
		{
			base.OnEnter();
			finished = false;
			id_param = id.IsNone ? UnityExtensions.GUID() : id.Value;
			go = Fsm.GetOwnerDefaultTarget(owner);
			if(go){
				switch(moveType){
				case MoveEnum.MoveAdd :
					switch(space){
					case Space.Self : 
						var pos = go.transform.position;
						go.transform.Translate(vectorValue.IsNone ? Vector3.zero : vectorValue.Value, space);
						vct = go.transform.localPosition;
						go.transform.position = pos;						
					break;
					case Space.World : 
						vct = go.transform.position + (vectorValue.IsNone ? Vector3.zero : vectorValue.Value);
					break;
					}
				break;
				case MoveEnum.MoveFrom:
					switch(space){
					case Space.Self:
						vct = go.transform.localPosition;
						if(targetObject.IsNone){
							go.transform.localPosition = vectorValue.IsNone ? Vector3.zero : vectorValue.Value;	
						} else {
						 	go.transform.position = targetObject.Value.transform.InverseTransformPoint((targetObject.IsNone ? Vector3.zero : targetObject.Value.transform.position) + (vectorValue.IsNone ? Vector3.zero : vectorValue.Value));	
						}
						
					break;
					case Space.World:
						vct = go.transform.position;
						go.transform.position = (targetObject.IsNone ? Vector3.zero : targetObject.Value.transform.position) + (vectorValue.IsNone ? Vector3.zero : vectorValue.Value);
					break;
					}
					
				break;
				case MoveEnum.MoveTo:
					switch(space){
					case Space.Self:
						if(targetObject.IsNone){
							vct = (vectorValue.IsNone ? Vector3.zero : vectorValue.Value);
						} else {
							var pos = go.transform.position;
							go.transform.position  = targetObject.Value.transform.position + (vectorValue.IsNone ? Vector3.zero : vectorValue.Value);						
							vct = go.transform.localPosition;
							go.transform.position = pos;
						}
					break;
					case Space.World:
						vct = targetObject.IsNone ? (vectorValue.IsNone ? Vector3.zero : vectorValue.Value) : (targetObject.Value.transform.position) + (vectorValue.IsNone ? Vector3.zero : vectorValue.Value);
					break;	
					}
				break;
				}
				       
				Fsm.Owner.StartCoroutine(go.transform.EasePosition(
				                                                   	duration.IsNone ? 1f : duration.Value,
				                                                   	delay.IsNone ? 0f : delay.Value,
				                                                   	vct,
				                                                    space,
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