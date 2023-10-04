/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem;

namespace ForieroEngine.Music.MusicXML.Xsd
{
    public static partial class MusicXMLExtensions
    {
        public static int GetStaveNumber(this direction direction, int defaultValue = -1) => direction == null || !int.TryParse(direction.staff, out var temp) ? defaultValue : temp;

        public static metronome GetMetronome(this direction direction)
        {
            if (direction == null) { return null; }
            metronome metronome = null;
            foreach (var directionType in direction.directiontype)
            {
                metronome = directionType.ItemsElementName.ValueOf<metronome>(ItemsChoiceType8.metronome, directionType.Items);
                break;
            }
            return metronome;
        }

        public static PlacementEnum GetPlacement(this direction direction)
        {
            var r = PlacementEnum.Undefined;
            if (!direction.placementSpecified) return r;
            return direction.placement switch
            {
                abovebelow.above => PlacementEnum.Above,
                abovebelow.below => PlacementEnum.Below,
                _ => r
            };
        }

        public static pedal GetPedal(this direction direction)
        {
            if (direction == null) { return null; }
            pedal pedal = null;
            foreach (var directionType in direction.directiontype)
            {
                pedal = directionType.ItemsElementName.ValueOf<pedal>(ItemsChoiceType8.pedal, directionType.Items);
                break;
            }
            return pedal;
        }

        public static octaveshift GetOctaveShift(this direction direction)
        {
            if (direction == null) { return null; }
            octaveshift octaveshift = null;
            foreach (var directionType in direction.directiontype)
            {
                octaveshift = directionType.ItemsElementName.ValueOf<octaveshift>(ItemsChoiceType8.octaveshift, directionType.Items);
                break;
            }
            return octaveshift;
        }

        public static formattedtextid GetWords(this direction direction)
        {
            if (direction == null) { return null; }
            formattedtextid formattedtextid = null;
            foreach (var directionType in direction.directiontype)
            {
                formattedtextid = directionType.ItemsElementName.ValueOf<formattedtextid>(ItemsChoiceType8.words, directionType.Items);
                break;
            }
            return formattedtextid;
        }
    }
}
