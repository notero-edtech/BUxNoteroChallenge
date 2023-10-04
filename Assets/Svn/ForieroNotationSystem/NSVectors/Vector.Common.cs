/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using System.Collections;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public enum VectorEnum
    {
        LineTest = 0,
        LineHorizontal = 100,
        LineVertical = 200,
        Beam = 300,
        Slur1 = 400,
        Slur2 = 500,
        Tie = 600,
        Hairpin = 700,
        Tuplet = 800,
        DurationBarHorizontal = 900,
        Undefined = int.MaxValue
    }

    public enum LineEndsEnum
    {
        None,
        Horizontal,
        Vertical
    }


    /* When you imagine line from (0,0) to (0, 10) then 
     * LEFT means left corner of the line where the line origins
     * MIDDLE means middle point in between Left and Right corner wherer the line origins
     * RIGHT means right corner of the line where the line origns
     */
    public enum LineAlignEnum
    {
        Left,
        Middle,
        Right
    }


    public enum BezierEnum
    {
        Line,
        Curve
    }

    public class Easing
	{

		public enum Mode
		{
			easeLinear = 0,
			easeInQuad = 1,
			easeOutQuad = 2,
			easeInOutQuad = 3,
			easeOutInQuad = 4,
			easeInCubic = 5,
			easeOutCubic = 6,
			easeInOutCubic = 7,
			easeOutInCubic = 8,
			easeInQuart = 9,
			easeOutQuart = 10,
			easeInOutQuart = 11,
			easeOutInQuart = 12,
			easeInSine = 13,
			easeOutSine = 14,
			easeInOutSine = 15,
			easeOutInSine = 16,
			easeInExpo = 17,
			easeOutExpo = 18,
			easeInOutExpo = 19,
			easeOutInExpo = 20,
			easeInCirc = 21,
			easeOutCirc = 22,
			easeInOutCirc = 23,
			easeOutInCirc = 24,
			easeInElastic = 25,
			easeOutElastic = 26,
			easeInOutElastic = 27,
			easeOutInElastic = 28,
			easeInBack = 29,
			easeOutBack = 30,
			easeInOutBack = 31,
			easeOutInBack = 32,
			easeInBounce = 33,
			easeOutBounce = 34,
			easeInOutBounce = 35,
			easeOutInBounce = 36,
			easeNone = 37
		}

		public static float GetEaseInQuad (float t, float b, float c, float d)
		{
			t = t / d;
			return c * t * t + b;
		}

		public static float GetEaseOutQuad (float t, float b, float c, float d)
		{
			t = t / d;
			return -c * t * (t - 2) + b;
		}

		// Easing equation float for a quadratic (t^2) easing in/out: acceleration until halfway,  deceleration.
		public static float GetEaseInOutQuad (float t, float b, float c, float d)
		{
			t = t / (d * 0.5f);
			if (t < 1)
				return c / 2 * t * t + b;


			t = t - 1;
			return -c / 2 * (t * (t - 2) - 1) + b;
		}

		// Easing equation float for a quadratic (t^2) easing out/in: deceleration until halfway,  acceleration.
		public static float GetEaseOutInQuad (float t, float b, float c, float d)
		{
			if (t < d / 2)
				return GetEaseOutQuad (t * 2, b, c / 2, d);

			return GetEaseInQuad ((t * 2) - d, b + c / 2, c / 2, d);
		}

		// Easing equation float for a cubic (t^3) easing in: accelerating from zero velocity.
		public static float GetEaseInCubic (float t, float b, float c, float d)
		{
			t = t / d;
			return c * t * t * t + b;
		}

		// Easing equation float for a cubic (t^3) easing out: decelerating from zero velocity.
		public static float GetEaseOutCubic (float t, float b, float c, float d)
		{
			t = t / d;
			t = t - 1;
			return c * (t * t * t + 1) + b;
		}


		// Easing equation float for a cubic (t^3) easing in/out: acceleration until halfway,  deceleration.
		public static float GetEaseInOutCubic (float t, float b, float c, float d)
		{
			t = t / (d * 0.5f);
			if (t < 1)
				return (c / 2) * t * t * t + b;

			t = t - 2;
			return (c / 2) * (t * t * t + 2) + b;
		}

		// Easing equation float for a cubic (t^3) easing out/in: deceleration until halfway,  acceleration.
		public static float GetEaseOutInCubic (float t, float b, float c, float d)
		{
			if (t < d / 2)
				return GetEaseOutCubic (t * 2, b, c / 2, d);

			return GetEaseInCubic ((t * 2) - d, b + c / 2, c / 2, d);
		}

		// Easing equation float for a quartic (t^4) easing in: accelerating from zero velocity.
		public static float GetEaseInQuart (float t, float b, float c, float d)
		{
			t = t / d;
			return c * t * t * t * t + b;
		}

		//Easing equation float for a quartic (t^4) easing out: decelerating from zero velocity.
		public static float GetEaseOutQuart (float t, float b, float c, float d)
		{
			t = t / d;
			t = t - 1;
			return -c * (t * t * t * t - 1) + b;
		}

		// Easing equation float for a quartic (t^4) easing in/out: acceleration until halfway,  deceleration.
		public static float GetEaseInOutQuart (float t, float b, float c, float d)
		{
			t = t / (d * 0.5f);
			if (t < 1)
				return (c / 2) * t * t * t * t + b;

			t = t - 2;
			return (-c / 2) * (t * t * t * t - 2) + b;
		}

		// Easing equation float for a quartic (t^4) easing out/in: deceleration until halfway,  acceleration.
		public static float GetEaseOutInQuart (float t, float b, float c, float d)
		{
			if (t < d / 2)
				return GetEaseOutQuart (t * 2, b, c / 2, d);

			return GetEaseInQuart ((t * 2) - d, b + c / 2, c / 2, d);
		}

		// Easing equation float for a sinusoidal (Mathf.Sin(t)) easing in: accelerating from zero velocity.
		public static float GetEaseInSine (float t, float b, float c, float d)
		{
			return -c * Mathf.Cos (t / d * (Mathf.PI * 0.5f)) + c + b;
		}

		// Easing equation float for a sinusoidal (Mathf.Sin(t)) easing out: decelerating from zero velocity.
		public static float GetEaseOutSine (float t, float b, float c, float d)
		{
			return c * Mathf.Sin (t / d * (Mathf.PI * 0.5f)) + b;
		}

		// Easing equation float for a sinusoidal (Mathf.Sin(t)) easing in/out: acceleration until halfway,  deceleration.
		public static float GetEaseInOutSine (float t, float b, float c, float d)
		{
			return -c / 2 * (Mathf.Cos (Mathf.PI * t / d) - 1) + b;
		}

		//Easing equation float for a sinusoidal (Mathf.Sin(t)) easing out/in: deceleration until halfway,  acceleration.
		public static float GetEaseOutInSine (float t, float b, float c, float d)
		{
			if (t < d / 2)
				return GetEaseOutSine (t * 2, b, c / 2, d);
			;

			return GetEaseInSine ((t * 2) - d, b + c / 2, c / 2, d);
		}

		// Easing equation float for an exponential (2^t) easing in: accelerating from zero velocity.
		public static float GetEaseInExpo (float t, float b, float c, float d)
		{
			if (t <= 0)
				return b;

			return c * Mathf.Pow (2, 10 * (t / d - 1)) + b - c * 0.001f;
		}

		// Easing equation float for an exponential (2^t) easing out: decelerating from zero velocity.
		public static float GetEaseOutExpo (float t, float b, float c, float d)
		{
			if (t >= d)
				return b + c;

			return c * 1.001f * (-Mathf.Pow (2, -10 * t / d) + 1) + b;
		}

		// Easing equation float for an exponential (2^t) easing in/out: acceleration until halfway,  deceleration.

		public static float GetEaseInOutExpo (float t, float b, float c, float d)
		{
			if (t <= 0)
				return b;

			if (t >= d)
				return b + c;

			t = t / d;
			if (t * 0.5f < 1)
				return c / 2 * Mathf.Pow (2, 10 * (t - 1)) + b - c * 0.0005f;

			return c / 2 * 1.0005f * (-Mathf.Pow (2, -10 * --t) + 2) + b;
		}

		// Easing equation float for an exponential (2^t) easing out/in: deceleration until halfway,  acceleration.

		public static float GetEaseOutInExpo (float t, float b, float c, float d)
		{
			if (t < d / 2)
				return GetEaseOutExpo (t * 2, b, c / 2, d);

			return GetEaseInExpo ((t * 2) - d, b + c / 2, c / 2, d);
		}

		// Easing equation float for a circular (sqrt(1-t^2)) easing in: accelerating from zero velocity.

		public static float GetEaseInCirc (float t, float b, float c, float d)
		{
			t = t / d;
			return -c * (Mathf.Sqrt (1 - t * t) - 1) + b;
		}

		// Easing equation float for a circular (sqrt(1-t^2)) easing out: decelerating from zero velocity.
		public static float GetEaseOutCirc (float t, float b, float c, float d)
		{
			t = t / d;
			t = t - 1;
			return c * Mathf.Sqrt (1 - t * t) + b;
		}

		// Easing equation float for a circular (sqrt(1-t^2)) easing in/out: acceleration until halfway,  deceleration.

		public static float GetEaseInOutCirc (float t, float b, float c, float d)
		{
			t = t / (d * 0.5f);
			if (t < 1)
				return (-c / 2) * (Mathf.Sqrt (1 - t * t) - 1) + b;

			t = t - 2;
			return (c / 2) * (Mathf.Sqrt (1 - t * t) + 1) + b;
		}

		// Easing equation float for a circular (sqrt(1-t^2)) easing out/in: deceleration until halfway,  acceleration.
		public static float GetEaseOutInCirc (float t, float b, float c, float d)
		{
			if (t < d / 2)
				return GetEaseOutCirc (t * 2, b, c / 2, d);

			return GetEaseInCirc ((t * 2) - d, b + c / 2, c / 2, d);
		}


		// Easing equation float for an elastic (exponentially decaying sine wave) easing in: accelerating from zero velocity.
		public static float GetEaseInElastic (float t, float b, float c, float d)
		{
			float p, s, a, postFix;
			if (t <= 0) {
				return b;
			}

			t = t / d;
			if (t >= 1) {
				return b + c;
			}

			p = d * 0.3f;
			a = c;
			s = p / 4;

			t = t - 1;
			postFix = a * Mathf.Pow (2, 10 * t);
			return -(postFix * Mathf.Sin ((t * d - s) * (2 * Mathf.PI) / p)) + b;
		}

		// Easing equation float for an elastic (exponentially decaying sine wave) easing out: decelerating from zero velocity.
		public static float GetEaseOutElastic (float t, float b, float c, float d)
		{
			float p, s, a;
			if (t <= 0) {
				return b;
			}

			t = t / d;
			if (t >= 1) {
				return b + c;
			}

			p = d * 0.3f;
			s = 0;
			a = 0;
			if (a == 0 || a < Mathf.Abs (c)) {
				a = c;
				s = p / 4;
			} else {
				s = p / (2 * Mathf.PI) * Mathf.Asin (c / a);
			}

			return (a * Mathf.Pow (2, -10 * t) * Mathf.Sin ((t * d - s) * (2 * Mathf.PI) / p) + c + b);
		}

		// Easing equation float for an elastic (exponentially decaying sine wave) easing in/out: acceleration until halfway,  deceleration.
		public static float GetEaseInOutElastic (float t, float b, float c, float d)
		{
			float p, s, a, postFix;
			if (t == 0) {
				return b;
			}

			t = t / (d * 0.5f);
			if (t == 2) {
				return b + c;
			}

			p = d * 0.3f * 1.5f;
			a = c;
			s = p / 4;

			if (t < 1) {
				t = t - 1;
				postFix = a * Mathf.Pow (2, 10 * t); // postIncrement is evil
				return -0.5f * (postFix * Mathf.Sin ((t * d - s) * (2 * Mathf.PI) / p)) + b;
			}

			t = t - 1;
			postFix = a * Mathf.Pow (2, -10 * t); // postIncrement is evil
			return postFix * Mathf.Sin ((t * d - s) * (2 * Mathf.PI) / p) * 0.5f + c + b;
		}

		// Easing equation float for an elastic (exponentially decaying sine wave) easing out/in: deceleration until halfway,  acceleration.
		public static float GetEaseOutInElastic (float t, float b, float c, float d)
		{
			if (t < d / 2)
				return GetEaseOutElastic (t * 2, b, c / 2, d);

			return GetEaseInElastic ((t * 2) - d, b + c / 2, c / 2, d);
		}

		// Easing equation float for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: accelerating from zero velocity.
		public static float GetEaseInBack (float t, float b, float c, float d)
		{
			float s = 1.70158f;
			t = t / d;
			return c * t * t * ((s + 1) * t - s) + b;
		}

		// Easing equation float for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: decelerating from zero velocity.
		public static float GetEaseOutBack (float t, float b, float c, float d)
		{
			float s = 1.70158f;
			t = t / d;
			return c * ((t - 1) * t * ((s + 1) * t + s) + 1) + b;
		}

		// Easing equation float for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: acceleration until halfway,  deceleration.
		public static float GetEaseInOutBack (float t, float b, float c, float d)
		{
			float s = 1.70158f * 1.525f;
			t = t / (d * 0.5f);

			if (t < 1)
				return c / 2 * (t * t * ((s + 1) * t - s)) + b;

			float postFix = t;
			t = t - 2;
			return c / 2 * ((postFix) * t * ((s + 1) * t + s) + 2) + b;
		}

		//Easing equation float for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out/in: deceleration until halfway,  acceleration.
		public static float GetEaseOutInBack (float t, float b, float c, float d)
		{
			if (t < d / 2)
				return GetEaseOutBack (t * 2, b, c / 2, d);

			return GetEaseInBack ((t * 2) - d, b + c / 2, c / 2, d);
		}

		// Easing equation float for a bounce (exponentially decaying parabolic bounce) easing out: decelerating from zero velocity.
		public static float GetEaseOutBounce (float t, float b, float c, float d)
		{
			t = t / d;
			if (t < (1 / 2.75))
				return c * (7.5625f * t * t) + b;

			if (t < (2 / 2.75)) {
				t = t - (1.5f / 2.75f);
				return c * (7.5625f * t * t + 0.75f) + b;
			}

			if (t < (2.5 / 2.75)) {
				t = t - (2.25f / 2.75f);
				return c * (7.5625f * t * t + 0.9375f) + b;
			}

			t = t - (2.625f / 2.75f);
			return c * (7.5625f * t * t + 0.984375f) + b;
		}

		// Easing equation float for a bounce (exponentially decaying parabolic bounce) easing in: accelerating from zero velocity.
		public static float GetEaseInBounce (float t, float b, float c, float d)
		{
			return c - GetEaseOutBounce (d - t, 0, c, d) + b;
		}

		//Easing equation float for a bounce (exponentially decaying parabolic bounce) easing in/out: acceleration until halfway,  deceleration.
		public static float GetEaseInOutBounce (float t, float b, float c, float d)
		{
			if (t < d / 2)
				return GetEaseInBounce (t * 2, 0, c, d) * 0.5f + b;

			return GetEaseOutBounce (t * 2 - d, 0, c, d) * 0.5f + c * 0.5f + b;
		}

		//Easing equation float for a bounce (exponentially decaying parabolic bounce) easing out/in: deceleration until halfway,  acceleration.
		public static float GetEaseOutInBounce (float t, float b, float c, float d)
		{
			if (t < d / 2)
				return GetEaseOutBounce (t * 2, b, c / 2, d);

			return GetEaseInBounce ((t * 2) - d, b + c / 2, c / 2, d);
		}

		public static float GetEaseWiggle (float t, float b, float c, float d)
		{
			if (t >= d)
				return b;

			return b + Mathf.Cos (((Time.time * 1000) % 360) * Mathf.Deg2Rad) * c;
		}

		public static float GetEase (float delta, Mode ease)
		{
			switch (ease) {
			case Mode.easeInQuad:
				return GetEaseInQuad (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutQuad:
				return GetEaseOutQuad (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeInOutQuad:
				return GetEaseInOutQuad (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutInQuad:
				return GetEaseOutInQuad (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeInCubic:
				return GetEaseInCubic (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutCubic:
				return GetEaseOutCubic (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeInOutCubic:
				return GetEaseInOutCubic (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutInCubic:
				return GetEaseOutInCubic (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeInQuart:
				return GetEaseInQuart (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutQuart:
				return GetEaseOutQuart (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeInOutQuart:
				return GetEaseInOutQuart (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutInQuart:
				return GetEaseOutInQuart (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeInSine:
				return GetEaseInSine (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutSine:
				return GetEaseOutSine (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeInOutSine:
				return GetEaseInOutSine (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutInSine:
				return GetEaseOutInSine (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeInExpo:
				return GetEaseInExpo (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutExpo:
				return GetEaseOutExpo (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeInOutExpo:
				return GetEaseInOutExpo (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutInExpo:
				return GetEaseOutInExpo (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeInCirc:
				return GetEaseInCirc (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutCirc:
				return GetEaseOutCirc (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeInOutCirc:
				return GetEaseInOutCirc (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutInCirc:
				return GetEaseOutInCirc (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeInElastic:
				return GetEaseInElastic (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutElastic:
				return GetEaseOutElastic (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeInOutElastic:
				return GetEaseInOutElastic (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutInElastic:
				return GetEaseOutInElastic (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeInBack:
				return GetEaseInBack (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutBack:
				return GetEaseOutBack (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeInOutBack:
				return GetEaseInOutBack (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutInBack:
				return GetEaseOutInBack (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeInBounce:
				return GetEaseInBounce (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutBounce:
				return GetEaseOutBounce (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeInOutBounce:
				return GetEaseInOutBounce (delta, 0.0f, 1.0f, 1.0f);
			case Mode.easeOutInBounce:
				return GetEaseOutInBounce (delta, 0.0f, 1.0f, 1.0f);
			default:
				return delta;
			}
		}

	}

}

