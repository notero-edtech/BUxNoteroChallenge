/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using ForieroEngine.Music.SMuFL;
using ForieroEngine.Music.NotationSystem.Extensions;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSNote : NSObjectSMuFL, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public static class Parsing
        {
            public class Part { public List<Voice> voices = new List<Voice>() { new Voice() }; }
            public static List<Part> parts = new List<Part>() { new Part() };
            public class Voice { public NSNote lastNote; public Chord lastChord; }
            public static void Reset() { parts = new List<Part>() { new Part() }; }
        }

        public class Chord
        {
            public NSNote rootNote = null;
            public List<NSNote> notes = new List<NSNote>();
        }

        public class Options : INSObjectOptions<Options>
        {
            public NoteEnum noteEnum = NoteEnum.Undefined;
            public BeamEnum beamEnum = BeamEnum.Undefined;
            public StemEnum stemEnum = StemEnum.Undefined;
            public StepEnum stepEnum = StepEnum.Undefined;
            public int octave = 4;
            public KeySignatureEnum keySignatureEnum = KeySignatureEnum.Undefined;
            public KeyModeEnum keyModeEnum = KeyModeEnum.Undefined;
            public int alter = 0;
            public bool autoStemEnum = true;
            public bool chordNote = false;
            public AccidentalEnum accidentalEnum = AccidentalEnum.Undefined;
            public YesNoEnum accidentalParenthesisEnum = YesNoEnum.Undefined;
            public ArticulationEnum articulationEnum = ArticulationEnum.Undefined;
            public PlacementEnum articulationPlacementEnum = PlacementEnum.Undefined;
            public FermataEnum fermataEnum = FermataEnum.Undefined;
            public PlacementEnum fermataPlacementEnum = PlacementEnum.Undefined;
            public int dotsCount = 0;
            public NoteHeadEnum noteHeadEnum = NoteHeadEnum.Undefined;
            public YesNoEnum filled = YesNoEnum.Undefined;
            public YesNoEnum parenthesis = YesNoEnum.Undefined;
            public NSFingering.Options? fingering = null;

            public void Reset()
            {
                noteEnum = NoteEnum.Undefined;
                beamEnum = BeamEnum.Undefined;
                stemEnum = StemEnum.Undefined;
                stepEnum = StepEnum.Undefined;
                octave = 4;
                keySignatureEnum = KeySignatureEnum.Undefined;
                keyModeEnum = KeyModeEnum.Undefined;
                alter = 0;
                autoStemEnum = true;
                chordNote = false;
                accidentalEnum = AccidentalEnum.Undefined;
                articulationEnum = ArticulationEnum.Undefined;
                articulationPlacementEnum = PlacementEnum.Undefined;
                fermataEnum = FermataEnum.Undefined;
                fermataPlacementEnum = PlacementEnum.Undefined;
                dotsCount = 0;
                noteHeadEnum = NoteHeadEnum.Undefined;
                filled = YesNoEnum.Undefined;
                parenthesis = YesNoEnum.Undefined;
                fingering = null;
            }

            public void CopyValuesFrom(Options o)
            {
                noteEnum = o.noteEnum;
                beamEnum = o.beamEnum;
                stemEnum = o.stemEnum;
                stepEnum = o.stepEnum;
                octave = o.octave;
                keySignatureEnum = o.keySignatureEnum;
                keyModeEnum = o.keyModeEnum;
                alter = o.alter;
                autoStemEnum = o.autoStemEnum;
                chordNote = o.chordNote;
                accidentalEnum = o.accidentalEnum;
                articulationEnum = o.articulationEnum;
                fermataEnum = o.fermataEnum;
                fermataPlacementEnum = o.fermataPlacementEnum;
                dotsCount = o.dotsCount;
                noteHeadEnum = o.noteHeadEnum;
                filled = o.filled;
                parenthesis = o.parenthesis;
                fingering = o.fingering;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public NSNoteStem noteStem;
        public NSAccidental accidental;
        public NSArticulation articulation;
        public NSFermata fermata;
        public NSNoteBeams noteBeams;
        public NSNoteFlag noteFlag;
        public NSDot dots;
        public NSTie tie;
        public NSObjectImage backgroundFill;
        public NSFingering fingering;

        public Chord chord = new Chord();

        public NSNote nextNote = null;
        public NSNote prevNote = null;

        public Vector2 size
        {
            get
            {
                if (options.noteEnum == NoteEnum.Undefined) return Vector2.one * 10f;
                else
                {
                    var boundingBoxEm = Metadata.GetGlyphBoundingBox(options.noteEnum.ToSMuFLUnicode());
                    return new Vector2(boundingBoxEm.widthEm, boundingBoxEm.heightEm) * NSSettingsStatic.smuflFontSize;
                }
            }
        }

        public override void Reset()
        {
            DestroyChildren();

            base.Reset();
            options.Reset();

            chord = new Chord();

            nextNote = null;
            prevNote = null;

            noteStem = null;
            accidental = null;
            noteBeams = null;
            noteFlag = null;
            dots = null;
            tie = null;
            fingering = null;

            snap.verticalDragEnum = DragEnum.Snap;
            snap.verticalDirection = VerticalDirectionEnum.Up;
            snap.verticalStep = ShiftStepEnum.Half;
            snap.horizontalDragEnum = DragEnum.None;
        }

        public override void Commit()
        {
            base.Commit();

            snap.verticalDragEnum = DragEnum.Snap;
            snap.verticalDirection = VerticalDirectionEnum.Up;
            snap.verticalStep = ShiftStepEnum.Half;
            snap.horizontalDragEnum = DragEnum.None;

            DestroyChildren();

            UpdateText();

            #region Parsing

            var partIndex = part ? part.options.index : 0;
            while (Parsing.parts.Count < part.options.index + 1) { Parsing.parts.Add(new Parsing.Part()); }
            while (Parsing.parts[partIndex].voices.Count < voiceNumber + 1) { Parsing.parts[partIndex].voices.Add(new Parsing.Voice()); }

            if (options.chordNote)
            {
                this.chord = Parsing.parts[partIndex].voices[voiceNumber].lastChord;

                if (NS.debug) Assert.IsNotNull(this.chord);
                if (NS.debug) Assert.IsTrue(options.stemEnum == StemEnum.Undefined);
                if (NS.debug) Assert.IsTrue(options.autoStemEnum == false);

                this.chord.notes.Add(this);
            }
            else
            {
                Parsing.parts[partIndex].voices[voiceNumber].lastChord = this.chord;
                this.chord.rootNote = this;
                this.chord.notes.Add(this);
            }

            #endregion

            if (options.noteEnum != NoteEnum.Undefined)
            {
                if ((options.stemEnum != StemEnum.Undefined || options.autoStemEnum) && options.noteEnum >= NoteEnum.Half)
                {
                    noteStem = this.AddObject<NSNoteStem>( pool);
                    noteStem.options.noteEnum = options.noteEnum;
                    noteStem.options.stemEnum = options.stemEnum;
                    noteStem.options.autoStemEnum = options.autoStemEnum;
                    noteStem.options.beamEnum = options.beamEnum;
                    noteStem.rectTransform.SetSiblingIndex(base.rectTransform.GetSiblingIndex() - 1);
                    noteStem.Commit();
                    Align(noteStem);
                    noteStem.PixelSnap();

                    noteBeams = noteStem.noteBeams;

                    if (options.beamEnum != BeamEnum.Undefined)
                    {
                        if (NS.debug) Assert.IsNotNull(noteBeams);
                    }

                    if (noteBeams)
                    {
                        switch (options.beamEnum)
                        {
                            case BeamEnum.Start:
                                prevNote = null;
                                nextNote = null;
                                Parsing.parts[partIndex].voices[voiceNumber].lastNote = this;
                                //if (NS.debug) Debug.LogFormat("Adding staveNumber = {0} voiceNumber = {1}", partIndex, voiceNumber);
                                break;
                            case BeamEnum.Continue:
                                prevNote = Parsing.parts[partIndex].voices[voiceNumber].lastNote;
                                if (NS.debug) Assert.IsNotNull(prevNote, string.Format("Previews beam is null for staveNumber = {0} voiceNumber = {1}", partIndex, voiceNumber));
                                if (NS.debug) Assert.IsTrue(prevNote.options.beamEnum == BeamEnum.Start || prevNote.options.beamEnum == BeamEnum.Continue, "Previous note must be BeamEnum.Start or BeamEnum.Continue");
                                prevNote.nextNote = this;
                                Parsing.parts[partIndex].voices[voiceNumber].lastNote = this;
                                break;
                            case BeamEnum.End:
                                prevNote = Parsing.parts[partIndex].voices[voiceNumber].lastNote;
                                if (NS.debug) Assert.IsNotNull(prevNote, string.Format("Previews beam is null for staveNumber = {0} voiceNumber = {1}", partIndex, voiceNumber));
                                if (NS.debug) Assert.IsTrue(prevNote.options.beamEnum == BeamEnum.Start || prevNote.options.beamEnum == BeamEnum.Continue, "Previous note must be BeamEnum.Start or BeamEnum.Continue");
                                prevNote.nextNote = this;
                                nextNote = null;
                                Parsing.parts[partIndex].voices[voiceNumber].lastNote = null;
                                break;
                        }
                    }
                    noteFlag = noteStem.noteFlag;
                }
            }

            if (options.accidentalEnum != AccidentalEnum.Undefined)
            {
                accidental = this.AddObject<NSAccidental>( pool);
                accidental.options.accidentalEnum = options.accidentalEnum;
                accidental.options.parenthesisEnum = options.accidentalParenthesisEnum;
                accidental.Commit();
                Align(accidental);
            }

            if (options.dotsCount > 0)
            {
                dots = this.AddObject<NSDot>(pool);
                dots.options.count = options.dotsCount;
                dots.Commit();
                Align(dots);
            }

            if (options.fingering != null)
            {
                fingering = this.AddObject<NSFingering>(pool);
                fingering.options = options.fingering;
                fingering.Commit();
                Align(fingering);
            }

            if (NSDisplaySettings.Articulations.Render && options.articulationEnum != ArticulationEnum.Undefined)
            {
                articulation = this.AddObject<NSArticulation>( pool);
                articulation.options.articulationEnum = options.articulationEnum;
                articulation.options.orientationEnum = options.articulationPlacementEnum.ToOrientationEnum();
                articulation.Commit();
                articulation.Shift(DirectionEnum.Up, true);
                Align(articulation);
            }

            if (NSDisplaySettings.Articulations.Render && options.fermataEnum != FermataEnum.Undefined)
            {
                fermata = this.AddObject<NSFermata>( pool);
                fermata.options.fermataEnum = options.fermataEnum;
                fermata.options.orientationEnum = options.articulationPlacementEnum.ToOrientationEnum();
                fermata.Commit();
                fermata.Shift(DirectionEnum.Up, true);
                Align(fermata);
            }

            if (backgroundFill) backgroundFill.SendVisuallyBack(this);
            
            this.SetVisible(NSDisplaySettings.Notes);
        }

        public void Update()
        {
            UpdateStem();

            Align(dots);

            if (NSDisplaySettings.Beams && noteBeams && noteBeams.options.beamEnum == BeamEnum.End)
            {
                GetBeamedNoteList().Beam();
            }
        }

        public void UpdateText()
        {
            var fontSize = NSSettingsStatic.smuflFontSize;

            switch (NSSettingsStatic.noteNamesEnum)
            {
                case ToneNamesEnum.Undefined:
                    text.SetFontSize(NSSettingsStatic.smuflFontSize);
                    backgroundFill.IsNotNull()?.Destroy();
                    backgroundFill = null;
                    break;
                case ToneNamesEnum.ToneNames:
                case ToneNamesEnum.SolfegeMovable:
                case ToneNamesEnum.SolfegeFixed:
                    if (options.noteEnum != NoteEnum.Undefined)
                    {
                        var unicode = NSSettingsStatic.noteNamesEnum.ToSMuFLUnicode(options.noteEnum);
                        var boundingBoxEm = Metadata.GetGlyphBoundingBox(unicode);
                        var s = new Vector2(boundingBoxEm.widthEm, boundingBoxEm.heightEm) * NSSettingsStatic.smuflFontSize;
                        fontSize = Mathf.RoundToInt(NSSettingsStatic.smuflFontSize * size.x / s.x) + 1;

                        Sprite sprite = null;

                        if (options.noteEnum == NoteEnum.Whole) sprite = NS.wholeNote;
                        else if (options.noteEnum == NoteEnum.Half) sprite = NS.halfNote;
                        else if (options.noteEnum > NoteEnum.Half) sprite = NS.quarterNote;

                        if (sprite && !backgroundFill)
                        {
                            backgroundFill = this.AddObject<NSObjectImage>(pool);
                            backgroundFill.image.sprite = sprite;
                            backgroundFill.image.color = Color.white;
                            backgroundFill.Commit();
                            backgroundFill.rectTransform.SetSize(size * scale * 0.9f);
                            backgroundFill.SendVisuallyBack(this);
                        }
                    }
                    break;
                default:
                    backgroundFill.IsNotNull()?.Destroy();
                    backgroundFill = null;
                    break;
            }

            text.SetFontSize(fontSize);

            rectTransform.SetSize(size);

            if(options.noteHeadEnum is NoteHeadEnum.Undefined or NoteHeadEnum.Normal){
                text.SetText(options.noteEnum.ToSMuFL(NSSettingsStatic.noteNamesEnum, options.stepEnum, options.keySignatureEnum, options.keyModeEnum, options.alter));
            } 
            else
            {
                text.SetText(options.noteHeadEnum.ToSMuFL());
            }
        }

        public void UpdateStem()
        {
            if (NS.debug) Assert.IsNotNull(chord.rootNote);

            var rootNote = chord.rootNote;
            var maxYNote = chord.notes.MaxBy(n => n.GetStepPosition());
            var minYNote = chord.notes.MinBy(n => n.GetStepPosition());
            chord.notes.Sort((x, y) => x.GetStepPosition().CompareTo(y.GetStepPosition()));

            var lastNoteShifted = false;

            if (chord.rootNote.noteStem)
            {
                if (chord.rootNote.options.autoStemEnum) chord.rootNote.noteStem.options.stemEnum = chord.notes.GetAutoStemEnum();

                if (Mathf.Approximately(chord.rootNote.noteStem.options.size, 0f))
                {
                    switch (chord.rootNote.noteStem.options.stemEnum)
                    {
                        case StemEnum.Down:
                            {
                                rootNote = maxYNote;
                                chord.rootNote.noteStem.AlignTo(minYNote, true, true);
                                chord.rootNote.noteStem.size = chord.rootNote.noteStem.GetDefaultStaveRelatedSize();
                            }
                            break;

                        case StemEnum.Up:
                            {
                                rootNote = minYNote;
                                chord.rootNote.noteStem.AlignTo(maxYNote, true, true);
                                chord.rootNote.noteStem.size = chord.rootNote.noteStem.GetDefaultStaveRelatedSize();
                            }
                            break;
                    }

                    chord.rootNote.noteStem.AlignTo(rootNote, true, true);
                    chord.rootNote.noteStem.size += minYNote.GetPositionY(false).Distance(maxYNote.GetPositionY(false));
                }
                else
                {
                    chord.rootNote.noteStem.size = chord.rootNote.noteStem.options.size;
                }

                switch (chord.rootNote.noteStem.options.stemEnum)
                {
                    case StemEnum.Down:
                        {
                            for (var i = chord.notes.Count() - 1; i >= 0; i--)
                            {
                                if (i == chord.notes.Count() - 1) continue;
                                
                                if (!lastNoteShifted && chord.notes[i].GetStepDistance(chord.notes[i + 1]) <= 1)
                                {
                                    chord.notes[i].SetPositionX(rootNote.GetPositionX(false) - size.x, true, false);
                                    lastNoteShifted = true;
                                    if (chord.rootNote.noteStem && i == 0 && chord.rootNote.options.noteEnum > NoteEnum.Half)
                                    {
                                        chord.rootNote.noteStem.size += ns.LineHalfSize;
                                    }
                                }
                                else
                                {
                                    lastNoteShifted = false;
                                    chord.notes[i].SetPositionX(rootNote.GetPositionX(false), true, false);
                                }
                            }
                        }
                        break;

                    case StemEnum.Up:
                        {
                            for (var i = 0; i < chord.notes.Count(); i++)
                            {
                                if (i == 0) continue;

                                if (!lastNoteShifted && chord.notes[i].GetStepDistance(chord.notes[i - 1]) <= 1)
                                {
                                    chord.notes[i].SetPositionX(rootNote.GetPositionX(false) + size.x, true, false);
                                    lastNoteShifted = true;
                                    if (chord.rootNote.noteStem && i == chord.notes.Count() - 1 && chord.rootNote.options.noteEnum > NoteEnum.Half)
                                    {
                                        chord.rootNote.noteStem.size += ns.LineHalfSize;
                                    }
                                }
                                else
                                {
                                    lastNoteShifted = false;
                                    chord.notes[i].SetPositionX(rootNote.GetPositionX(false), true, false);
                                }
                            }
                        }
                        break;
                }

                rootNote.Align(chord.rootNote.noteStem);
                chord.rootNote.noteStem.Update();
            }
            else
            {
                for (var i = 0; i < chord.notes.Count(); i++)
                {
                    if (i == 0) continue;

                    if (!lastNoteShifted && chord.notes[i].GetStepDistance(chord.notes[i - 1]) <= 1)
                    {
                        chord.notes[i].SetPositionX(rootNote.GetPositionX(false) + (options.noteEnum > NoteEnum.Half ? size.x : size.x * 0.85f), true, false);
                        lastNoteShifted = true;
                    }
                    else
                    {
                        lastNoteShifted = false;
                        chord.notes[i].SetPositionX(rootNote.GetPositionX(false), true, false);
                    }
                }
            }
        }

        List<NSNote> GetBeamedNoteList()
        {
            var note = this;

            while (note.prevNote != null)
            {
                note = note.prevNote;
            }

            List<NSNote> notes = new List<NSNote>();
            notes.Add(note);

            while (note.nextNote != null)
            {
                notes.Add(note.nextNote);
                note = note.nextNote;
            }

            return notes;
        }

        #region INSColorable implementation

        public new void SetColor(Color color)
        {
            this.text.SetColor(color);
        }

        public new Color GetColor()
        {
            return this.text.GetColor();
        }

        #endregion

        #region align

        public virtual void Align(NSAccidental accidental)
        {
            if (!accidental) return;

            accidental.AlignTo(this, true, true);

            switch (options.noteEnum)
            {
                case NoteEnum.Breve:
                    accidental.Shift(DirectionEnum.Left, true, 1, ShiftStepEnum.Whole, true);
                    accidental.Shift(DirectionEnum.Left, true, 1, ShiftStepEnum.Quarter, true);
                    break;
                case NoteEnum.Whole:
                    accidental.Shift(DirectionEnum.Left, true, 1, ShiftStepEnum.Whole, true);
                    break;
                default:
                    accidental.Shift(DirectionEnum.Left, true, 1, ShiftStepEnum.Half, true);
                    accidental.Shift(DirectionEnum.Left, true, 1, ShiftStepEnum.Quarter, true);
                    break;
            }
        }

        public virtual void Align(NSFermata fermata)
        {

        }

        public virtual void Align(NSArticulation articulation)
        {

        }

        public virtual void Align(NSFingering fingering)
        {
            
        }

        public virtual void Align(NSDot dots)
        {
            if (!dots) return;

            dots.AlignTo(this, true, true);

            switch (options.noteEnum)
            {
                case NoteEnum.Breve:
                    dots.Shift(DirectionEnum.Right, true, 1, ShiftStepEnum.Whole, true);
                    dots.Shift(DirectionEnum.Right, true, 1, ShiftStepEnum.Quarter, true);
                    break;
                case NoteEnum.Whole:
                    dots.Shift(DirectionEnum.Right, true, 1, ShiftStepEnum.Whole, true);
                    break;
                default:
                    dots.Shift(DirectionEnum.Right, true, 1, ShiftStepEnum.Half, true);
                    dots.Shift(DirectionEnum.Right, true, 1, ShiftStepEnum.Quarter, true);
                    break;
            }

            if (stave && dots.IsOnStaveLine())
            {
                dots.Shift(DirectionEnum.Up, true, 1, ShiftStepEnum.Half, true);
            }
        }

        public virtual void Align(NSNoteStem stem)
        {
            if (!stem) return;

            stem.AlignTo(this, true, true);

            switch (stem.options.stemEnum)
            {
                case StemEnum.Up:
                    stem.PixelShiftX(size.x / 2f - ns.StemWidth, false, true);
                    stem.rectTransform.AlignAnchoredPositionToPixels(PixelAlignEnum.Ceil);
                    break;

                case StemEnum.Down:
                    stem.PixelShiftX(-size.x / 2f + ns.StemWidth, false, true);
                    stem.rectTransform.AlignAnchoredPositionToPixels(PixelAlignEnum.Floor);
                    break;
            }
        }

        #endregion

    }
}
