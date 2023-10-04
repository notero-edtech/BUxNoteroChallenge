/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using System.Collections;
using ForieroEngine.Music.SMuFL.Classes;
using ForieroEngine.Music.SMuFL.Extensions;
using UnityEngine.UI;

namespace ForieroEngine.Music.NotationSystem.Classes
{
	public class NSMultiSegmentLine : NSObjectSMuFL
	{
		public Vector3 startPoint;
		public Vector3 endPoint;

		public SMuFL.Ranges.MultiSegmentLines start;
		public SMuFL.Ranges.MultiSegmentLines body;
		public SMuFL.Ranges.MultiSegmentLines end;

		public override void Reset ()
		{
			base.Reset ();

			text.SetText ("");

			start = (SMuFL.Ranges.MultiSegmentLines)(-1);
			body = (SMuFL.Ranges.MultiSegmentLines)(-1);
			end = (SMuFL.Ranges.MultiSegmentLines)(-1);
		}
	}
}
