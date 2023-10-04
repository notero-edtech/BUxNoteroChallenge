using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMFadeMusic : MonoBehaviour {

    public string musicGroupId = "";

    private void OnDestroy()
    {
        SM.StopMusic(musicGroupId);
    }
}
