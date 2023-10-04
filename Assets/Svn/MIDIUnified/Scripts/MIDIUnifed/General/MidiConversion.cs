using System;
using UnityEngine;

namespace ForieroEngine.MIDIUnified
{
	public enum TheorySystemEnum
	{
		ToneNames,
		SolfegeFixed,
		SolfegeMovable,
		Undefined = int.MaxValue
	}
	
	public enum TonesSystemEnum
	{
		ToneNames = 0,
		Solfege = 1,
		Undefined = int.MaxValue
	}
	
	public enum SolfegeDisplayEnum
	{
		Syllabic = 0,
		Symbolic = 1,
		Undefined = int.MaxValue
	}

	public enum SolfegeSystemEnum
	{
		Fixed = 0,
		Movable = 1,
		Undefined = int.MaxValue
	}
	
	public static class MidiConversion
	{
		
		public static int MidiIndex(NoteEnum note, AccidentalEnum accidental, OctaveEnum octave) => (int)note + (int)accidental + octave.MidiIndex();
		public static int MidiIndex(this MidiEvents.NoteDef n) => MidiIndex(n.Note, n.Accidental, n.Octave);
		public static int MidiIndex(this OctaveEnum octave) => ((int) (octave == OctaveEnum.None ? OctaveEnum.Octave4 : octave) * 12) + 24;
		
		//FORMAT : C2 or C2# or C2## the same for flats//
		public static byte NoteToMidiIndex(string aMidiString)
		{
			byte result = 0;
			switch (aMidiString.Length)
			{
				case 2:
					result = (byte)(BaseMidiIndex(aMidiString[0]) + OctaveMidiIndex(byte.Parse(aMidiString[1].ToString())));
					break;
				case 3:
					result = (byte)(BaseMidiIndex(aMidiString[0]) + OctaveMidiIndex(byte.Parse(aMidiString[1].ToString())) + AccidentalShift(aMidiString[2].ToString()));
					break;
				case 4:
					result = (byte)(BaseMidiIndex(aMidiString[0]) + OctaveMidiIndex(byte.Parse(aMidiString[1].ToString())) + AccidentalShift(aMidiString.Substring(2)));
					break;
			}
			return result;
		}

		public static int GetByteVolume(float aVolume)
		{
			return (int)(Mathf.Clamp01(aVolume) * 127);
		}

		public static int ToByteVolume(this float volume)
		{
			return (int)(Mathf.Clamp01(volume) * 127);
		}

		public static int GetMidiVolume(float aVolume)
		{
			var result = 0;
			if (aVolume > 0)
			{
				if (aVolume < 1) { result = Mathf.RoundToInt(127f * aVolume); } else{ result = 127; }
			}
			else result = 0;
			return result;
		}

		public static int GetByteVolume(float aVolume, int aStreamVolume)
		{
			return (int)(Mathf.Clamp01(aVolume) * aStreamVolume);
		}

		public static byte OctaveMidiIndex(byte anOctaveIndex)
		{
			return (byte)(anOctaveIndex * 12);
		}

		public static int AccidentalShift(string anAccidental)
		{
			var result = 0;
			switch (anAccidental)
			{
				case "#": result = 1; break;
				case "##": result = 2; break;
				case "b": result = -1; break;
				case "bb": result = -2; break;
			}
			return result;
		}

		public static byte BaseMidiIndex(char aNoteName)
		{
			byte result = 0;
			switch (aNoteName)
			{
				case 'C': result = 0; break;
				case 'D': result = 2; break;
				case 'E': result = 4; break;
				case 'F': result = 5; break;
				case 'G': result = 7; break;
				case 'A': result = 9; break;
				case 'B': result = 11; break;
				default: result = 0; break;
			}
			return result;
		}

