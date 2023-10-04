/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;


public partial class NSTestSuiteManual : MonoBehaviour
{
    Vector2 NotesTest(NSStave stave, Vector2 shift)
    {
        float pixelShift = shift.x;
        foreach (AccidentalEnum accidentalEnum in System.Enum.GetValues(typeof(AccidentalEnum)))
        {
            //if (accidentalEnum == AccidentalEnum.Undefined)
            //	continue;
            //foreach (StemEnum stemEnum in System.Enum.GetValues(typeof(StemEnum))) {
            //	if (stemEnum == StemEnum.Undefined)
            //		continue;
            //	foreach (NoteEnum noteEnum in System.Enum.GetValues(typeof(NoteEnum))) {
            //		if (noteEnum == NoteEnum.Undefined)
            //			continue;

            //		pixelShift += 100;
            //		NSNote note = stave.AddNote (noteEnum, BeamEnum.Undefined, stemEnum, accidentalEnum, 3);
            //		note.Commit ();
            //		note.PixelShift (new Vector2 (pixelShift, 0), true);
            //	}
            //}
        }
        return new Vector2(pixelShift, shift.y);
    }
}
