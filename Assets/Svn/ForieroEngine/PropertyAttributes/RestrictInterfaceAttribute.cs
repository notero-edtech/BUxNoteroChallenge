using UnityEngine;

public class RestrictInterfaceAttribute : PropertyAttribute
{
	public System.Type RestrictType { get; set; }

	public RestrictInterfaceAttribute (System.Type restrictType)
	{
		RestrictType = restrictType;
	}
}
