/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTickerSystem : NS
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
                            internal static partial class Rest
                            {
                                internal static rest spRest = null;
                                internal static NSRest nsRest = null;

                                internal static void Parse(note note)
                                {
                                    spRest = spNote.ItemsElementName.ValueOf<rest>(ItemsChoiceType1.rest, spNote.Items);

                                    nsRest = nsStaveFixed.AddObject<NSRest>( PoolEnum.NS_FIXED);

                                    if (spRest.measureSpecified && spRest.measure == yesno.yes)
                                    {
                                        // difficulties ... rest can be specified as whole measure
                                        // actually without and type value 
                                        // so what is needed is to convert time signature => rest 
                                        // can be dotted like in 3/4 time signature 

                                        nsRest.options.CopyValuesFrom(Part.timeSignatureStruct.GetRestOptions());
                                        nsRest.Commit();
                                    }
                                    else
                                    {
                                        nsRest.options.restEnum = spNote.ToRestEnum();
                                        if (spNote.HasDot()) { nsRest.options.dotsCount = spNote.GetDotCount(); }
                                        nsRest.Commit();
                                    }

                                    if (!string.IsNullOrEmpty(spRest.displayoctave))
                                    {
                                        var y = nsRest.GetPositionOnStave((int)spRest.displaystep.ToNS(), int.Parse(spRest.displayoctave), nsStaveFixed.parsing.clef);
                                        nsRest.SetPositionY(y, true, true);
                                    }

                                    nsRest.PixelShiftX(Part.fbPixels, true);

                                    //nsRest.Update();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
