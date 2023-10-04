using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIDISoundSettingsInstance : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        MIDISoundSettings.Free();
    }
}
