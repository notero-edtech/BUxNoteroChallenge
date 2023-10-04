/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine.Assertions;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSPageLayoutTickerSystem : NS
    {
        static partial class ScorePartwise
        {
            internal static partial class Part
            {
                internal static partial class Measure
                {
                    internal static partial class Item
                    {
                        internal static partial class NoteRest
                        {
                            internal static note spNote = null;

                            static int staveNumber = 0;
                            static int voiceNumber = 0;
                            static NSStave nsStaveFixed = null;

                            static float timeInMeasure = 0;
                            static float timePixels = 0;

                            internal static void Parse(note note)
                            {
                                spNote = note;

                                if (spNote != null)
                                {
                                    staveNumber = spNote.GetStaveNumber();
                                    if (NS.debug) Assert.IsTrue(staveNumber >= 0 && staveNumber < nsPart.parsing.staves.Count, string.Format("staveNumber = {0} staves.Count = {1}", staveNumber, nsPart.parsing.staves.Count));

                                    if (!spNote.IsChord()) voiceNumber = spNote.GetVoiceNumber();
                                    if (NS.debug) Assert.IsTrue(voiceNumber >= 0 && voiceNumber < Part.voices.Count, string.Format("voiceNumber = {0}  Part.voices.Count = {1}", voiceNumber, Part.voices.Count));

                                    nsStaveFixed = nsPart.parsing.staves[staveNumber];

                                    if (spNote.IsCue() || spNote.IsGrace())
                                    {
                                        return;
                                    }

                                    timeInMeasure = spNote.GetTime(spAttributesDivisions, tempo);
                                    timePixels = timeInMeasure
                                        * NSPlayback.NSRollingPlayback.pixelsPerSecond
                                        * (NSSettingsStatic.canvasRenderMode == CanvasRenderMode.Screen ? 1f / NSPlayback.Zoom : 1f);                                        

                                    if (spNote.IsRest())
                                    {
                                        Rest.Parse(spNote);
                                    }
                                    else
                                    {
                                        Note.Parse(spNote);
                                    }

                                    if (!spNote.IsChord() && !spNote.IsCue() && !spNote.IsGrace())
                                    {
                                        Part.measureTime.divisions += spNote.GetDuration();
                                        Part.measureTime.time += spNote.GetTime(spAttributesDivisions, tempo); ;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
