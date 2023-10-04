/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem.Classes;

namespace ForieroEngine.Music.MusicXML.Xsd
{
    public static partial class MusicXMLExtensions
    {
        public static float GetDefaultTempo(this scorepartwise score, float defaultValue = 120)
        {
            if (score?.identification?.encoding == null) return defaultValue;

            var encoding = score.identification.encoding;
            var software = encoding.ItemsElementName.ValuesOf<string>(ItemsChoiceType.software, encoding.Items);
            foreach (var s in software) { if (s.ToLower().Contains("sibelius")) { defaultValue = 100; } }
            return defaultValue;
        }

        public static float GetTempoPerQuarterNote(this scorepartwise score, float defaultValue = 120f)
        {
            if (score?.part == null) return defaultValue;

            for (var p = 0; p < score.part.Length; p++)
            {
                var part = score.part[p];
                if (part.measure == null) continue;

                for (var m = 0; m < part.measure.Length; m++)
                {
                    var measure = part.measure[m];
                    var directions = measure.Items.ObjectsOfType<direction>();
                    foreach (var direction in directions)
                    {
                        var metronome = direction.GetMetronome();
                        if (metronome != null) { defaultValue = metronome.GetTempoPerQuarterNote(defaultValue); }
                        if (direction.sound != null) { defaultValue = direction.sound.GetTempoPerQuarterNote(defaultValue); }
                    }

                    break;
                }
            }

            return defaultValue;
        }

        public static void SetMetronomeMarkOptions(this scorepartwise score, NSMetronomeMark.Options options)
        {
            if (score?.part == null) return;

            for (var p = 0; p < score.part.Length; p++)
            {
                var part = score.part[p];
                if (part.measure == null) continue;

                for (var m = 0; m < part.measure.Length; m++)
                {
                    var measure = part.measure[m];
                    var directions = measure.Items.ObjectsOfType<direction>();
                    foreach (var direction in directions)
                    {
                        direction.GetMetronome()?.SetNSMetronomeMarkOptions(options);
                    }

                    break;
                }
            }
        }
    }
}
