// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Easing")]
	[Tooltip("Cancel both background Easings or active action easings.")]
	public class EasingCancel : FsmStateAction
	{
		[Tooltip("Easing identifier.")]
		public FsmString id;
		[Tooltip("Cancel all Easings with ID substring.")] 
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
			UnityExtensions.CancelAnimateTo(id.IsNone ? "" : id.Value, contains, category.IsNone ? "" : category.Value);
			Finish();
		}
		
		public override void OnExit (){
			base.OnExit();
		}	
	}
}