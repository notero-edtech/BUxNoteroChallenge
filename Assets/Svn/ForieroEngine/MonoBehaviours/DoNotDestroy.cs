using UnityEngine;
using System.Collections;

public class DoNotDestroy : MonoBehaviour {
	void Awake(){
		DontDestroyOnLoad(gameObject);	
	}
}
