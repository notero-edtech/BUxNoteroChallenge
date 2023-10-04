using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Notero.Unity.UI.VirtualPiano
{
    public class VirtualPianoHelper
    {
        public const int OctaveTotalKeys = 12;
        public const int OctaveWhiteKeys = 7;

        /// <summary>
        /// Indicate when to generate the black seed
        /// and this is how it looks like 🎹
        /// </summary>
        public static readonly int[] BlackLaneDivider = { 1, 3, 6, 8, 10 };

        public static readonly string[] NoteLabels = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        public static readonly string[] NoteLabelsFlat = { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };
        public static readonly string[] Accidental = { "\u266D", "\u266F" };

        public static int GetOctaveIndex(int midiId)
        {
            return Mathf.FloorToInt(midiId / OctaveTotalKeys);
        }

        public static int GetKeyIndex(int midiId)
        {
            return midiId % OctaveTotalKeys;
        }

        private static readonly string m_NoteNameFormat = "{0}{1}_{2}";

        public static string GetNoteName(int noteIndex, int minimumKey = 0)
        {
            string noteLabel = NoteLabels[GetKeyIndex(noteIndex)];
            noteIndex += minimumKey;
            return string.Format(m_NoteNameFormat, noteLabel, GetOctaveIndex(noteIndex), noteIndex);
        }

        public static string GetNoteLabel(int noteIndex, ShowAccidentalType showAccidental = ShowAccidentalType.Sharp, string separator = "/")
        {
            string noteLable = "";

            switch(showAccidental)
            {
                case ShowAccidentalType.Both:
                    if(BlackLaneDivider.Contains(GetKeyIndex(noteIndex)))
                    {
                        var note1 = NoteLabels[GetKeyIndex(noteIndex)].First().ToString() + Accidental[1];
                        var note2 = NoteLabelsFlat[GetKeyIndex(noteIndex)].First().ToString() + Accidental[0];
                        noteLable = note1 + separator + note2;
                    }
                    else
                    {
                        noteLable = NoteLabels[GetKeyIndex(noteIndex)];
                    }

                    break;
                case ShowAccidentalType.Flat:
                    noteLable = NoteLabelsFlat[GetKeyIndex(noteIndex)].First().ToString() + Accidental[0];
                    break;
                case ShowAccidentalType.Sharp:
                    noteLable = NoteLabels[GetKeyIndex(noteIndex)].First().ToString() + Accidental[1];
                    break;
            }

            return noteLable;
        }

        public static List<float> GetLanePosition(float containerSize, float whiteKeySize, float blackKeySize, int inputOctaves)
        {
            float whiteKeyAmount = inputOctaves * OctaveWhiteKeys;
            float posX = (containerSize - (whiteKeySize * whiteKeyAmount)) / 2;
            List<float> posXOfKeys = new List<float>();

            while(posXOfKeys.Count() < inputOctaves * OctaveTotalKeys)
            {
                posXOfKeys.Add(posX);
                posX += whiteKeySize;

                if(IsBlackKey(posXOfKeys.Count()))
                {
                    posXOfKeys.Add(posX - blackKeySize / 2);
                }
            }

            return posXOfKeys;
        }

        public static bool IsBlackKey(int midiId)
        {
            return BlackLaneDivider.Count(i => i == GetKeyIndex(midiId)) > 0;
        }
    }

    public enum PianoKeyType
    {
        WhiteKey,
        BlackKey
    }

    public enum ShowAccidentalType
    {
        Flat = 1,
        Sharp = 2,
        Both = 3
    }
}