		public static int ToBaseMidiIndex(this int aMidiIndex) => aMidiIndex % 12;
		public static int BaseMidiIndex(int aMidiIdx) => aMidiIdx % 12;
		public static bool IsBlackKey(int aMidiIdx)
		{
			var baseIndex = BaseMidiIndex(aMidiIdx);
			var result = false;
			switch (baseIndex)
			{
				case 1: result = true; break;
				case 3: result = true; break;
				case 6: result = true; break;
				case 8: result = true; break;
				case 10: result = true; break;
			}
			return result;
		}

		public static bool IsWhiteKey(int aMidiIdx)
		{
			var baseIndex = BaseMidiIndex(aMidiIdx);
			var result = true;
			switch (baseIndex)
			{
				case 1: result = false; break;
				case 3: result = false; break;
				case 6: result = false; break;
				case 8: result = false; break;
				case 10: result = false; break;
			}
			return result;
		}

		public static int ToIntervalBaseIndex(this string anIntervalName)
		{
			var result = 0;
			switch (anIntervalName)
			{
				case "P1": result = 0; break;
				case "m2": result = 1; break;
				case "M2": result = 2; break;
				case "m3": result = 3; break;
				case "M3": result = 4; break;
				case "P4": result = 5; break;
				case "TT": result = 6; break;
				case "P5": result = 7; break;
				case "m6": result = 8; break;
				case "M6": result = 9; break;
				case "m7": result = 10; break;
				case "M7": result = 11; break;
				case "P8": result = 12; break;
			}
			return result;
		}

		public static IntervalEnum ToIntervalBaseEnum(this string anIntervalName)
		{
			var result = IntervalEnum.P1;
			switch (anIntervalName)
			{
				case "P1": result = IntervalEnum.P1; break;
				case "m2": result = IntervalEnum.m2; break;
				case "M2": result = IntervalEnum.M2; break;
				case "m3": result = IntervalEnum.m3; break;
				case "M3": result = IntervalEnum.M3; break;
				case "P4": result = IntervalEnum.P4; break;
				case "TT": result = IntervalEnum.TT; break;
				case "P5": result = IntervalEnum.P5; break;
				case "m6": result = IntervalEnum.m6; break;
				case "M6": result = IntervalEnum.M6; break;
				case "m7": result = IntervalEnum.m7; break;
				case "M7": result = IntervalEnum.M7; break;
				case "P8": result = IntervalEnum.P8; break;
			}
			return result;
		}

		public static ToneEnum ToToneEnum(this int aMidiIdx) => GetBaseToneFromMidiIndex(aMidiIdx);
		public static ToneEnum GetBaseToneFromMidiIndex(int aMidiIdx)
		{
			aMidiIdx = aMidiIdx.ToBaseMidiIndex();
			var result = ToneEnum.A;
			switch (aMidiIdx)
			{
				case 0: result = ToneEnum.C; break;
				case 1: result = ToneEnum.CSharpDFlat; break;
				case 2: result = ToneEnum.D; break;
				case 3: result = ToneEnum.DSharpEFlat; break;
				case 4: result = ToneEnum.E; break;
				case 5: result = ToneEnum.F; break;
				case 6: result = ToneEnum.FSharpGFlat; break;
				case 7: result = ToneEnum.G; break;
				case 8: result = ToneEnum.GSharpAFlat; break;
				case 9: result = ToneEnum.A; break;
				case 10: result = ToneEnum.ASharpBFlat; break;
				case 11: result = ToneEnum.B; break;
			}
			return result;
		}

		public static Color ToColor(this NoteEnum aNote)
		{
			switch (aNote)
			{
				case NoteEnum.None: return Color.black;
				case NoteEnum.A: return ToneEnum.A.ToColor();
				case NoteEnum.B: return ToneEnum.B.ToColor();
				case NoteEnum.C: return ToneEnum.C.ToColor();
				case NoteEnum.D: return ToneEnum.D.ToColor();
				case NoteEnum.E: return ToneEnum.E.ToColor();
				case NoteEnum.F: return ToneEnum.F.ToColor();
				case NoteEnum.G: return ToneEnum.G.ToColor();
			}

			return Color.black;
		}

