// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Easing")]
	[Tooltip("Follows an object rotation.")]
	public class EasingRotateUpdate : FsmStateAction
	{
		public FsmOwnerDefault owner;
		[Tooltip("Easing identifier.")]
		public FsmString id;
		public FsmString category;
		[Tooltip("GameObject target.")]	
		public FsmGameObject target;
		public FsmVector3 offset;
		
		[ActionSection("Time")]
		[Tooltip("Easing duration time.")]
		public FsmFloat duration;
		[Tooltip("Delayed start.")]
		public FsmFloat delay;
		[Tooltip("How fast will the rotation react to a change. 0 - immediate, 1 - it will take a 1s to complete the rotation to a new target position.")]
		public FsmFloat smoothTime;
		
		[ActionSection("Events")]
		public FsmEvent startedEvent;
		public FsmEvent finishedEvent;
		//[Tooltip("If checked the easing will end when a state is exited.")]
		//public bool stopOnExit = true;
		
		string id_param = "";
		GameObject go;
		bool finished = false;
		
		public override void Reset (){
			base.Reset();
			smoothTime = 1f;
			target = new FsmGameObject{UseVariable = true};
			id = new FsmString{UseVariable = true};
			category = "RotateUpdate";
			duration  = new FsmFloat{ UseVariable = true};
			delay = new FsmFloat{UseVariable = true};
			startedEvent = null;
			finishedEvent = null;
			offset = new FsmVector3{UseVariable = true};
			go = null;
		}
		                   
		
		public override void OnEnter ()
		{
			base.OnEnter();
			finished = false;
			id_param = id.IsNone ? UnityExtensions.GUID() : id.Value;
			go = Fsm.GetOwnerDefaultTarget(owner);
			if(go){
				Fsm.Owner.StartCoroutine(go.transform.RotateUpdate(
				                                                 duration.IsNone ? 0f : duration.Value,
				                                                 delay.IsNone ? 0f : delay.Value,
				                                                 smoothTime.IsNone ? 1f : smoothTime.Value,
				                                                 target.Value.transform,
				                                                 offset.IsNone ? Vector3.zero : offset.Value,
				                                                 Space.Self,
				                                                 (int)UnityExtensions.AxisConstraints.none,
				                                                 () => {if(startedEvent != null) Fsm.Event(startedEvent);},
																 () => {if(finishedEvent != null) Fsm.Event(finishedEvent); finished = true; Finish();},  
				                                                 category.IsNone ? "" : category.Value,
				                                                 id_param));
			}
			//if(duration.IsNone || duration.Value == 0f) Finish();
		}
		
		public override void OnExit (){
			base.OnExit();
			if(!finished) UnityExtensions.CancelAnimateTo(id_param, false, category.IsNone ? "" : category.Value);
		}	
	}
}