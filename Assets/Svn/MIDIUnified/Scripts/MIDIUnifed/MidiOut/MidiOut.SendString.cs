/* Copyright © Marek Ledvina, Foriero s.r.o. */

using System;
using UnityEngine;

namespace ForieroEngine.MIDIUnified
{
    public static partial class MidiOut
    {
        public static class MidiStringParser
        {
            static string[] items;
            static CommandEnum command = CommandEnum.None;
            static int transposition = 0;
            static float masterVolume = 1f;

            public static void Parse(string s, float masterVolume = 1f, int transposition = 0)
            {
                if (string.IsNullOrEmpty(s)) return;

                MidiStringParser.transposition = transposition;
                MidiStringParser.masterVolume = masterVolume;

                items = s.Split(':');

                if (items.Length <= 1)
                {
                    Debug.LogError("Not enough parameters");
                    return;
                }

                switch (items[0])
                {
                    case "PERCUSSION" when items.Length == 3 || items.Length == 4: ParsePercussion(); break;
                    case "ON" when items.Length == 4 || items.Length == 5 || items.Length == 6: ParseNoteON(); break;
                    case "OFF" when items.Length == 3: ParseNoteOFF(); break;
                    case "INSTRUMENT" when items.Length == 3: ParseInstrument(); break;
                    default: Debug.LogError("Command not found : " + items[0]); break;
                }
            }

            static bool ParseChannel(string s, out int ch)
            {
                if (int.TryParse(s, out ch))
                {
                    if (ch.IsInChannelRange()) return true; else return false;
                }
                else return false;
            }
                      
            static void ParseStrings(string s, string defaultValue, out string[] result)
            {
                result = s.Split(',');
                for (int i = 0; i < result.Length; i++)
                {
                    if (string.IsNullOrEmpty(result[i])) result[i] = defaultValue;
                }
            }

            static void ParseTones(string s, out int[] result)
            {
                ParseStrings(s, "", out var strings);
                result = new int[strings.Length];
                for(int i=0;i<result.Length;i++)
                {
                    result[i] = MidiConversion.MidiStringToMidiIndex(strings[i]);
                    if (result[i] == 0)
                    {
                        Debug.LogError("Allowed tone strings : C3, Cb3, C#3");
                        return;
                    }
                    result[i] += transposition;
                }
            }

            static void ParsePercussions(string s, out PercussionEnum[] result)
            {
                ParseStrings(s, "", out var strings);
                result = new PercussionEnum[strings.Length];

                for (int i = 0; i < result.Length; i++)
                {
                    if (!int.TryParse(strings[i], out var percussion))
                    {
                        if (Enum.TryParse<PercussionEnum>(strings[i], true, out result[i]))
                        {
                            percussion = (int)result[i];
                        }
                        else
                        {
                            Debug.LogError("Could not parse percussions : " + strings[i]);
                        }
                    }

                    if (!Enum.IsDefined(typeof(PercussionEnum), result[i]))
                    {
                        Debug.LogError("Could not parse percussions : " + strings[i]);
                    }
                }
            }

            static void ParseFloats( string s, float defaultValue, out float[] result, int length)
            {
                var strings = s.Split(',');

                result = new float[length];

                string r = "";

                for (int i = 0; i < result.Length; i++)
                {
                    if (i >= 0 && i < strings.Length) r = strings[i];
                    else if (strings.Length > 0) r = strings[0];
                    else r = "";

                    if (string.IsNullOrEmpty(r))
                    {
                        result[i] = defaultValue;
                        continue;
                    }

                    if(!float.TryParse(r.Replace(".", ","), out result[i])){
                        result[i] = defaultValue;                 
                    }
                }
            }

