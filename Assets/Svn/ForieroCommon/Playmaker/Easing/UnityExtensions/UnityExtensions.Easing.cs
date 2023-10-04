using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static partial class UnityExtensions {
	
	public static void DrawPathGizmos(Transform[] path,Color color) {
		if(path.Length>0){
			//create and store path points:
			Vector3[] suppliedPath = new Vector3[path.Length];
			for (int i = 0; i < path.Length; i++) {
				suppliedPath[i]=path[i].position;
			}
			
			DrawPathHelper(suppliedPath, color);
		}
	}
	
	public static void DrawPathGizmos(Vector3[] path, Color color) {
		if(path.Length>0){
			DrawPathHelper(path, color);
		}
	}
	
	private static void DrawPathHelper(Vector3[] path, Color color){
		Vector3[] vector3s = PathControlPointGenerator(path);
		
		//Line Draw:
		Vector3 prevPt = Interp(vector3s,0);
		Gizmos.color=color;
		int SmoothAmount = path.Length*20;
		for (int i = 1; i <= SmoothAmount; i++) {
			float pm = (float) i / SmoothAmount;
			Vector3 currPt = Interp(vector3s,pm);
			Gizmos.DrawLine(currPt, prevPt);
			prevPt = currPt;
		}
	}
	
	private static Vector3[] PathControlPointGenerator(Vector3[] path){
		Vector3[] suppliedPath;
		Vector3[] vector3s;
		
		//create and store path points:
		suppliedPath = path;

		//populate calculate path;
		int offset = 2;
		vector3s = new Vector3[suppliedPath.Length+offset];
		Array.Copy(suppliedPath,0,vector3s,1,suppliedPath.Length);
		
		//populate start and end control points:
		//vector3s[0] = vector3s[1] - vector3s[2];
		vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
		vector3s[vector3s.Length-1] = vector3s[vector3s.Length-2] + (vector3s[vector3s.Length-2] - vector3s[vector3s.Length-3]);
		
		//is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
		if(vector3s[1] == vector3s[vector3s.Length-2]){
			Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
			Array.Copy(vector3s,tmpLoopSpline,vector3s.Length);
			tmpLoopSpline[0]=tmpLoopSpline[tmpLoopSpline.Length-3];
			tmpLoopSpline[tmpLoopSpline.Length-1]=tmpLoopSpline[2];
			vector3s=new Vector3[tmpLoopSpline.Length];
			Array.Copy(tmpLoopSpline,vector3s,tmpLoopSpline.Length);
		}	
		
		return(vector3s);
	}
	
		/// <summary>
	/// Returns the length of a curved path drawn through the provided array of Transforms.
	/// </summary>
	/// <returns>
	/// A <see cref="System.Single"/>
	/// </returns>
	/// <param name='path'>
	/// A <see cref="Transform[]"/>
	/// </param>
	public static float PathLength(Transform[] path){
		Vector3[] suppliedPath = new Vector3[path.Length];
		float pathLength = 0;
		
		//create and store path points:
		for (int i = 0; i < path.Length; i++) {
			suppliedPath[i]=path[i].position;
		}
		
		Vector3[] vector3s = PathControlPointGenerator(suppliedPath);
		
		//Line Draw:
		Vector3 prevPt = Interp(vector3s,0);
		int SmoothAmount = path.Length*20;
		for (int i = 1; i <= SmoothAmount; i++) {
			float pm = (float) i / SmoothAmount;
			Vector3 currPt = Interp(vector3s,pm);
			pathLength += Vector3.Distance(prevPt,currPt);
			prevPt = currPt;
		}
		
		return pathLength;
	}
	
	/// <summary>
	/// Returns the length of a curved path drawn through the provided array of Vector3s.
	/// </summary>
	/// <returns>
	/// The length.
	/// </returns>
	/// <param name='path'>
	/// A <see cref="Vector3[]"/>
	/// </param>
	public static float PathLength(Vector3[] path){
		float pathLength = 0;
		
		Vector3[] vector3s = PathControlPointGenerator(path);
		
		//Line Draw:
		Vector3 prevPt = Interp(vector3s,0);
		int SmoothAmount = path.Length*20;
		for (int i = 1; i <= SmoothAmount; i++) {
			float pm = (float) i / SmoothAmount;
			Vector3 currPt = Interp(vector3s,pm);
			pathLength += Vector3.Distance(prevPt,currPt);
			prevPt = currPt;
		}
		
		return pathLength;
	}	
	
	/// <summary>
	/// Creates and returns a full-screen Texture2D for use with CameraFade.
	/// </summary>
	/// <returns>
	/// Texture2D
	/// </returns>
	/// <param name='color'>
	/// Color
	/// </param>
	public static Texture2D CameraTexture(Color color){
		Texture2D texture = new Texture2D(Screen.width,Screen.height,TextureFormat.ARGB32, false);
		Color[] colors = new Color[Screen.width*Screen.height];
		for (int i = 0; i < colors.Length; i++) {
			colors[i]=color;
		}
		texture.SetPixels(colors);
		texture.Apply();
		return(texture);		
	}
	
	/// <summary>
	/// Puts a GameObject on a path at the provided percentage 
	/// </summary>
	/// <param name="target">
	/// A <see cref="GameObject"/>
	/// </param>
	/// <param name="path">
	/// A <see cref="Vector3[]"/>
	/// </param>
	/// <param name="percent">
	/// A <see cref="System.Single"/>
	/// </param>
	public static void PutOnPath(GameObject target, Vector3[] path, float percent){
		target.transform.position=Interp(PathControlPointGenerator(path),percent);
	}
	
	/// <summary>
	/// Puts a GameObject on a path at the provided percentage 
	/// </summary>
	/// <param name="target">
	/// A <see cref="Transform"/>
	/// </param>
	/// <param name="path">
	/// A <see cref="Vector3[]"/>
	/// </param>
	/// <param name="percent">
	/// A <see cref="System.Single"/>
	/// </param>
	public static void PutOnPath(Transform target, Vector3[] path, float percent){
		target.position=Interp(PathControlPointGenerator(path),percent);
	}	
	
	/// <summary>
	/// Puts a GameObject on a path at the provided percentage 
	/// </summary>
	/// <param name="target">
	/// A <see cref="GameObject"/>
	/// </param>
	/// <param name="path">
	/// A <see cref="Transform[]"/>
	/// </param>
	/// <param name="percent">
	/// A <see cref="System.Single"/>
	/// </param>
	public static void PutOnPath(GameObject target, Transform[] path, float percent){
		//create and store path points:
		Vector3[] suppliedPath = new Vector3[path.Length];
		for (int i = 0; i < path.Length; i++) {
			suppliedPath[i]=path[i].position;
		}	
		target.transform.position=Interp(PathControlPointGenerator(suppliedPath),percent);
	}	
	
	/// <summary>
	/// Puts a GameObject on a path at the provided percentage 
	/// </summary>
	/// <param name="target">
	/// A <see cref="Transform"/>
	/// </param>
	/// <param name="path">
	/// A <see cref="Transform[]"/>
	/// </param>
	/// <param name="percent">
	/// A <see cref="System.Single"/>
	/// </param>
	public static void PutOnPath(Transform target, Transform[] path, float percent){
		//create and store path points:
		Vector3[] suppliedPath = new Vector3[path.Length];
		for (int i = 0; i < path.Length; i++) {
			suppliedPath[i]=path[i].position;
		}	
		target.position=Interp(PathControlPointGenerator(suppliedPath),percent);
	}		
	
	/// <summary>
	/// Returns a Vector3 position on a path at the provided percentage  
	/// </summary>
	/// <param name="path">
	/// A <see cref="Transform[]"/>
	/// </param>
	/// <param name="percent">
	/// A <see cref="System.Single"/>
	/// </param>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	public static Vector3 PointOnPath(Transform[] path, float percent){
		//create and store path points:
		Vector3[] suppliedPath = new Vector3[path.Length];
		for (int i = 0; i < path.Length; i++) {
			suppliedPath[i]=path[i].position;
		}	
		return(Interp(PathControlPointGenerator(suppliedPath),percent));
	}
	
	//andeeee from the Unity forum's steller Catmull-Rom class ( http://forum.unity3d.com/viewtopic.php?p=218400#218400 ):
	private static Vector3 Interp(Vector3[] pts, float t){
		int numSections = pts.Length - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
		float u = t * (float) numSections - (float) currPt;
				
		Vector3 a = pts[currPt];
		Vector3 b = pts[currPt + 1];
		Vector3 c = pts[currPt + 2];
		Vector3 d = pts[currPt + 3];
		
		return .5f * (
			(-a + 3f * b - 3f * c + d) * (u * u * u)
			+ (2f * a - 5f * b + 4f * c - d) * (u * u)
			+ (-a + c) * u
			+ 2f * b
		);
	}
	
	//andeeee from the Unity forum's steller Catmull-Rom class ( http://forum.unity3d.com/viewtopic.php?p=218400#218400 ):
	private class CRSpline {
		public Vector3[] pts;
		
		public CRSpline(params Vector3[] pts) {
			this.pts = new Vector3[pts.Length];
			Array.Copy(pts, this.pts, pts.Length);
		}
		
		
		public Vector3 Interp(float t) {
			int numSections = pts.Length - 3;
			int currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
			float u = t * (float) numSections - (float) currPt;
			Vector3 a = pts[currPt];
			Vector3 b = pts[currPt + 1];
			Vector3 c = pts[currPt + 2];
			Vector3 d = pts[currPt + 3];
			return .5f*((-a+3f*b-3f*c+d)*(u*u*u)+(2f*a-5f*b+4f*c-d)*(u*u)+(-a+c)*u+2f*b);
		}	
	}
	
	public static System.Func<float, float, float, float> GetEasingFunction(EaseType anEaseType){
		System.Func<float, float, float, float> ease = null;
		switch (anEaseType){
		case EaseType.easeInQuad:
			ease  = (easeInQuad);
			break;
		case EaseType.easeOutQuad:
			ease = (easeOutQuad);
			break;
		case EaseType.easeInOutQuad:
			ease = (easeInOutQuad);
			break;
		case EaseType.easeInCubic:
			ease = (easeInCubic);
			break;
		case EaseType.easeOutCubic:
			ease = (easeOutCubic);
			break;
		case EaseType.easeInOutCubic:
			ease = (easeInOutCubic);
			break;
		case EaseType.easeInQuart:
			ease = (easeInQuart);
			break;
		case EaseType.easeOutQuart:
			ease = (easeOutQuart);
			break;
		case EaseType.easeInOutQuart:
			ease = (easeInOutQuart);
			break;
		case EaseType.easeInQuint:
			ease = (easeInQuint);
			break;
		case EaseType.easeOutQuint:
			ease = (easeOutQuint);
			break;
		case EaseType.easeInOutQuint:
			ease = (easeInOutQuint);
			break;
		case EaseType.easeInSine:
			ease = (easeInSine);
			break;
		case EaseType.easeOutSine:
			ease = (easeOutSine);
			break;
		case EaseType.easeInOutSine:
			ease = (easeInOutSine);
			break;
		case EaseType.easeInExpo:
			ease = (easeInExpo);
			break;
		case EaseType.easeOutExpo:
			ease = (easeOutExpo);
			break;
		case EaseType.easeInOutExpo:
			ease = (easeInOutExpo);
			break;
		case EaseType.easeInCirc:
			ease = (easeInCirc);
			break;
		case EaseType.easeOutCirc:
			ease = (easeOutCirc);
			break;
		case EaseType.easeInOutCirc:
			ease = (easeInOutCirc);
			break;
		case EaseType.easeLinear:
			ease = (easeLinear);
			break;
		case EaseType.easeSpring:
			ease = (easeSpring);
			break;
		case EaseType.easeInBounce:
			ease = (easeInBounce);
			break;
		case EaseType.easeOutBounce:
			ease = (easeOutBounce);
			break;
		case EaseType.easeInOutBounce:
			ease = (easeInOutBounce);
			break;
		case EaseType.easeInBack:
			ease = (easeInBack);
			break;
		case EaseType.easeOutBack:
			ease = (easeOutBack);
			break;
		case EaseType.easeInOutBack:
			ease = (easeInOutBack);
			break;
		case EaseType.easeInElastic:
			ease = (easeInElastic);
			break;
		case EaseType.easeOutElastic:
			ease = (easeOutElastic);
			break;
		case EaseType.easeInOutElastic:
			ease = (easeInOutElastic);
			break;
		default: 
			ease = (easeLinear);
			break;
		}
		return ease;
	}
	
	/// <summary>
	/// The type of easing to use based on Robert Penner's open source easing equations (http://www.robertpenner.com/easing_terms_of_use.html).
	/// </summary>
	public enum EaseType{
		easeInQuad,
		easeOutQuad,
		easeInOutQuad,
		easeInCubic,
		easeOutCubic,
		easeInOutCubic,
		easeInQuart,
		easeOutQuart,
		easeInOutQuart,
		easeInQuint,
		easeOutQuint,
		easeInOutQuint,
		easeInSine,
		easeOutSine,
		easeInOutSine,
		easeInExpo,
		easeOutExpo,
		easeInOutExpo,
		easeInCirc,
		easeOutCirc,
		easeInOutCirc,
		easeLinear,
		easeSpring,
		easeInBounce,
		easeOutBounce,
		easeInOutBounce,
		easeInBack,
		easeOutBack,
		easeInOutBack,
		easeInElastic,
		easeOutElastic,
		easeInOutElastic,
	}
		
	#region Easing Functions
	
	public static float easeLinear(float start, float end, float value){
		return Mathf.Lerp(start, end, value);
	}
	
	public static float clerp(float start, float end, float value){
		float min = 0.0f;
		float max = 360.0f;
		float half = Mathf.Abs((max - min) / 2.0f);
		float retval = 0.0f;
		float diff = 0.0f;
		if ((end - start) < -half){
			diff = ((max - start) + end) * value;
			retval = start + diff;
		}else if ((end - start) > half){
			diff = -((max - end) + start) * value;
			retval = start + diff;
		}else retval = start + (end - start) * value;
		return retval;
    }

	public static float easeSpring(float start, float end, float value){
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}

	public static float easeInQuad(float start, float end, float value){
		end -= start;
		return end * value * value + start;
	}

	public static float easeOutQuad(float start, float end, float value){
		end -= start;
		return -end * value * (value - 2) + start;
	}

	public static float easeInOutQuad(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end / 2 * value * value + start;
		value--;
		return -end / 2 * (value * (value - 2) - 1) + start;
	}

	public static float easeInCubic(float start, float end, float value){
		end -= start;
		return end * value * value * value + start;
	}

	public static float easeOutCubic(float start, float end, float value){
		value--;
		end -= start;
		return end * (value * value * value + 1) + start;
	}

	public static float easeInOutCubic(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end / 2 * value * value * value + start;
		value -= 2;
		return end / 2 * (value * value * value + 2) + start;
	}

	public static float easeInQuart(float start, float end, float value){
		end -= start;
		return end * value * value * value * value + start;
	}

	public static float easeOutQuart(float start, float end, float value){
		value--;
		end -= start;
		return -end * (value * value * value * value - 1) + start;
	}

	public static float easeInOutQuart(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end / 2 * value * value * value * value + start;
		value -= 2;
		return -end / 2 * (value * value * value * value - 2) + start;
	}

	public static float easeInQuint(float start, float end, float value){
		end -= start;
		return end * value * value * value * value * value + start;
	}

	public static float easeOutQuint(float start, float end, float value){
		value--;
		end -= start;
		return end * (value * value * value * value * value + 1) + start;
	}

	public static float easeInOutQuint(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end / 2 * value * value * value * value * value + start;
		value -= 2;
		return end / 2 * (value * value * value * value * value + 2) + start;
	}

	public static float easeInSine(float start, float end, float value){
		end -= start;
		return -end * Mathf.Cos(value / 1 * (Mathf.PI / 2)) + end + start;
	}

	public static float easeOutSine(float start, float end, float value){
		end -= start;
		return end * Mathf.Sin(value / 1 * (Mathf.PI / 2)) + start;
	}

	public static float easeInOutSine(float start, float end, float value){
		end -= start;
		return -end / 2 * (Mathf.Cos(Mathf.PI * value / 1) - 1) + start;
	}

	public static float easeInExpo(float start, float end, float value){
		end -= start;
		return end * Mathf.Pow(2, 10 * (value / 1 - 1)) + start;
	}

	public static float easeOutExpo(float start, float end, float value){
		end -= start;
		return end * (-Mathf.Pow(2, -10 * value / 1) + 1) + start;
	}

	public static float easeInOutExpo(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end / 2 * Mathf.Pow(2, 10 * (value - 1)) + start;
		value--;
		return end / 2 * (-Mathf.Pow(2, -10 * value) + 2) + start;
	}

	public static float easeInCirc(float start, float end, float value){
		end -= start;
		return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
	}

	public static float easeOutCirc(float start, float end, float value){
		value--;
		end -= start;
		return end * Mathf.Sqrt(1 - value * value) + start;
	}

	public static float easeInOutCirc(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return -end / 2 * (Mathf.Sqrt(1 - value * value) - 1) + start;
		value -= 2;
		return end / 2 * (Mathf.Sqrt(1 - value * value) + 1) + start;
	}

	/* GFX47 MOD START */
	public static float easeInBounce(float start, float end, float value){
		end -= start;
		float d = 1f;
		return end - easeOutBounce(0, end, d-value) + start;
	}
	/* GFX47 MOD END */

	/* GFX47 MOD START */
	//public static float bounce(float start, float end, float value){
	public static float easeOutBounce(float start, float end, float value){
		value /= 1f;
		end -= start;
		if (value < (1 / 2.75f)){
			return end * (7.5625f * value * value) + start;
		}else if (value < (2 / 2.75f)){
			value -= (1.5f / 2.75f);
			return end * (7.5625f * (value) * value + .75f) + start;
		}else if (value < (2.5 / 2.75)){
			value -= (2.25f / 2.75f);
			return end * (7.5625f * (value) * value + .9375f) + start;
		}else{
			value -= (2.625f / 2.75f);
			return end * (7.5625f * (value) * value + .984375f) + start;
		}
	}
	/* GFX47 MOD END */

	/* GFX47 MOD START */
	public static float easeInOutBounce(float start, float end, float value){
		end -= start;
		float d = 1f;
		if (value < d/2) return easeInBounce(0, end, value*2) * 0.5f + start;
		else return easeOutBounce(0, end, value*2-d) * 0.5f + end*0.5f + start;
	}
	/* GFX47 MOD END */

	public static float easeInBack(float start, float end, float value){
		end -= start;
		value /= 1;
		float s = 1.70158f;
		return end * (value) * value * ((s + 1) * value - s) + start;
	}

	public static float easeOutBack(float start, float end, float value){
		float s = 1.70158f;
		end -= start;
		value = (value / 1) - 1;
		return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
	}

	public static float easeInOutBack(float start, float end, float value){
		float s = 1.70158f;
		end -= start;
		value /= .5f;
		if ((value) < 1){
			s *= (1.525f);
			return end / 2 * (value * value * (((s) + 1) * value - s)) + start;
		}
		value -= 2;
		s *= (1.525f);
		return end / 2 * ((value) * value * (((s) + 1) * value + s) + 2) + start;
	}

	public static float easePunch(float amplitude, float value){
		float s = 9;
		if (value == 0){
			return 0;
		}
		if (value == 1){
			return 0;
		}
		float period = 1 * 0.3f;
		s = period / (2 * Mathf.PI) * Mathf.Asin(0);
		return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
    }
	
	/* GFX47 MOD START */
	public static float easeInElastic(float start, float end, float value){
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d) == 1) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p / 4;
			}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		return -(a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
	}		
	/* GFX47 MOD END */

	/* GFX47 MOD START */
	//public static float elastic(float start, float end, float value){
	public static float easeOutElastic(float start, float end, float value){
	/* GFX47 MOD END */
		//Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d) == 1) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p / 4;
			}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
	}		
	
	/* GFX47 MOD START */
	public static float easeInOutElastic(float start, float end, float value){
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d/2) == 2) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p / 4;
			}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
		return a * Mathf.Pow(2, -10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
	}		
	/* GFX47 MOD END */
	
	#endregion
}
