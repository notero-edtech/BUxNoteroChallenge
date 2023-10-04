using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Foriero")]
	[Tooltip("Add Star?")]
	public class PixelToWorld: FsmStateAction
	{
		public enum AxesZero{ BottomCenter, BottomRight, BottomLeft, UpperCenter, UpperRight, UpperLeft, RightCenter, LeftCenter, Center };
		
		[RequiredField]
	    [UIHint(UIHint.Variable)]
		public FsmVector3 storeValue;
		public AxesZero axesZero;
		[Tooltip("x, y for screen point and z for depth or better for distance from camera.")]
		public FsmVector3 screenPoint;
		public bool normalized;
		
		public override void Reset()
		{
			storeValue = null;
			axesZero = AxesZero.UpperLeft;
			screenPoint = null;
		}

		public override void OnEnter()
		{
			
			Vector3 screenAxes = GetScreenVector();
			Vector3 vct = screenPoint.IsNone ? Vector3.zero : screenPoint.Value;
			if(normalized){
				vct = screenAxes + new Vector3(vct.x * Screen.width, vct.y * Screen.height, vct.z);	
				vct = Camera.main.ScreenToWorldPoint(vct);
			} else {
				vct = Camera.main.ScreenToWorldPoint(screenAxes + vct);
			}
			storeValue.Value = vct;
								
			Finish();
		}
		
		Vector3 GetScreenVector(){
			Vector3 result = Vector3.zero;
			switch(axesZero){
			case AxesZero.UpperLeft:
				result = new Vector3(0, Screen.height, 0);
				break;
			case AxesZero.UpperRight:
				result = new Vector3(Screen.width, Screen.height, 0);
				break;
			case AxesZero.UpperCenter:
				result = new Vector3(Screen.width/2f, Screen.height, 0);
				break;
			case AxesZero.LeftCenter:
				result = new Vector3(0, Screen.height/2f, 0);
				break;
			case AxesZero.RightCenter:
				result = new Vector3(Screen.width, Screen.height/2f, 0);
				break;
			case AxesZero.BottomLeft:
				result = new Vector3(0, 0, 0);
				break;
			case AxesZero.BottomRight:
				result = new Vector3(Screen.width, 0, 0);
				break;
			case AxesZero.BottomCenter:
				result = new Vector3(Screen.width/2f, 0, 0);
				break;
			case AxesZero.Center:
				result = new Vector3(Screen.width/2f, Screen.height/2f, 0);
				break;
			}
			
			return result;
		}
	}
}