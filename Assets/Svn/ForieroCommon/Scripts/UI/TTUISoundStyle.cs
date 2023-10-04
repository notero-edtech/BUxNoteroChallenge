using UnityEngine;
using System.Collections;

public class TTUISoundStyle : ScriptableObject
{
	public SM.Provider provider = SM.Provider.Default;

	public string onMouseEnter = "OnMouseEnter";
	public string onMouseExit = "OnMouseExit";
	public string onMouseDown = "OnMouseDown";
	public string onMouseUp = "OnMouseUp";
}
