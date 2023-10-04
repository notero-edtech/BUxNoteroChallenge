using System.Collections;
using UnityEngine;

public class GameSettingsInit : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return null;
        GameSettings.instance.audio.Init();
    }
}
