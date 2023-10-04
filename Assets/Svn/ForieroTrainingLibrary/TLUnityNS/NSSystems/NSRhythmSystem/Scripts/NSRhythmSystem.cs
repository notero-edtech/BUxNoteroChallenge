/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.Music.NotationSystem.Classes;
using ForieroEngine.Music.Training;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRhythmSystem : NS
    {
        public readonly NSRhythmSystemSpecificSettings nsRhythmSystemSettings;
        public NSRhythmSystem(NSBehaviour nsBehaviour, NSRhythmSystemSettings so)
            : base(nsBehaviour, so.nsSystemSettings, so.nsSystemSpecificSettings)
        {
            this.nsRhythmSystemSettings = so.nsSystemSpecificSettings;
        }

        public override NSObjectCheckEnum CheckAddObjectConstraints<T>(NSObject parent)
        {
            return NSObjectCheckEnum.Undefined;
        }

        public override NSObjectCheckEnum CheckSetObjectConstraints<T>(NSObject parent)
        {
            return NSObjectCheckEnum.Undefined;
        }

        public override void LoadMidi(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public override void LoadMusicXML(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public override void LoadMusic(float[] samples, int channels, float totalTime)
        {
            throw new NotImplementedException();
        }

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        public override void ZoomChanged(float zoom)
        {
            throw new NotImplementedException();
        }

        public int measuresPerRow = 2;

        List<NSStave> staves = new List<NSStave>();

        private NoteEnum ConvertNoteFrom(TL.Enums.NoteAndRestFlags duration)
        {
            switch (duration)
            {
                case TL.Enums.NoteAndRestFlags.Whole: return NoteEnum.Whole;
                case TL.Enums.NoteAndRestFlags.Half: return NoteEnum.Half;
                case TL.Enums.NoteAndRestFlags.Quarter: return NoteEnum.Quarter;
                case TL.Enums.NoteAndRestFlags.Item8th: return NoteEnum.Item8th;
                default: return NoteEnum.Item16th;
            }
        }

        public void LoadTLData(TL.Exercises.Data tlData)
        {
            this.DestroyChildren();

            int currentRow = 0;
            int currentCell = 0;

            int currentMeasureIndex = 0;
            NSStave stave = null;

            int tighRow = -1;
            NSTie tigh = null;

            var sideOffset = 5f;

            float measuresWidth = 0;
            float measureWidth = 0;

            var barLines = new List<NSBarLine>();
            NSBarLine barLine = null;

            while (true)
            {
                if (currentMeasureIndex >= tlData.measures.Count)
                {
                    break;
                }

                var currentMeasure = tlData.measures[currentMeasureIndex];

                if (currentCell == 0)
                {
                    // stave creation in the middle of the screen //
                    stave = this.AddObject<NSStave>();
                    stave.options.staveEnum = StaveEnum.One;
                    stave.Commit();

                    NSBracket._options.Reset();
                    stave.SetLeftSystemBracket(NSBracket._options);

                    NSBarLine._options.Reset();
                    NSBarLine._options.barLineEnum = BarLineEnum.Short;
                    stave.SetLeftBarLine(NSBarLine._options);
                    stave.SetRightBarLine(NSBarLine._options);

                    NSClef._options.Reset();
                    NSClef._options.clefEnum = ClefEnum.Undefined;
                    stave.SetClef(NSClef._options);

                    NSTimeSignature._options.Reset();
                    NSTimeSignature._options.timeSignatureEnum = TimeSignatureEnum.Normal;
                    NSTimeSignature._options.timeSignatureStruct = new TimeSignatureStruct(4, 4);
                    stave.SetTimeSignature(NSTimeSignature._options);

                    NSKeySignature._options.Reset();
                    NSKeySignature._options.keySignatureEnum = KeySignatureEnum.Undefined;
                    stave.SetKeySignature(NSKeySignature._options);

                    stave.Arrange();

                    stave.timeSignature.Shift(DirectionEnum.Down, true, 2);
                    stave.leftBarLine.Shift(DirectionEnum.Down, false, 3);
                    stave.rightBarLine.Shift(DirectionEnum.Down, false, 3);

                    // creating imaginary start barline that has origin at the right side of timesignature //

                    if (currentRow == 0)
                    {
                        barLine = stave.AddObject<NSBarLine>();
                        barLine.options.barLineEnum = BarLineEnum.Undefined;
                        barLine.Commit();
                        barLine.AlignTo(stave.leftBarLine, true, true);
                        barLine.AlignXTo(stave.timeSignature, true, true);
                        barLine.PixelShiftX(stave.timeSignature.numeratorText.text.GetPreferredWidth() / 2f, true);
                    }
                    else
                    {
                        stave.timeSignature.numeratorText.text.SetText("");
                        stave.timeSignature.denominatorText.text.SetText("");

                        barLine = stave.leftBarLine;
                    }

                    measuresWidth = barLine.Distance(stave.rightBarLine);
                    measureWidth = measuresWidth / measuresPerRow;

                    #region Barlines

                    barLines.Add(barLine);

                    for (int m = 0; m < measuresPerRow - 1; m++)
                    {
                        barLine = stave.AddObject<NSBarLine>();
                        barLine.options.barLineEnum = BarLineEnum.Short;
                        barLine.Commit();
                        barLine.AlignTo(barLines.First(), true, true);
                        barLine.PixelShiftX(measureWidth, true);
                        barLines.Add(barLine);
                    }

                    barLines.Add(stave.rightBarLine);

                    #endregion

                    //stave.AddObject<NSLedgerLines>();

                    staves.Add(stave);
                }

                // adding TLData //
                barLine = barLines[currentCell];

                float curPos = ns.LineSize * 2;

                foreach (var data in currentMeasure.notes)
                {
                    // creating helper object and positioning it so that I can AlignTo it with real objects //
                    var o = stave.AddObject<NSObject>();
                    o.Commit();
                    o.AlignTo(barLine, true, true);
                    o.PixelShiftX(curPos, true);
                    o.Shift(DirectionEnum.Up, true, 3);

                    if (data.isRest)
                    {
                        var rest = stave.AddObject<NSRest>();
                        rest.options.restEnum = (RestEnum)ConvertNoteFrom(data.duration);
                        rest.Commit();
                        rest.AlignTo(o, true, true);
                    }
                    else
                    {
                        int chordIndex = 0;
                        foreach (var midi in data.midis)
                        {
                            var note = stave.AddObject<NSNote>();
                            note.options.noteEnum = ConvertNoteFrom(data.duration);
                            if (chordIndex == 0) note.options.stemEnum = StemEnum.Up;

                            if (data.tupletEnum != Training.Classes.MidiStruct.TupletEnum.Undefined && data.tupletValue > 0)
                            {
                                switch (data.tupletEnum)
                                {
                                    case Training.Classes.MidiStruct.TupletEnum.Begin: note.options.beamEnum = BeamEnum.Start; break;
                                    case Training.Classes.MidiStruct.TupletEnum.Continue: note.options.beamEnum = BeamEnum.Continue; break;
                                    case Training.Classes.MidiStruct.TupletEnum.End: note.options.beamEnum = BeamEnum.End; break;
                                }
                            }

                            var dur = TL.Utilities.Rhythms.GetNoteRelativeDuration(data.duration);
                            if (dur < 1) // 1 = quarter relative duration
                            {
                                dur += 0;
                            }

                            note.Commit();
                            note.AlignTo(o, true, true);

                            // implementing tigh //
                            if (chordIndex == 0)
                            {
                                if (data.tighEnum == Training.Classes.MidiStruct.TighEnum.Begin)
                                {
                                    tigh = stave.AddObject<NSTie>();
                                    tigh.Commit();
                                    tigh.AlignTo(note, true, true);

                                    tighRow = currentRow;
                                }
                                else if (data.tighEnum == Training.Classes.MidiStruct.TighEnum.End)
                                {
                                    bool sameRow = (tighRow == currentRow);
                                    // we still need to take care tighs that are at the end or begining of measure row //
                                    tigh.options.length = sameRow ? tigh.Distance(note) : (ns.screenWidth / 2f - tigh.GetPositionX(true)) * 2f;
                                    tigh.options.height = -20;
                                    tigh.Commit();
                                    tigh.Shift(DirectionEnum.Down, true, 1);

                                    if (!sameRow)
                                    {
                                        float ofs = tigh.options.length;

                                        tigh = stave.AddObject<NSTie>();
                                        tigh.options.length = ofs;
                                        tigh.options.height = -20;
                                        tigh.Commit();
                                        tigh.AlignTo(note, true, true);
                                        tigh.Shift(DirectionEnum.Down, true, 1);
                                        tigh.PixelShiftX(-ofs, true);
                                    }
                                }
                            }

                            note.Shift(DirectionEnum.Up, true, chordIndex);

                            note.Update();

                            chordIndex++;
                        }
                    }

                    var duration = TL.Utilities.Rhythms.GetNoteRelativeDuration(data.duration) / 4.0;

                    if (data.tupletEnum != Training.Classes.MidiStruct.TupletEnum.Undefined && data.tupletValue > 0)
                    {
                        var tupletMult = 2 / (float)data.tupletValue;
                        duration *= tupletMult;
                    }

                    var curOfs = (float)((measureWidth) * duration);
                    curPos += curOfs;
                }

                currentMeasureIndex++;

                currentCell++;
                if (currentCell >= measuresPerRow)
                {
                    currentCell = 0;
                    currentRow++;
                    barLines.Clear();
                }
            }

            for (int i = 0; i < staves.Count; i++)
            {
                stave = staves[i];
                // moving the shole stave to Top //
                stave.SetPosition(stave.poolRectTransform.anchoredPosition + Vector2.up * stave.screenHeight / 2f, true, true);
                // shifting stave //
                stave.PixelShiftY(-100 - (i) * ns.nsSystemSettings.smuflFontSize * 1.5f, true);
            }
        }
    }
}
