/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;


public partial class NSTestSuiteManual : MonoBehaviour
{
    Vector2 BarLinesTest(NSStave stave, Vector2 shift)
    {
        float pixelShift = shift.x;
        foreach (BarLineEnum barlineEnum in System.Enum.GetValues(typeof(BarLineEnum)))
        {
            if (barlineEnum == BarLineEnum.Undefined)
                continue;

            //NSBarLine barline = stave.AddBarLine(barlineEnum);
            //barline.PixelShift(new Vector2(pixelShift, shift.y), true);
            //barline.Commit();
            //pixelShift += 50;
        }

        pixelShift += 50;
        pixelShift += 50;
        pixelShift += 50;

        for (int i = 0; i < 10; i++)
        {
            //NSBarLineVector barLineVector = part.AddBarLine();
            //barLineVector.Commit();
            //barLineVector.PixelShiftX(pixelShift, true);
            //pixelShift += 50;
            //barLineVector.rectTransform.AlignAnchoredPositionToPixels(PixelAlignEnum.Round);
        }

        return new Vector2(pixelShift, shift.y);
    }
}
