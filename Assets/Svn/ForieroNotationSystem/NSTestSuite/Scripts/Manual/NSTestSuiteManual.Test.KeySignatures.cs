/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;


public partial class NSTestSuiteManual : MonoBehaviour
{
    Vector2 KeySinaturesTest(NSStave stave, Vector2 shift)
    {
        float pixelShift = shift.x;
        foreach (KeySignatureEnum keySignatureEnum in System.Enum.GetValues(typeof(KeySignatureEnum)))
        {
            pixelShift += 150;
            if (keySignatureEnum == KeySignatureEnum.Undefined)
                continue;

            //NSKeySignature keySignature = stave.AddKeySignature (keySignatureEnum);
            //keySignature.PixelShift (new Vector2 (pixelShift, shift.y), true);
            //keySignature.Commit ();
        }
        return new Vector2(pixelShift, shift.y);
    }
}
