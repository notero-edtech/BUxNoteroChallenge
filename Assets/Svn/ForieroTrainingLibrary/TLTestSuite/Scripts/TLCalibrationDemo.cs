/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.Training;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TLCalibrationDemo : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnGUI()
    {
        if (TL.Calibration.Tap.IsCalibrating())
        {
            if (GUI.Button(new Rect(10, 10, 200, 40), "Stop"))
            {
                TL.Calibration.Tap.StopCalibration();
            }
        }
        else
        {
            if (GUI.Button(new Rect(10, 10, 200, 40), "Start Tap Calibration"))
            {
                TL.Calibration.Tap.StartCalibration();
            }

        }


        if (GUI.Button(new Rect(10, 60, 200, 40), "Clap Calibrate"))
        {
            TL.Calibration.Clap.Calibrate();
        }

    }

}
