using UnityEngine;

public class SceneNameAttribute : PropertyAttribute
{
	public int selectedValue = 0;
	public bool enableOnly = true;

	public SceneNameAttribute (bool enableOnly = true)
	{
		this.enableOnly = enableOnly;
	}
}