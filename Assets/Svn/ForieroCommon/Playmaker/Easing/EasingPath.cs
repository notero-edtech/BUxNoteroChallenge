// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory ("Easing")]
	[Tooltip ("Easing Path for moving an object along a path.")]
	public class EasingPath : FsmStateAction
	{
		public FsmOwnerDefault owner;
		[Tooltip ("Easing identifier. Don't set it if you do not want to control this Easing with Easing Pause, Easing Resume or Easing Cancel.")]
		public FsmString id;
		public FsmString category;
		public Space space;
		
		[ActionSection ("Time")]
		[Tooltip ("Easing duration time.")]
		public FsmFloat duration;
		[Tooltip ("Delayed start.")]
		public FsmFloat delay;
		public FsmFloat smoothTime;
		public FsmFloat lookAheadTime;
		
		[Tooltip ("Easing Type Effect.")]
		public UnityExtensions.EaseType ease = UnityExtensions.EaseType.easeInOutSine;
		public UnityExtensions.LoopType loopType;
		[Tooltip ("Reverse Easing process.")]
		public FsmBool reverse;
				
		[ActionSection ("Events")]
		public FsmEvent startedEvent;
		public FsmEvent finishedEvent;
		//[Tooltip("If checked the easing will end when a state is exited.")]
		//public bool stopOnExit = true;
		
		[ActionSection ("Path")]
		public FsmBool orientToPath;
		public FsmBool moveToPath;
		[CompoundArray ("Path Nodes", "Transform", "Vector")]
		[Tooltip ("A list of objects to draw a Catmull-Rom spline through for a curved animation path.")]
		public FsmGameObject[] transforms;
		[Tooltip ("A list of positions to draw a Catmull-Rom through for a curved animation path. If Transform is defined, this value is added as a local offset.")]
		public FsmVector3[] vectors;
					
		Vector3[] tempVct3;
		string id_param = "";
		GameObject go;
		bool finished = false;
			
		//		public override void OnDrawGizmos(){
		//			if(transforms.Length >= 2) {
		//				tempVct3 = new Vector3[transforms.Length];
		//				for(int i = 0;i<transforms.Length;i++){
		//					if(transforms[i].IsNone) tempVct3[i] = vectors[i].IsNone ? Vector3.zero : vectors[i].Value;
		//					else {
		//						if(transforms[i].Value == null) tempVct3[i] = vectors[i].IsNone ? Vector3.zero : vectors[i].Value;
		//						else tempVct3[i] = transforms[i].Value.transform.position + (vectors[i].IsNone ? Vector3.zero : vectors[i].Value);
		//					}
		//				}
		//				UnityExtensions.DrawPathGizmos(tempVct3, Color.yellow);
		//			}
		//		}
		
		public override void Reset ()
		{
			base.Reset ();
			id = new FsmString{ UseVariable = true };
			category = "Path";
			duration = new FsmFloat{ Value = 1f };
			delay = new FsmFloat{ UseVariable = true };
			smoothTime = 0.3f;
			lookAheadTime = 0.05f;
			ease = UnityExtensions.EaseType.easeInOutSine;
			startedEvent = null;
			finishedEvent = null;
			loopType = UnityExtensions.LoopType.None;
			reverse = new FsmBool{ UseVariable = true };
			space = Space.Self;
			transforms = new FsmGameObject[0];
			vectors = new FsmVector3[0];
			tempVct3 = new Vector3[0];
			moveToPath = true;
			orientToPath = false;
			go = null;
			
		}

		
		public override void OnEnter ()
		{
			base.OnEnter ();
			finished = false;
			id_param = id.IsNone ? UnityExtensions.GUID () : id.Value;
			go = Fsm.GetOwnerDefaultTarget (owner);
			if (go) {
				tempVct3 = new Vector3[transforms.Length];
				if (reverse.IsNone ? false : reverse.Value) {
					for (int i = 0; i < transforms.Length; i++) {
						if (transforms [i].IsNone)
							tempVct3 [tempVct3.Length - 1 - i] = vectors [i].IsNone ? Vector3.zero : vectors [i].Value;
						else {
							if (transforms [i].Value == null)
								tempVct3 [tempVct3.Length - 1 - i] = vectors [i].IsNone ? Vector3.zero : vectors [i].Value;
							else
								tempVct3 [tempVct3.Length - 1 - i] = transforms [i].Value.transform.position + (vectors [i].IsNone ? Vector3.zero : vectors [i].Value);
						}
					}
				} else {
					for (int i = 0; i < transforms.Length; i++) {
						if (transforms [i].IsNone)
							tempVct3 [i] = vectors [i].IsNone ? Vector3.zero : vectors [i].Value;
						else {
							if (transforms [i].Value == null)
								tempVct3 [i] = vectors [i].IsNone ? Vector3.zero : vectors [i].Value;
							else
								tempVct3 [i] = transforms [i].Value.transform.position + (vectors [i].IsNone ? Vector3.zero : vectors [i].Value);
						}
					}
				}
				Fsm.Owner.StartCoroutine (go.transform.EasePath (
					duration.IsNone ? 1f : duration.Value,
					delay.IsNone ? 0f : delay.Value,
					smoothTime.IsNone ? 0.3f : smoothTime.Value,
					lookAheadTime.IsNone ? 0.05f : lookAheadTime.Value,
					tempVct3,
					moveToPath.IsNone ? true : moveToPath.Value,
					orientToPath.IsNone ? false : orientToPath.Value,
					space,
					UnityExtensions.GetEasingFunction (ease),
					() => {
						if (startedEvent != null)
							Fsm.Event (startedEvent);
					},
					() => {
						if (finishedEvent != null)
							Fsm.Event (finishedEvent);
						finished = true;
						Finish ();
					},
					loopType,
					false,
					category.IsNone ? "" : category.Value,
					id_param));
			} else {
				Finish ();	
			}
		}

		public override void OnExit ()
		{
			base.OnExit ();
			if (!finished)
				UnityExtensions.CancelAnimateTo (id_param, false, category.IsNone ? "" : category.Value);
		}
		
	}
}