		public static ToneEnum ToToneEnum(this NoteEnum aNote)
		{
			switch (aNote)
			{
				case NoteEnum.None: return ToneEnum.None;
				case NoteEnum.A: return ToneEnum.A;
				case NoteEnum.B: return ToneEnum.B;
				case NoteEnum.C: return ToneEnum.C;
				case NoteEnum.D: return ToneEnum.D;
				case NoteEnum.E: return ToneEnum.E;
				case NoteEnum.F: return ToneEnum.F;
				case NoteEnum.G: return ToneEnum.G;
			}

			return ToneEnum.None;
		}
		
		public static Color ToColor(this ToneEnum aTone)
		{
			Color result = Color.black;
			switch (aTone)
			{
				case ToneEnum.A:
					result = HexToRGB(74, 1, 200);
					break;
				case ToneEnum.ASharpBFlat:
					result = HexToRGB(49, 49, 236);
					break;
				case ToneEnum.B:
					result = HexToRGB(3, 146, 206);
					break;
				case ToneEnum.C:
					result = HexToRGB(74, 176, 2);
					break;
				case ToneEnum.CSharpDFlat:
					result = HexToRGB(188, 217, 2);
					break;
				case ToneEnum.D:
					result = HexToRGB(247, 233, 0);
					break;
				case ToneEnum.DSharpEFlat:
					result = HexToRGB(242, 185, 12);
					break;
				case ToneEnum.E:
					result = HexToRGB(251, 153, 2);
					break;
				case ToneEnum.F:
					result = HexToRGB(253, 91, 19);
					break;
				case ToneEnum.FSharpGFlat:
					result = HexToRGB(238, 0, 0);
					break;
				case ToneEnum.G:
					result = HexToRGB(202, 0, 69);
					break;
				case ToneEnum.GSharpAFlat:
					result = HexToRGB(141, 29, 175);
					break;
			}
			return result;
		}

		public static TheorySystemEnum theorySystem = TheorySystemEnum.ToneNames;

		public static string ToToneEnglishName(this int midiIndex, char separator = '/')
		{
			return GetToneEnglishNameFromMidiIndex(midiIndex, separator);
		}

		public static string GetToneNameFromMidiIndex(int aMidiIdx, char separator = '/', TheorySystemEnum aTheorySystem = TheorySystemEnum.ToneNames, KeySignatureEnum aKeySignature = KeySignatureEnum.CMaj_AMin)
		{
			string result = "";
			switch (aTheorySystem)
			{
				case TheorySystemEnum.ToneNames:
					result = GetToneEnglishNameFromMidiIndex(aMidiIdx, separator);
					break;
				case TheorySystemEnum.SolfegeFixed:
					result = GetToneSolfageNameFromMidiIndex(aMidiIdx, KeySignatureEnum.CMaj_AMin, separator);
					break;
				case TheorySystemEnum.SolfegeMovable:
					result = GetToneSolfageNameFromMidiIndex(aMidiIdx, aKeySignature, separator);
					break;
			}
			return result;
		}

		public static string GetToneEnglishNameFromMidiIndex(int aMidiIdx, char separator = '/')
		{
			aMidiIdx %= 12;

			string result = "";

			switch (aMidiIdx)
			{
				case 0:
					result = "C";
					break;
				case 1:
					result = "C#" + separator + "Db";
					break;
				case 2:
					result = "D";
					break;
				case 3:
					result = "D#" + separator + "Eb";
					break;
				case 4:
					result = "E";
					break;
				case 5:
					result = "F";
					break;
				case 6:
					result = "F#" + separator + "Gb";
					break;
				case 7:
					result = "G";
					break;
				case 8:
					result = "G#" + separator + "Ab";
					break;
				case 9:
					result = "A";
					break;
				case 10:
					result = "A#" + separator + "Bb";
					break;
				case 11:
					result = "B";
					break;
			}

			return result;
		}

