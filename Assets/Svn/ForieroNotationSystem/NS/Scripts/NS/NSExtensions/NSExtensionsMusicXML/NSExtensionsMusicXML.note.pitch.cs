/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.MusicXML.Xsd;
using UnityEngine.Assertions;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSExtensionsMusicXML
    {
        public static int ToMidiIndex(this pitch pitch)
        {
            if (NS.debug)
            {
                Assert.IsNotNull(pitch);
                Assert.IsNotNull(pitch.octave);
                Assert.IsFalse(string.IsNullOrEmpty(pitch.octave));
            }
            return (int.Parse(pitch.octave) + 1) * 12 + pitch.step.ToMidiIndex() + pitch.GetAlter();
        }

        public static int GetAlter(this pitch pitch) => pitch.alterSpecified ? (int)System.Math.Round(pitch.alter) : 0;
        
        public static int ToMidiIndex(this step step) =>
            step switch
            {
                step.A => 9,
                step.B => 11,
                step.C => 0,
                step.D => 2,
                step.E => 4,
                step.F => 5,
                step.G => 7,
                _ => -1
            };

        public static int ToMidiIndex(this StepEnum step) =>
            step switch
            {
                StepEnum.A => 9,
                StepEnum.B => 11,
                StepEnum.C => 0,
                StepEnum.D => 2,
                StepEnum.E => 4,
                StepEnum.F => 5,
                StepEnum.G => 7,
                StepEnum.Undefined => -1,
                _ => -1
            };
        
        public static string i2stepstring(this int i) =>
            i switch
            {
                5 => "A",
                6 => "B",
                0 => "C",
                1 => "D",
                2 => "E",
                3 => "F",
                4 => "G",
                _ => ""
            };

        public static StepEnum i2stepenum(this int i) =>
            i switch
            {
                5 => StepEnum.A,
                6 => StepEnum.B,
                0 => StepEnum.C,
                1 => StepEnum.D,
                2 => StepEnum.E,
                3 => StepEnum.F,
                4 => StepEnum.G,
                _ => StepEnum.Undefined
            };

        public static int step2i(this string step)
        {
            if (step.Length != 1) return -1;
            return step[0] switch
            {
                'A' => 5,
                'B' => 6,
                'C' => 0,
                'D' => 1,
                'E' => 2,
                'F' => 3,
                'G' => 4,
                _ => -1
            };
        }

        public static string i2alter(this int i) =>
            i switch
            {
                -2 => "bb",
                -1 => "b",
                0 => "",
                1 => "#",
                2 => "##",
                _ => ""
            };

        public static int alter2i(this string alter) =>
            alter switch
            {
                "bb" => -2,
                "b" => -1,
                "" => 0,
                "#" => 1,
                "##" => 2,
                _ => 0
            };

        public static string string2accidental(this string accidental) =>
            accidental switch
            {
                "sharp" => ((char)0x0023).ToString(),
                "double-sharp" => ((char)0x2039).ToString(),
                "natural" => ((char)0x006E).ToString(),
                "flat" => ((char)0x0062).ToString(),
                "flat-flat" => ((char)0x222B).ToString(),
                _ => ""
            };

        public static StepEnum midi2step(this int aStep) =>
            aStep switch
            {
                9 => StepEnum.A,
                11 => StepEnum.B,
                0 => StepEnum.C,
                2 => StepEnum.D,
                4 => StepEnum.E,
                5 => StepEnum.F,
                7 => StepEnum.G,
                _ => StepEnum.Undefined
            };

        public static int step2midi(this StepEnum aStep) =>
            aStep switch
            {
                StepEnum.A => 9,
                StepEnum.B => 11,
                StepEnum.C => 0,
                StepEnum.D => 2,
                StepEnum.E => 4,
                StepEnum.F => 5,
                StepEnum.G => 7,
                StepEnum.Undefined => 0,
                _ => 0
            };
    }
}
