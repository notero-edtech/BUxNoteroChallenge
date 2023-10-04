using UnityEngine;
using System.Collections;

namespace ForieroEngine.Extensions
{
	public static partial class ForieroEngineExtensions
	{
		public static int GreatestCommonDivider (this int a, int b)
		{
			var r = 0;
			while (b != 0) { r = a % b; a = b; b = r; }
			return a;
		}
		public static float Radians (this float value) => value * Mathf.Rad2Deg;
		public static float Degrees (this float value) => value * Mathf.Rad2Deg;
		public static float Cos (this float value) => Mathf.Cos (value);	
		public static float Sin (this float value) => Mathf.Sin (value);	
		public static float Tan (this float value) => Mathf.Tan (value);
	}
}
