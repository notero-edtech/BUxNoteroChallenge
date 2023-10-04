/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;
using ForieroEngine.Music.NotationSystem.Systems;
using UnityEngine;
using UnityEngine.Assertions;

public partial class NSTestSuiteManual : MonoBehaviour
{
    NSRollingLeftRightSystem ns = null;

    public void Reset()
    {

        xPosition = 0f;
        yPosition = 0f;
    }

    NSPart part = null;
    NSStave stave = null;

    float xPosition = 0f;
    float yPosition = 0f;

    float staveDistance = 150f;

    public void Create()
    {
        Init();

        if (nsBehaviour.ns is NSRollingLeftRightSystem)
        {
            ns = nsBehaviour.ns as NSRollingLeftRightSystem;

            for (int i = 0; i < renderTestCount; i++)
            {
                foreach (KeyValuePair<Test, bool> kv in tests)
                {
                    if (kv.Value)
                    {
                        part = AddPart();
                        stave = AddStave();

                        var t = stave.AddObject<NSObjectText>();
                        t.text.SetText(kv.Key.ToString());
                        t.Shift(DirectionEnum.Up, true, 5);

                        RenderTest(kv.Key);

                        stave.PixelShiftY(-yPosition, true);
                        yPosition += staveDistance;
                    }
                }
            }

            ns.Commit();
        }
        else
        {
            Debug.LogError("NSSystem is not NSRollingLeftRightSystem");
        }
    }

    private void RenderTest(Test test)
    {
        switch (test)
        {
            case Test.Beams: break;
            case Test.Barlines: BarLinesTest(stave, Vector2.zero); break;
            case Test.Clefs: ClefsTest(stave, Vector2.zero); break;
            case Test.KeySignatures: KeySinaturesTest(stave, Vector2.zero); break;
            case Test.Notes: NotesTest(stave, Vector2.zero); break;
            case Test.Rests: RestTest(stave, Vector2.zero); break;
            case Test.TimeSignatures: TimeSignatureTest(stave, Vector2.zero); break;
        }
    }

    private NSPart AddPart()
    {
        NSPart._options.Reset();
        NSPart._options.id = "";
        NSPart part = ns.AddPart(NSPart._options, PivotEnum.MiddleCenter, PoolEnum.NS_FIXED);
        part.rectTransform.gameObject.name = typeof(NSPart).Name;
        return part;
    }

    private NSStave AddStave()
    {
        NSStave._options.Reset();
        NSStave._options.staveEnum = StaveEnum.Five;
        NSStave stave = part.AddStave(NSStave._options, PivotEnum.MiddleCenter, PoolEnum.NS_FIXED);
        stave.Commit();

        //stave.SetBracket(100);

        //stave.SetTimeSignature(numerator, denominator);
        //stave.SetClef(clefEnum);
        //stave.SetKeySignature(keySignatureEnum);
        return stave;
    }
}
