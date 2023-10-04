// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Easing")]
	[Tooltip("Special easing effect.")]
	public class EasingPunch : FsmStateAction
	{
		public enum PunchEnum{
			Position,
			Rotation,
			Scale
		}
		
		public FsmOwnerDefault owner;
		[Tooltip("For Pausing, Resuming and Canceling set this id.")]
		public FsmString id;
		public FsmString category;
		[Tooltip("Shake Type.")]
		public PunchEnum punchType;
		public Space space;
		[RequiredField]
		[Tooltip("Vector3 shake.")]
		public FsmVector3 punchVector;
		
		[ActionSection("Time")]
		[Tooltip("Easing duration time.")]
		public FsmFloat duration;
		[Tooltip("Delayed start.")]
		public FsmFloat delay;
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
		bool finished = false;
		
		public override void Reset (){
			base.Reset();
			id = new FsmString{UseVariable = true};
			category = "Punch";
			punchVector = new FsmVector3{UseVariable = true};
			duration  = new FsmFloat{ Value = 1f};
			delay = new FsmFloat{UseVariable = true};
			startedEvent = null;
			finishedEvent = null;
			punchType = PunchEnum.Position;
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
				switch(punchType){
				case PunchEnum.Position :
					Fsm.Owner.StartCoroutine(go.transform.PunchPosition(
						                                            duration.IsNone ? 1f : duration.Value,
						                                            delay.IsNone ? 0f : delay.Value,
				                                                    punchVector.Value,
					                                                space,
				                                                    () => { if(startedEvent != null) Fsm.Event(startedEvent);},
																	() => { if(finishedEvent != null) Fsm.Event(finishedEvent); finished = true; Finish(); },
																	loopType,
																	reverse.IsNone ? false : reverse.Value,
																	category.IsNone ? "" : category.Value,
																	id_param));
				break;
				case PunchEnum.Rotation :
					Fsm.Owner.StartCoroutine(go.transform.PunchRotation(
						                                            duration.IsNone ? 1f : duration.Value,
						                                            delay.IsNone ? 0f : delay.Value,
				                                                    punchVector.Value,
					                                                space,
				                                                    () => { if(startedEvent != null) Fsm.Event(startedEvent);},
																	() => { if(finishedEvent != null) Fsm.Event(finishedEvent); finished = true; Finish(); },
																	loopType,
																	reverse.IsNone ? false : reverse.Value,
																	category.IsNone ? "" : category.Value,
																	id_param));
				break;
				case PunchEnum.Scale :
					Fsm.Owner.StartCoroutine(go.transform.PunchScale(
						                                            duration.IsNone ? 1f : duration.Value,
						                                            delay.IsNone ? 0f : delay.Value,
				                                                    punchVector.Value,
				                                                    () => { if(startedEvent != null) Fsm.Event(startedEvent);},
																	() => { if(finishedEvent != null) Fsm.Event(finishedEvent); finished = true; Finish(); },
																	loopType,
																	reverse.IsNone ? false : reverse.Value,
																	category.IsNone ? "" : category.Value,
																	id_param));
				break;
				}
			}
		}
		
		public override void OnExit (){
			base.OnExit();
			if(!finished) UnityExtensions.CancelAnimateTo(id_param, false, category.IsNone ? "" : category.Value);
		}	
	}
}