            // COMMAND:INSTRUMENT:[VOLUME]:([START]) //
            // PERSCUSSION:LowBongo:0.8 //
            // PERCUSSION:LowBongo,HighBongo,HighBongo:0.8:0,0.2,0.4 //
            static void ParsePercussion()
            {
                ParsePercussions(items[1], out var percussions);
                ParseFloats(items[2], 0.8f, out var volumes, percussions.Length);

                if(items.Length == 4) {
                    ParseFloats(items[3], 0, out var starts, percussions.Length);
                    for (int i = 0; i < percussions.Length; i++)
                    {
                        MidiOut.SchedulePercussion(percussions[i], (volumes[i] * masterVolume).ToByteVolume(), starts[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < percussions.Length; i++)
                    {
                        MidiOut.Percussion(percussions[i], (volumes[i] * masterVolume).ToByteVolume());
                    }
                }
            }

            // COMMAND:CHANNEL:[TONE]:[VOLUME]:([DURATION]:([START])) //
            // ON:0:C3:0.8:0:2 //
            // ON:0:C3,E3,G3:1:0:2 //
            // ON:0:C3,E3,G3:0.8:2,1.8,1.6:0,0.2,0.4 //
            static void ParseNoteON()
            {
                if (!ParseChannel(items[1], out var channel))
                {
                    Debug.LogError("Could not parse channel : " + items[1]);
                    return;
                }

                command = CommandEnum.MIDI_NOTE_ON;

                ParseTones(items[2], out var tones);
                ParseFloats(items[3], 1, out var volumes, tones.Length);
                if (items.Length == 5)
                {
                    ParseFloats(items[4], 0, out var starts, tones.Length);
                    for (int i = 0; i < tones.Length; i++)
                    {
                       MidiOut.NoteDispatch(tones[i], aDelay: starts[i], anAttack: (volumes[i] * masterVolume).ToByteVolume());
                    }
                }
                else if (items.Length == 6)
                {
                    ParseFloats(items[4], 0, out var starts, tones.Length);
                    ParseFloats(items[5], 1, out var durations, tones.Length);
                    for (int i = 0; i < tones.Length; i++)
                    {
                        MidiOut.NoteDispatch(tones[i], aDuration: durations[i], aDelay: starts[i], anAttack: (volumes[i] * masterVolume).ToByteVolume());
                    }
                }
                else
                {
                    for (int i = 0; i < tones.Length; i++)
                    {
                        MidiOut.SendShortMessage((int)command, channel, tones[i], (volumes[i] * masterVolume).ToByteVolume(), -1);
                    }
                }
            }

            // COMMAND:CHANNEL:[TONE] //
            // OFF:0:C3 //
            // OFF:0:C3,E3,G3 //
            static void ParseNoteOFF()
            {
                if (!ParseChannel(items[1], out var channel))
                {
                    Debug.LogError("Could not parse channel : " + items[1]);
                    return;
                }

                command = CommandEnum.MIDI_NOTE_ON;

                ParseTones(items[2], out var tones);

                for (int i = 0; i < tones.Length; i++)
                {
                    MidiOut.SendShortMessage((int)command, channel, tones[i], 0, -1);
                }
            }

            // COMMAND:INSTRUMENT; //
            // INSTRUMENT:0:Marimba //
            // INSTRUMENT:0:50 //
            static void ParseInstrument()
            {
                if (!ParseChannel(items[1], out var channel))
                {
                    Debug.LogError("Could not parse channel : " + items[1]);
                    return;
                }

                command = CommandEnum.MIDI_PROGRAM_CHANGE;
                if (!int.TryParse(items[2], out var programeChange))
                {
                    if (Enum.TryParse<ProgramEnum>(items[2], true, out var p))
                    {
                        programeChange = (int)p;
                    }
                    else
                    {
                        Debug.LogError("Could not parse instrument : " + items[2]);
                        return;
                    }
                }

                if (!Enum.IsDefined(typeof(ProgramEnum), programeChange))
                {
                    Debug.LogError("Could not parse percussions : " + items[2]);
                    return;
                }

                MidiOut.SetInstrument(programeChange, channel);
            }
        }

        public static void SendString(string s, float masterVolume = 1f, int transposition = 0)
        {
            MidiStringParser.Parse(s, masterVolume, transposition);
        }
    }
}