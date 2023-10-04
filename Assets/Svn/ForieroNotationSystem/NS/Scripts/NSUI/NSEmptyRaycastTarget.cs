/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;

public class NSEmptyRaycastTarget : Graphic
{
	public override void SetMaterialDirty () { return; }
	public override void SetVerticesDirty () { return; }
	protected override void OnPopulateMesh (VertexHelper vh) { vh.Clear (); return; }
}
