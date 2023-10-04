/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

namespace ForieroEngine.Music.MusicXML.Xsd
{
    public static partial class MusicXMLExtensions
    {
        public static float GetTempoPerQuarterNote(this sound sound, float defaultValue = 120f) => sound is not { tempoSpecified: true } ? defaultValue : (float)sound.tempo;

        public static float GetTempoPerQuarterNote(this metronome metronome, float defaultValue = 120.0f)
        {
            if (metronome == null) { return defaultValue; }

            var beatsPerMinute = metronome.GetBeatsPerMinute();
            var beatUnit = metronome.GetBeatUnit();
            var beatUnitDots = metronome.GetBeatUnitDots();

            var dotsCoefficient = beatUnitDots.GetDotsCoefficient();

            var distance = Mathf.Abs(notetypevalue.quarter - beatUnit);

            if (beatUnit > notetypevalue.quarter) { return beatsPerMinute * (distance * dotsCoefficient) * 2f; }
            else if (beatUnit < notetypevalue.quarter) { return beatsPerMinute * (distance * dotsCoefficient) / 2f; }
            else { return beatsPerMinute * dotsCoefficient; }
        }

        public static void SetNSMetronomeMarkOptions(this metronome metronome, NSMetronomeMark.Options options)
        {
            if (metronome == null) return;
            options.beatsPerMinute = metronome.GetBeatsPerMinute(options.beatsPerMinute);
            options.noteEnum = metronome.GetBeatUnit().ToNoteEnum();
            options.dots = metronome.GetBeatUnitDots();
        }

        public static float GetBeatsPerMinute(this metronome metronome, float defaultValue = 120.0f)
        {
            if (metronome == null) { return defaultValue; }
            if (!metronome.ItemsElementName.Contains(ItemsChoiceType7.perminute)) return defaultValue;
            var value = metronome.ItemsElementName.ValueOf<perminute>(ItemsChoiceType7.perminute, metronome.Items).Value;
            if (float.TryParse(value, out var temp)) { return temp; }
            Debug.LogError("NS perminute does parsing error " + value);
            return defaultValue;
        }

        public static notetypevalue GetBeatUnit(this metronome metronome, notetypevalue defaultValue = notetypevalue.quarter)
        {
            if (metronome == null) { return defaultValue; }
            if (!metronome.ItemsElementName.Contains(ItemsChoiceType7.beatunit)) return defaultValue;
            return metronome.ItemsElementName.ValueOf<notetypevalue>(ItemsChoiceType7.beatunit, metronome.Items);
        }

        public static int GetBeatUnitDots(this metronome metronome, int defaultValue = 0)
        {
            return metronome == null ? defaultValue : metronome.ItemsElementName.ValuesOf<empty>(ItemsChoiceType7.beatunitdot, metronome.Items).Count;
        }
    }
}