		public static string GetToneSolfageNameFromMidiIndex(int aMidiIdx, KeySignatureEnum aKeySignature = KeySignatureEnum.CMaj_AMin, char separator = '/')
		{
			aMidiIdx = aMidiIdx.BaseMidiIndex();
			if (aKeySignature > KeySignatureEnum.CMaj_AMin) { aMidiIdx = (aMidiIdx + Mathf.Abs(aKeySignature.ToInt()) * 5).BaseMidiIndex(); }
			else if (aKeySignature < KeySignatureEnum.CMaj_AMin) { aMidiIdx = (aMidiIdx + Mathf.Abs(aKeySignature.ToInt()) * 7).BaseMidiIndex(); }
			else { }
			var result = "";
			switch (aMidiIdx)
			{
				case 0: result = "Do"; break;
				case 1: result = "Di" + separator + "Ra"; break;
				case 2: result = "Re"; break;
				case 3: result = "Ri" + separator + "Me"; break;
				case 4: result = "Mi"; break;
				case 5: result = "Fa"; break;
				case 6: result = "Fi" + separator + "Se"; break;
				case 7: result = "So"; break;
				case 8: result = "Si" + separator + "Le"; break;
				case 9: result = "La"; break;
				case 10: result = "Li" + separator + "Te"; break;
				case 11: result = "Ti"; break;
			}
			return result;
		}

		public static Color GetToneColorFromMidiIndex(int aMidiIdx)
		{
			aMidiIdx = aMidiIdx.ToBaseMidiIndex();
			var result = Color.black;
			switch (aMidiIdx)
			{
				case 0: result = ToColor(ToneEnum.C); break;
				case 1: result = ToColor(ToneEnum.CSharpDFlat); break;
				case 2: result = ToColor(ToneEnum.D); break;
				case 3: result = ToColor(ToneEnum.DSharpEFlat); break;
				case 4: result = ToColor(ToneEnum.E); break;
				case 5: result = ToColor(ToneEnum.F); break;
				case 6: result = ToColor(ToneEnum.FSharpGFlat); break;
				case 7: result = ToColor(ToneEnum.G); break;
				case 8: result = ToColor(ToneEnum.GSharpAFlat); break;
				case 9: result = ToColor(ToneEnum.A); break;
				case 10: result = ToColor(ToneEnum.ASharpBFlat); break;
				case 11: result = ToColor(ToneEnum.B); break;
			}
			return result;
		}
               
        public static Color HexToRGB(int r, int g, int b)
		{
			var red = (r) / 255f;
			var green = (g) / 255f;
			var blue = (b) / 255f;
			var finalColor = new Color();
			finalColor.r = red;
			finalColor.g = green;
			finalColor.b = blue;
			finalColor.a = 1;
			return finalColor;
		}
        		
		/// <summary>"C2", "C#3", "Cb4"
		/// number = octave index
		/// # | b = accidentals
		/// A,B,C,D,E,F,G = note names
		public static int MidiStringToMidiIndex(string s)
		{
			var midiShift = 0;
			var octaveIndex = 0;
			var noteIndex = BaseMidiIndex(s[0]);
			if (s.Contains("#"))
			{
				midiShift++;
				octaveIndex = int.Parse(s[2].ToString());
			}
			else if (s.Contains("b"))
			{
				midiShift--;
				octaveIndex = int.Parse(s[2].ToString());
			}
			else
			{
				octaveIndex = int.Parse(s[1].ToString());
			}
			return octaveIndex * 12 + noteIndex + midiShift;
		}

		public static double ToFreq(this int midiIndex)
		{
			var semitone_ratio = Math.Pow(2.0, 1.0 / 12.0);
			var c5 = 220.0 * Math.Pow(semitone_ratio, 3.0);
			var c0 = c5 * Math.Pow(0.5, 5.0);

			return c0 * Math.Pow(semitone_ratio, midiIndex);
		}
	}
}

