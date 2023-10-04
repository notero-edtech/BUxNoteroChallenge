using UnityEngine;
using System.Collections;

public class LangAudacity : ScriptableObject {

	public string audacityPath;
	public string exportPath;

	[HideInInspector]
	public string[] labelFiles = new string[0];
}
