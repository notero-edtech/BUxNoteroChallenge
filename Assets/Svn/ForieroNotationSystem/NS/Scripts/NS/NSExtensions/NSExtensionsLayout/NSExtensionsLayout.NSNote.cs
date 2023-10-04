/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;
using UnityEngine.Assertions;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSExtensionsLayout
    {
        #region general rules

        // in engraved music, all beams are one half space thick //
        // the distance between multiple beams is also one half space //
        // this degree of beam thickness is designed to prevent beams from being obscured by the thinner staff lines //

        // when only two notes are to be beamed, stem length is the same as with flagged notes -> one full octave, or extending to the middle line if in the ledger lines. //
        // for each additional beam past two, stems are lenghtened one space //

        // when beaming two or more notes of extrememe intervalic distance, stem length may be compromised providein there's at least one full space between the innermost beam and the notehead nearest to it //

        // stems within a beamed group usually all go in the same direction //
        // in two note groupings, the note further from the middle line determines stem direction //
        // in beamed grouping of three or more notes, stem direction is determined by the placement of the majority of noteheads beight either above or below the middle line //

        // if there are an equal number of notes in the beamed group above the middle line as below, the note furthest from the middle line determines the stem direction //

        #endregion

        #region slant rules

        // beams must reflect the overall direction of pitch change within the beamed note group //
        // if the last note of the group to be bemed is higher than the first one, the beam(s) should slant upwards //
        // if the last note of the group is lower than the first, the beam)s) should slant downwards //

        // when most of the stems of the beamed note group end on the middle line of the staff, the beam(s) must still slant in the propper direction though only slightly and crossing the middle staff line //

        #endregion

        public static bool IsSamePitch(this NSNote note, NSNote otherNote)
        {
            if (note == null || otherNote == null) return false;
            return note.options.stepEnum == otherNote.options.stepEnum && note.options.alter == otherNote.options.alter;
        }

        public static StemEnum GetAutoStemEnum(this List<NSNote> notes)
        {
            var result = StemEnum.Undefined;
            Assert.IsNotNull(notes);

            if (!notes.First().stave) return StemEnum.Up;
            var stavePositionY = notes.First().stave.GetPositionY(false);
            float directionWeight = 0;

            foreach (var note in notes) { directionWeight += note.GetPositionY(false) - stavePositionY;            }
            if (Mathf.Approximately(directionWeight, 0f) || directionWeight > 0) { result = StemEnum.Down; }
            
            else { result = StemEnum.Up; }
            return result;
        }

        public static int GetHalfLineDistance(this NSNote note1, NSNote note2) => Mathf.RoundToInt(note1.GetPositionY(false).Distance(note2.GetPositionY(false)) / note1.ns.LineHalfSize);
        public static int GetLineDistance(this NSNote note1, NSNote note2) => Mathf.RoundToInt(note1.GetPositionY(false).Distance(note2.GetPositionY(false)) / note1.ns.LineSize);
        public static int GetStepDistance(this NSNote note1, NSNote note2) => Mathf.Abs(note2.GetStepPosition() - note1.GetStepPosition());
        public static int GetStepPosition(this NSNote note) => note.options.octave * 7 + (int)note.options.stepEnum;

        /// <summary>
        /// Stem the specified notes.
        /// </summary>
        /// <returns>The stem.</returns>
        /// <param name="notes">Notes.</param>
        public static void CalculateAndSetStemSizes(this List<NSNote> notes)
        {
            //(this.GetPositionY(false).Distance(stave.GetPositionY(false));
            Assert.IsNotNull(notes);
            Assert.IsTrue(notes.Count() > 1);

            if (notes.Count() <= 1) return;

            var lineSize = notes.First().ns.LineSize;
            var minStemSize = lineSize; // plus number of flags //
            var stavePositionY = notes.First().stave.GetPositionY(false);

            // are the first and last note the same pitch //
            var firstLastAreEqual = Mathf.Approximately(notes.First().GetPositionY(false), notes.Last().GetPositionY(false));
            // is first note lower than the last one //
            var firstIsLowerThanLast = notes.First().GetPositionY(false) < notes.Last().GetPositionY(false);

            StemEnum stemEnum = notes.First().noteStem.options.stemEnum;
            float directionWeight = 0;
            foreach (var note in notes)
            {
                directionWeight += note.GetPositionY(false) - stavePositionY;
                if (note.noteStem.options.stemEnum != stemEnum)
                {
                    stemEnum = StemEnum.Undefined;
                }
            }

            // are all stems in the same direction //
            var sameStemEnum = stemEnum != StemEnum.Undefined;

            // are generally the notes same distance from stave //
            var sameDirectionWeight = Mathf.Approximately(directionWeight, 0);
            var upDirectionWeight = directionWeight > 0;

            var firstLastLineDistance = notes.First().GetLineDistance(notes.Last());

            if (notes.Count() == 2)
            {
                var firstNoteStemSize = notes.First().noteStem.GetDefaultStaveRelatedSize();
                var firstNoteOnStave = notes.First().IsOnStave();
                var firstNoteBelowStave = notes.First().IsBelowStave();
                var firstNoteAboveStave = notes.First().IsAboveStave();

                var lastStemSize = notes.Last().noteStem.GetDefaultStaveRelatedSize();
                var secondtNoteStemSize = notes.Last().noteStem.GetDefaultStaveRelatedSize();
                var secondNoteOnStave = notes.Last().IsOnStave();
                var secondNoteBelowStave = notes.Last().IsBelowStave();
                var secondNoteAboveStave = notes.Last().IsAboveStave();

                var slant = notes.First().BeamSlant(notes.Last());
            }
            else
            {

            }
        }

        /// <summary>
        /// Gets the beam slant.
        /// </summary>
        /// <returns>The beam slant.</returns>
        /// <param name="notes">Notes.</param>
        public static float GetBeamSlant(this List<NSNote> notes)
        {
            return 0;
        }

        /// <summary>
        /// Beam the specified notes.
        /// </summary>
        /// <returns>The beam.</returns>
        /// <param name="notes">Notes.</param>
        public static void Beam(this List<NSNote> notes)
        {
            if (NS.debug) Assert.IsNotNull(notes);
            if (NS.debug) Assert.IsTrue(notes.Count() > 1);

            float angle = 0f;

            foreach (var note in notes)
            {
                switch (note.options.beamEnum)
                {
                    case BeamEnum.Start:
                        if (NS.debug) Assert.IsNull(note.prevNote);
                        if (NS.debug) Assert.IsNotNull(note.nextNote);
                        break;
                    case BeamEnum.Continue:
                        if (NS.debug) Assert.IsNotNull(note.prevNote);
                        if (NS.debug) Assert.IsNotNull(note.nextNote);

                        if (note.prevNote && note.prevNote.noteBeams)
                        {
                            angle = 0f;
                            for (int i = 0; i < note.prevNote.noteBeams.beams.Count; i++)
                            {
                                if (i < note.noteBeams.beams.Count)
                                {
                                    note.prevNote.noteBeams.beams[i].endPoint = note.prevNote.noteBeams.beams[i].rectTransform.GetAnchoredPosition(note.noteBeams.beams[i].vector.canvas.worldCamera, note.noteBeams.beams[i].rectTransform);
                                    angle = Vector2.Angle(new Vector2(note.prevNote.noteBeams.beams[i].endPoint.x, 0), note.prevNote.noteBeams.beams[i].endPoint);
                                }
                                else
                                {
                                    note.prevNote.noteBeams.beams[i].endPoint = new Vector2(note.prevNote.noteBeams.beams[i].endPoint.x, Mathf.Sin(angle * Mathf.Deg2Rad) * note.prevNote.noteBeams.beams[i].endPoint.x);
                                }
                            }

                        }
                        else
                        {
                            Debug.LogError("You are trying to set BeamEnum.Continue but prevNote.noteBeams is null!!!");
                        }
                        break;
                    case BeamEnum.End:
                        if (NS.debug) Assert.IsNotNull(note.prevNote);
                        if (NS.debug) Assert.IsNull(note.nextNote);

                        if (note.prevNote && note.prevNote.noteBeams)
                        {
                            angle = 0f;
                            for (int i = 0; i < note.prevNote.noteBeams.beams.Count; i++)
                            {
                                if (i < note.noteBeams.beams.Count)
                                {
                                    note.prevNote.noteBeams.beams[i].endPoint = note.prevNote.noteBeams.beams[i].rectTransform.GetAnchoredPosition(note.noteBeams.beams[i].vector.canvas.worldCamera, note.noteBeams.beams[i].rectTransform);
                                    angle = Vector2.Angle(new Vector2(note.prevNote.noteBeams.beams[i].endPoint.x, 0), note.prevNote.noteBeams.beams[i].endPoint);
                                }
                                else
                                {
                                    note.prevNote.noteBeams.beams[i].endPoint = new Vector2(note.prevNote.noteBeams.beams[i].endPoint.x, Mathf.Sin(angle * Mathf.Deg2Rad) * note.prevNote.noteBeams.beams[i].endPoint.x);
                                }
                            }

                            for (int i = 0; i < note.noteBeams.beams.Count; i++)
                            {
                                if (i < note.prevNote.noteBeams.beams.Count)
                                {
                                    note.noteBeams.beams[i].endPoint = Vector2.zero;
                                }
                                else
                                {
                                    note.noteBeams.beams[i].endPoint = new Vector2(-note.noteBeams.beams[i].endPoint.x, note.noteBeams.beams[i].endPoint.y);
                                    note.noteBeams.beams[i].endPoint = new Vector2(note.noteBeams.beams[i].endPoint.x, Mathf.Sin(angle * Mathf.Deg2Rad) * note.noteBeams.beams[i].endPoint.x);
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError("You are trying to set BeamEnum.End but prevNote.noteBeams is null!!!");
                        }
                        break;
                }
            }
        }
    }
}
