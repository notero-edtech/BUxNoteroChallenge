/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NSRectBlocker : Graphic
{
	/*
	 * This blocks input within the entire rect, as if there was an image drawn (but it doesn't actually draw anything)
	 */

	public override bool Raycast (Vector2 sp, Camera eventCamera)
	{
		if (!isActiveAndEnabled) {
			return false;
		}

		return true;
	}

	protected override void OnPopulateMesh (VertexHelper vh)
	{
		vh.Clear ();
	}
}
