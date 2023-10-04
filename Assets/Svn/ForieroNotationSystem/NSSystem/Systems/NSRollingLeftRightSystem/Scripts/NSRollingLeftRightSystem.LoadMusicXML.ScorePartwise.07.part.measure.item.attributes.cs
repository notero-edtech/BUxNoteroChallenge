/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Linq;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;
using ForieroEngine.Music.NotationSystem.Extensions;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Systems
{
	public partial class NSRollingLeftRightSystem : NS
	{
		static partial class ScorePartwise
		{
			internal static partial class Part
			{
				internal static partial class Measure
				{
					internal static partial class Item
					{
						internal static partial class Attributes
						{
							internal static attributes spAttributes = null;

							internal static void Parse(attributes attributes)
							{
								spAttributes = attributes;

								spAttributesDivisions = spAttributes.GetDivisions(spAttributesDivisions);
								spAttributesStaveCount = spAttributes.GetStaveCount(nsPart.parsing.staves.Count);
								spAttributesStaveCount = spAttributesStaveCount == 0 ? 1 : spAttributesStaveCount;

								if (nsPart.parsing.staves.Count >= spAttributesStaveCount) return;
								
								while (nsPart.parsing.staves.Count < spAttributesStaveCount)
								{
									NSStave._options.Reset();
									NSStave._options.id = (nsPart.parsing.staves.Count + 1).ToString();
									NSStave._options.index = nsPart.parsing.staves.Count;
									NSStave._options.staveEnum = StaveEnum.Five;
									NSStave._options.background = true;
									NSStave._options.systemBracket = !nsPart.parsing.staves.Any();
									NSStave._options.lineDistance = ns.StaveLineDistance;
									
									if (ns.specificSettings.stretch == Stretch.Auto) 
										NSStave._options.width =  ns.fixedCanvasRT.GetWidth() - ns.specificSettings.autoMargin * 2f;
									else NSStave._options.width = ns.specificSettings.width;
									
									var nsStave = nsPart.AddStave(NSStave._options, PivotEnum.MiddleCenter, PoolEnum.NS_FIXED);
									nsPart.parsing.staves.Add(nsStave);

									if (nsPart.parsing.staves.Count == 1)
									{
										nsStave.metronomeMark = nsStave.AddObject<NSMetronomeMark>( PoolEnum.NS_FIXED);
										nsStave.metronomeMark.options.CopyValuesFrom(Part.metronomeMarkOptions);
										nsStave.metronomeMark.Commit();

										nsPart.parsing.metronomeMark.CopyValuesFrom(Part.metronomeMarkOptions);

										var metronomeMark = nsStave.AddObject<NSMetronomeMark>( PoolEnum.NS_MOVABLE);
										metronomeMark.options.CopyValuesFrom(Part.metronomeMarkOptions);
										metronomeMark.Commit();

										metronomeMark.passable = true;

										metronomeMark.pixelTime = fbPixels;

										metronomeMark.SetPositionY(nsStave.topEdge + ns.LineSize * 3, true, true);
										metronomeMark.SetAlpha(NSSettingsStatic.hiddenObjectsAlpha);
										metronomeMark.hidden = true;
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
