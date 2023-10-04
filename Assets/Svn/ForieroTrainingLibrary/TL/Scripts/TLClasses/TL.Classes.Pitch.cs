/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.MIDIUnified;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training.Classes.Pitches
{
    [Serializable]
    public struct PitchRange
    {
        public int low;
        public int high;

        // those should come from another place later....
        public static string customLow = "G3";
        public static string customHigh = "Gb4";

        public static string usersLow = "G3";
        public static string usersHigh = "Gb4";

        public PitchRange(string lowName, string highName)
        {
            this.low = MidiConversion.MidiStringToMidiIndex(lowName);
            this.high = MidiConversion.MidiStringToMidiIndex(highName);
        }

        public static PitchRange FromPreset(TL.Enums.PitchRangesPresetEnum preset)
        {
            switch (preset)
            {
                case TL.Enums.PitchRangesPresetEnum.Custom:
                    return new PitchRange(customLow, customHigh);

                //case TL.Enums.PitchRangesPresetEnum.UserVocalRange:
                //return new PitchRange(usersLow, usersHigh);

                case TL.Enums.PitchRangesPresetEnum.Soprano:
                    return new PitchRange("C4", "A5");

                case TL.Enums.PitchRangesPresetEnum.MezzoSoprano:
                    return new PitchRange("A3", "F5");

                case TL.Enums.PitchRangesPresetEnum.Alto:
                    return new PitchRange("G3", "E5");

                case TL.Enums.PitchRangesPresetEnum.ContraAlto:
                    return new PitchRange("F3", "D5");

                case TL.Enums.PitchRangesPresetEnum.CounterTenor:
                    return new PitchRange("G3", "E5");

                case TL.Enums.PitchRangesPresetEnum.Tenor:
                    return new PitchRange("B2", "G4");

                case TL.Enums.PitchRangesPresetEnum.Baritone:
                    return new PitchRange("G2", "E4");

                case TL.Enums.PitchRangesPresetEnum.Bass:
                    return new PitchRange("E2", "C4");

                case TL.Enums.PitchRangesPresetEnum.PianoInstrument:
                    return new PitchRange("A0", "C8");

                case TL.Enums.PitchRangesPresetEnum.GuitarInstrument:
                    return new PitchRange("E2", "B6");

                case TL.Enums.PitchRangesPresetEnum.BassInstrument:
                    return new PitchRange("G3", "Gb4");

                default:
                    return new PitchRange();
            }
        }

        public int GetRandomPitch()
        {
            return UnityEngine.Random.Range(low, high);
        }

        public int GetRandomPitchFromTone(TL.Enums.ToneEnum tone)
        {
            int dir = TL.Utilities.RandomBool() ? 1 : -1;
            int pitch = GetRandomPitch();

            while (TL.Utilities.Midi.FromIndex(pitch) != tone)
            {
                pitch += dir;

                if (pitch > high)
                {
                    pitch = low;
                }
                else
                if (pitch < low)
                {
                    pitch = high;
                }
            }

            return pitch;
        }

        public int[] GetPitchRangeFromTones(TL.Enums.ToneEnum[] tones)
        {
            TL.Enums.ToneEnum firstTone = tones[0];
            TL.Enums.ToneEnum curTone = firstTone;
            TL.Enums.ToneEnum prevTone;

            int startPitch = GetRandomPitchFromTone(firstTone);

            // check if using starting pitch will fit inside the pitch range
            /*if (startPitch + distance <= this.high)
			{
				return null;
			}*/

            var result = new int[tones.Length];
            result[0] = startPitch;

            // start filling pitch array, starting from picked initial pitch
            curTone = firstTone;
            for (int i = 1; i < tones.Length; i++)
            {
                prevTone = curTone;
                curTone = tones[i];
                startPitch += TL.Utilities.Theory.GetDistanceBetweenTones(prevTone, curTone);
                result[i] = startPitch;
            }

            return result;
        }

    }
}

