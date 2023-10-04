using System.Collections;
using System.Collections.Generic;
using ForieroEngine.MIDIUnified.SysEx;
using UnityEngine;
using UnityEngine.UI;

public class MTCExample : MonoBehaviour
{
    // Start is called before the first frame update
    public Text text;
    bool update = false;


    void Start()
    {
        update = true;
        SysEx.MTC.OnMTCRealTimeFrame += () =>
        {
            update = true;
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (update)
        {
            text.text = SysEx.MTC.mtcRealTimeFrame.ToString();
            update = false;
        }
    }
}
