/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Linq;
using ForieroEngine.Music.NotationSystem;

namespace ForieroEngine.Music.MusicXML.Xsd
{
    public static partial class MusicXMLExtensions
    {
        public static float GetDuration(this note note)
        {
            if (note.ItemsElementName.Contains(ItemsChoiceType1.duration))
            { return Convert.ToSingle(note.Items[note.ItemsElementName.IndexOf(ItemsChoiceType1.duration)]); }
            else { return 0; }
        }

        public static float GetTime(this note note, float measureDivision, float tempo)
        {
            if (note.ItemsElementName.Contains(ItemsChoiceType1.duration)) 
            { return note.GetDuration() / measureDivision * 60.0f / tempo; } else { return 0; }
        }

        public static float GetDuration(this backup backup) => Convert.ToSingle(backup.duration);
        public static float GetTime(this backup backup, float measureDivision, float tempo) => Convert.ToSingle(backup.duration) / measureDivision * 60.0f / tempo;
        public static float GetDuration(this forward forward) => Convert.ToSingle(forward.duration);
        public static float GetTime(this forward forward, float measureDivision, float tempo) => Convert.ToSingle(forward.duration) / measureDivision * 60.0f / tempo;
        
        #region Is
        public static bool IsChord(this note note) => note.ItemsElementName.Contains(ItemsChoiceType1.chord);
        public static bool IsCue(this note note) => note.ItemsElementName.Contains(ItemsChoiceType1.cue);
        public static bool IsDuration(this note note) => note.ItemsElementName.Contains(ItemsChoiceType1.duration);
        public static bool IsGrace(this note note) => note.ItemsElementName.Contains(ItemsChoiceType1.grace);
        public static bool IsPitch(this note note) => note.ItemsElementName.Contains(ItemsChoiceType1.pitch);
        public static bool IsRest(this note note) => note.ItemsElementName.Contains(ItemsChoiceType1.rest);
        public static bool IsTie(this note note) => note.ItemsElementName.Contains(ItemsChoiceType1.tie);
        public static bool IsUnPitched(this note note) => note.ItemsElementName.Contains(ItemsChoiceType1.unpitched);
        #endregion
        
        #region Get
        //public static chord GetChord(this note note) => note?.ItemsElementName.ValueOf<chord>(ItemsChoiceType1.chord, note.Items);
        //public static cue GetCue(this note note) => note?.ItemsElementName.ValueOf<cue>(ItemsChoiceType1.cue, note.Items);
        //public static duration GetDuration(this note note) => note?.ItemsElementName.ValueOf<duration>(ItemsChoiceType1.duration, note.Items);
        public static grace GetGrace(this note note) => note?.ItemsElementName.ValueOf<grace>(ItemsChoiceType1.grace, note.Items);
        public static pitch GetPitch(this note note) => note?.ItemsElementName.ValueOf<pitch>(ItemsChoiceType1.pitch, note.Items);
        public static rest GetRest(this note note) => note?.ItemsElementName.ValueOf<rest>(ItemsChoiceType1.rest, note.Items);
        public static tie GetTie(this note note) => note?.ItemsElementName.ValueOf<tie>(ItemsChoiceType1.tie, note.Items);
        public static unpitched GetUnPitched(this note note) => note?.ItemsElementName.ValueOf<unpitched>(ItemsChoiceType1.unpitched, note.Items);
        #endregion
        
        public static bool HasDot(this note note) => note.dot != null && (note.dot.Length != 0);
        public static bool HasStem(this note note) => note.stem != null;

        public static OverUnderEnum GetTieOrientation(this note note)
        {
            if (!note.notations.HasTied()) return OverUnderEnum.Undefined;
            var tied = note.notations[0].Items.ObjectOfType<tied>();
            return (OverUnderEnum)tied.orientation;
        }
        
        public static fingering GetFingering(this note note)
        {
            if (!note.notations.HasTechnical()) return null;
            var t = note.notations[0].Items.ObjectOfType<technical>();
            return t.Items.ObjectOfType<fingering>();
        }

        public static TieTypeEnum GetTieType(this note note)
        {
            if (!note.notations.HasTied()) return TieTypeEnum.Undefined;
            var tied = note.notations[0].Items.ObjectOfType<tied>();
            return (TieTypeEnum)tied.type;
        }

        public static int GetTieNumber(this note note)
        {
            if (!note.notations.HasTied()) return 1;
            var tied = note.notations[0].Items.ObjectOfType<tied>();
            return string.IsNullOrEmpty(tied.number) ? 1 : int.Parse(tied.number);
        }

        public static int GetDotCount(this note note) => !note.HasDot() ? 0 : note.dot.Length;
        
        public static int GetStaveNumber(this note note, int defaultValue = 0)
        {
            if (note == null) return defaultValue; 
            if (int.TryParse(note.staff, out var temp)) return temp - 1;
            return defaultValue;
        }

        public static int GetVoiceNumber(this note note, int defaultValue = 0)
        {
            if (note == null) return defaultValue;
            if (int.TryParse(note.voice, out var temp)) return temp - 1;
            return defaultValue;
        }
    }
}
