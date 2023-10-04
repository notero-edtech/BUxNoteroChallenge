/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;


public partial class NSTestSuiteManual : MonoBehaviour
{
    Vector2 RestTest(NSStave stave, Vector2 shift)
    {
        float pixelShift = shift.x;
        foreach (RestEnum restEnum in System.Enum.GetValues(typeof(RestEnum)))
        {
            pixelShift += 75;
            if (restEnum == RestEnum.Undefined)
                continue;

            //NSRest rest = stave.AddRest (restEnum, 2);
            //rest.PixelShift (new Vector2 (pixelShift, shift.y), true);
            //rest.Commit ();
        }
        return new Vector2(pixelShift, shift.y);
    }
}
