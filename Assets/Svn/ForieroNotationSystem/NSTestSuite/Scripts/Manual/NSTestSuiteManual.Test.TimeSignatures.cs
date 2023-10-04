/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;
using ForieroEngine.Music.SMuFL.Ranges;


public partial class NSTestSuiteManual : MonoBehaviour
{
    Vector2 TimeSignatureTest(NSStave stave, Vector2 shift)
    {
        float pixelShift = shift.x;
        for (int d = 1; d < 13; d++)
        {
            for (int n = 1; n < 13; n++)
            {
                //NSTimeSignature timeSignature = stave.AddTimeSignature (n, d);
                //timeSignature.PixelShift (new Vector2 (pixelShift, shift.y), true);
                //timeSignature.Commit ();
                //pixelShift += 70;
            }
        }

        return new Vector2(pixelShift, shift.y);
    }
}
