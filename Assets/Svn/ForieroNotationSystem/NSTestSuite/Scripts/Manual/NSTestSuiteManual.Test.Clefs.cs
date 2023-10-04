/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;


public partial class NSTestSuiteManual : MonoBehaviour
{
    Vector2 ClefsTest(NSStave stave, Vector2 shift)
    {
        float pixelShift = shift.x;
        foreach (ClefEnum clefEnum in System.Enum.GetValues(typeof(ClefEnum)))
        {
            //pixelShift += 75;
            //if (clefEnum == ClefEnum.Undefined)
            //	continue;

            //NSClef clef = stave.AddClef (clefEnum);
            //clef.PixelShift (new Vector2 (pixelShift, shift.y), true);
            //clef.Commit ();
        }
        return new Vector2(pixelShift, shift.y);
    }
}
