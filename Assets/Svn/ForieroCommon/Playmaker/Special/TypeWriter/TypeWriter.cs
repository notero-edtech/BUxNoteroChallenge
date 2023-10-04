using UnityEngine;
using System;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Foriero")]
	[Tooltip("TypeWriter.")]
	public class TypeWriter : FsmStateAction
	{
			
		[RequiredField]
		[Tooltip("The Script to add to the Game Object.")]
		[UIHint(UIHint.ScriptComponent)]
		public FsmString input;
		
		[RequiredField]
		[Tooltip("The Script to add to the Game Object.")]
		[UIHint(UIHint.ScriptComponent)]
		public FsmString output;
		
		[RequiredField]
		[Tooltip("The Script to add to the Game Object.")]
		[UIHint(UIHint.ScriptComponent)]
		public FsmInt counter;
		
		[RequiredField]
		[Tooltip("The Script to add to the Game Object.")]
		[UIHint(UIHint.ScriptComponent)]
		public FsmFloat time;
		
		[RequiredField]
		public FsmAnimationCurve animCurve;
				
		public FsmEvent nextCharEvent;
		public FsmEvent finishedEvent;

		public override void Reset()
		{
			input = new FsmString{UseVariable = true};
			output = new FsmString{UseVariable = true};
			counter = new FsmInt{UseVariable = true};
			time = new FsmFloat{UseVariable = true};
			animCurve = null;
			nextCharEvent = null;
			finishedEvent = null;
		}

		public override void OnEnter()
		{
			counter.Value++;
			if(counter.Value < input.Value.Length) {
				if(animCurve != null){
					float elapsedTime = ((float)counter.Value/(float)input.Value.Length)*time.Value;
					float unitTime = (1f/(float)input.Value.Length)*time.Value;
					float waitTime = unitTime*animCurve.curve.Evaluate(animCurve.curve.keys[0].time + (elapsedTime/time.Value)*(animCurve.curve.keys[animCurve.curve.keys.Length -1].time - animCurve.curve.keys[0].time));
					Fsm.Owner.StartCoroutine(Wait(waitTime));
				} else {
					Fsm.Owner.StartCoroutine(Wait(0.15f));
				}
			} else {
				counter.Value = 0;
				if(finishedEvent != null) Fsm.Event(finishedEvent);
				Finish();
			}
		}
		
		IEnumerator Wait(float aTime){
			yield return new WaitForSeconds(aTime);
			if(counter.Value < input.Value.Length){
				output.Value = input.Value.Substring(0,counter.Value + 1);
			 	if(nextCharEvent != null) Fsm.Event(nextCharEvent);
			} else {
				if(finishedEvent != null) Fsm.Event(finishedEvent);
				Finish();
			}
		}
		
		public override void OnExit()
		{
			
		}

	}
}