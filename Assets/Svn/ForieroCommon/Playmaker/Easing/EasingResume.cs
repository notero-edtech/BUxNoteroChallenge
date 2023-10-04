// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Easing")]
	[Tooltip("Resumes both background Easings or active action Easings.")]
	public class EasingResume : FsmStateAction
	{
		[Tooltip("Easing identifier.")]
		public FsmString id;
		[Tooltip("Pause all Easings with ID substring.")]
		public bool contains;
		[Tooltip("Easing Category")]
		public FsmString category;
		
		public override void Reset (){
			base.Reset();
			id = new FsmString{UseVariable = true};
			contains = false;
			category = new FsmString{UseVariable = true};
		}
		                   
		
		public override void OnEnter ()
		{
			base.OnEnter();
			UnityExtensions.ResumeAnimateTo(id.IsNone ? "" : id.Value, contains, category.IsNone ? "" : category.Value);
			Finish();
		}
		
		public override void OnExit (){
			base.OnExit();
		}	
	}
}