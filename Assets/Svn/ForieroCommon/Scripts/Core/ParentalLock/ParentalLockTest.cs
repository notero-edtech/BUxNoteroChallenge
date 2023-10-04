using UnityEngine;
using System.Collections;

public class ParentalLockTest : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		ParentalLock.Show (4, (ok) => {
			Debug.Log ("OK : " + ok.ToString ());
			Start ();
		});
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
