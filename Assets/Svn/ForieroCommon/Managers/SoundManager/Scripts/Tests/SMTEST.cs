using UnityEngine;
using System.Collections;

public class SMTEST : MonoBehaviour
{

	public string musicGroup = "";

	public void OnPlayClick ()
	{
		SM.PlayMusic (musicGroup);
	}

	public void OnPauseClick ()
	{
		SM.PauseMusic (musicGroup);
	}

	public void OnStopClick ()
	{
		SM.StopMusic (musicGroup);
	}

	public void OnNextClick ()
	{
		SM.NextMusic (musicGroup);
	}

	public void OnPrevClick ()
	{
		SM.PrevMusic (musicGroup);
	}


}
