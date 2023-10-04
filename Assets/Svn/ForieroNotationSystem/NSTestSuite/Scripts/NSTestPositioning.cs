/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Extensions;
using ForieroEngine.Music.NotationSystem.Classes;
using ForieroEngine.Music.NotationSystem.Systems;
using UnityEngine;

public class NSTestPositioning : MonoBehaviour
{

    public NSBehaviour nsBehaviour;

    public NSDefaultSystem ns;

    IEnumerator Start()
    {
        yield return null;
        yield return null;
        ns = nsBehaviour.ns as NSDefaultSystem;

        AddNotesTo(ns);

        var o = ns.AddObject<NSObject>();
        o.Commit();

        AddNotesTo(o);

        o.Shift(DirectionEnum.Up, true, 10);

        o = ns.AddObject<NSObject>();
        o.Commit();

        AddNotesTo(o);

        o.PixelShiftY(o.poolRectTransform.GetHeight() / 2f, true);
        // or you can call //
        o.SetPositionY(o.poolRectTransform.GetHeight() / 2f, true, true);

        o = ns.AddObject<NSObject>();
        o.Commit();

        AddNotesTo(o);

        o.SetScreenPosition(ScreenPositionEnum.BottomLeft, true, true);
    }

    void AddNotesTo(NSObject o)
    {
        var noteMiddle = o.AddObject<NSNote>(PoolEnum.NS_FIXED, PivotEnum.MiddleCenter);
        noteMiddle.options.noteEnum = NoteEnum.Whole;
        noteMiddle.Commit();

        var noteLeft = o.AddObject<NSNote>(PoolEnum.NS_FIXED, PivotEnum.MiddleLeft);
        noteLeft.options.noteEnum = NoteEnum.Whole;
        noteLeft.Commit();

        var noteRight = o.AddObject<NSNote>(PoolEnum.NS_FIXED, PivotEnum.MiddleRight);
        noteRight.options.noteEnum = NoteEnum.Whole;
        noteRight.Commit();

        var noteTop = o.AddObject<NSNote>(PoolEnum.NS_FIXED, PivotEnum.TopCenter);
        noteTop.options.noteEnum = NoteEnum.Whole;
        noteTop.Commit();

        var noteBottom = o.AddObject<NSNote>(PoolEnum.NS_FIXED, PivotEnum.BottomCenter);
        noteBottom.options.noteEnum = NoteEnum.Whole;
        noteBottom.Commit();
    }
}
