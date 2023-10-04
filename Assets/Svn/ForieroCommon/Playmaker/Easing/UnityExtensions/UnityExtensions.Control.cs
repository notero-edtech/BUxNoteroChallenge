using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ForieroEngine;

public static partial class UnityExtensions
{

	static char[] chars = {
		'a',
		'b',
		'c',
		'd',
		'e',
		'f',
		'g',
		'h',
		'i',
		'j',
		'k',
		'l',
		'm',
		'n',
		'o',
		'p',
		'q',
		'r',
		's',
		't',
		'u',
		'v',
		'w',
		'x',
		'y',
		'z',
		'A',
		'B',
		'C',
		'D',
		'E',
		'F',
		'G',
		'H',
		'I',
		'J',
		'K',
		'L',
		'M',
		'N',
		'O',
		'P',
		'Q',
		'R',
		'S',
		'T',
		'U',
		'V',
		'W',
		'X',
		'Y',
		'Z',
		'0',
		'1',
		'2',
		'3',
		'4',
		'5',
		'6',
		'7',
		'8'
	};

	public static string GUID ()
	{
		int strlen = 16;
		int num_chars = chars.Length - 1;
		string randomChar = "";
		for (int i = 0; i < strlen; i++) {
			randomChar += chars [(int)Mathf.Floor (UnityEngine.Random.Range (0, num_chars))];
		}
		return randomChar;
	}

	#region Animation Control

	public class CancelSignal
	{
		public bool free = true;
		public string id = "";
		public string category = "";
		public bool cancel = false;
		public bool paused = false;
	}

	public static List<CancelSignal> cancelSignals = new List<CancelSignal> { 
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ },
		new CancelSignal{ }
	};

	public static void CancelAll ()
	{
		foreach (CancelSignal cs in cancelSignals) {
			cs.cancel = true;	
		}
	}

	public static void CancelAnimateTo (string anId, bool aContains, string aCategory)
	{
		var x = aContains ? 
				from cs in cancelSignals
		  where cs.id.Contains (anId) && (string.IsNullOrEmpty (aCategory) ? true : cs.category.Equals (aCategory)) && !cs.cancel
		  select cs
			:
				from cs in cancelSignals
		  where cs.id.Equals (anId) && (string.IsNullOrEmpty (aCategory) ? true : cs.category.Equals (aCategory)) && !cs.cancel
		  select cs;	

		if (Foriero.debug) {
			Debug.Log ("Easing - Cancel Count : " + x.Count ().ToString () + " Buffer : " + cancelSignals.Count.ToString ());
		}

		foreach (CancelSignal cs in x) {
			cs.cancel = true;
		}	
	}

	public static void PauseAnimateTo (string anId, bool aContains, string aCategory)
	{
		var x = aContains ?
				from cs in cancelSignals
		  where cs.id.Contains (anId) && string.IsNullOrEmpty (aCategory) ? true : cs.category.Equals (aCategory)
		  select cs
			:
				from cs in cancelSignals
		  where cs.id.Equals (anId) && string.IsNullOrEmpty (aCategory) ? true : cs.category.Equals (aCategory)
		  select cs;

		if (Foriero.debug) {
			Debug.Log ("Easing - Pause Count : " + x.Count ().ToString () + " Buffer : " + cancelSignals.Count.ToString ());
		}
		foreach (CancelSignal cs in x) {
			cs.paused = true;
		}
	}

	public static void ResumeAnimateTo (string anId, bool aContains, string aCategory)
	{
		var x = aContains ?
				from cs in cancelSignals
		  where cs.id.Contains (anId) && string.IsNullOrEmpty (aCategory) ? true : cs.category.Equals (aCategory)
		  select cs
			:
				from cs in cancelSignals
		  where cs.id.Equals (anId) && string.IsNullOrEmpty (aCategory) ? true : cs.category.Equals (aCategory)
		  select cs;
		if (Foriero.debug) {
			Debug.Log ("Easing - Resume Count : " + x.Count ().ToString () + " Buffer : " + cancelSignals.Count.ToString ());
		}
		foreach (CancelSignal cs in x) {
			cs.paused = false;
		}
	}

	private static CancelSignal GetFreeCancelSignal ()
	{
		CancelSignal cs = null;
		for (int i = 0; i < cancelSignals.Count (); i++) {
			if (cancelSignals [i].free == true) {
				cs = cancelSignals [i];
				cs.free = false;
				cs.cancel = false;
				cs.paused = false;
				cs.id = "";
				break;
			}
		}
				
		if (cs == null) {
			cs = new UnityExtensions.CancelSignal{ free = false, id = "", cancel = false, paused = false };
			cancelSignals.Add (cs);
		}
		return cs;
	}

	#endregion
}
