/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;
using UnityEngine.Assertions;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified;

namespace ForieroEngine.Music.NotationSystem.Systems
{
	public partial class NSRollingTopBottomSystem : NS
	{
		private int startOctaveIndex = 2;
		private int numberOfOctaves = 7;
		private bool includeLowerAB = true;
		private bool includeUpperC = true;

		private int startIndex { get { return startOctaveIndex * 12 - (includeLowerAB ? 3 : 0); } }
		private int endIndex { get { return startOctaveIndex * 12 + numberOfOctaves * 12 + (includeUpperC ? 1 : 0); } }
		private float totalWhiteKeys { get { return (float)((includeLowerAB ? 2 : 0) + numberOfOctaves * 7 + (includeUpperC ? 1 : 0)); } }
		private float totalWidth { get { return ns.nsBehaviour.fixedPoolRT.GetWidth() * ns.nsBehaviour.fixedCanvasScaler.scaleFactor; } }
		private float whiteKeyWidth { get { return totalWidth / totalWhiteKeys; } }
		private float blackKeyWidth { get { return whiteKeyWidth * 2f / 3f; } }

		private List<NSObjectVector> verticalLines = new List<NSObjectVector>();

		private void CreateVerticalLine(float x, bool thick)
		{
			var line = this.AddObject<NSObjectVector>(PoolEnum.NS_FIXED, PivotEnum.BottomCenter);
			line.vector.vectorEnum = VectorEnum.LineVertical;
			line.rectTransform.pivot = new Vector2(0.5f, 0);
			line.rectTransform.anchorMin = new Vector2(0.5f, 0f);
			line.rectTransform.anchorMax = new Vector2(0.5f, 1f);

			if (thick)
			{
				line.vector.lineVertical.options.thickness = ns.LineWidth * (float)specificSettings.lineThickness;
				line.vector.color = Color.black.Alfa(0.5f);
			}
			else
			{
				line.vector.lineVertical.options.thickness = ns.LineWidth * 3f / 4f * (float)specificSettings.lineThickness;
				line.vector.color = Color.black.Alfa(0.35f);
			}

			line.vector.lineVertical.options.sidesBlur = ns.LineWidth / 2f;
			line.vector.lineVertical.options.endsBlur = ns.LineWidth / 2f;
			line.vector.lineVertical.options.followRectTransformHeight = true;

			//line.rectTransform.SetBottom(options.sideOffse);
			//line.rectTransform.SetTop(-options.sideOffset);

			line.Commit();

			line.SetPositionX(x, false, false);

			verticalLines.Add(line);
		}

		private void CreateMiddleCLine()
		{
			CreateVerticalLine(GetKeyPosition(60), false);
		}

		private readonly int[] _patternPositive = new[] { 0, 4, 7, 11, 14, 17, 21, 24 };
		private readonly int[] _patternNegative = new[] { 0, -3, -7, -10, -13, -17, -20, -24 };
		private int GetStavePositiveLine(int v)
		{
			var i = Mathf.Abs(v) / _patternPositive.Length;
			if (v >= _patternPositive.Length) v = Mathf.Abs(v) % _patternPositive.Length + 1;
			return i * 24 + _patternPositive[v];
		}
		
		private int GetStaveNegativeLine(int v)
		{
			var i = Mathf.Abs(v) / _patternPositive.Length;
			if (v >= _patternNegative.Length) v = Mathf.Abs(v) % _patternNegative.Length + 1;
			return i * -24 + _patternNegative[v];
		}
		
		private void CreateTrebleLines()
		{
			for (var i = 0; i < 5; i++)
			{
				CreateVerticalLine(GetKeyPosition(60 + GetStavePositiveLine(i+1)), true);	
			}

			if (!specificSettings.ledgerLines) return;
			for (var i = 0; i < specificSettings.ledgerLinesCount; i++)
			{
				CreateVerticalLine(GetKeyPosition(60 + GetStavePositiveLine(i+6)), false);	
			}
		}

		private void CreateBassLines()
		{
			for (var i = 0; i < 5; i++)
			{
				CreateVerticalLine(GetKeyPosition(60 + GetStaveNegativeLine(i+1)), true);	
			}
			if (!specificSettings.ledgerLines) return;
			for (var i = 0; i < specificSettings.ledgerLinesCount; i++)
			{
				CreateVerticalLine(GetKeyPosition(60 + GetStaveNegativeLine(i+6)), false);	
			}
		}
		
		private void CreateVerticalLines()
		{
			for (var i = startOctaveIndex; i < (startOctaveIndex + numberOfOctaves); i++)
			{
				if(specificSettings.cLine) CreateVerticalLine(GetKeyPosition(i * 12) - whiteKeyWidth / 2f, specificSettings.cLineThick);
				if(specificSettings.eLine) CreateVerticalLine(GetKeyPosition(i * 12) - whiteKeyWidth / 2f + 3f * whiteKeyWidth, specificSettings.eLineThick);
			}
			if(specificSettings.cLine) CreateVerticalLine(GetKeyPosition((startOctaveIndex + numberOfOctaves) * 12) - whiteKeyWidth / 2f, specificSettings.cLineThick);
		}

		private void DestroyVerticalLines()
		{
			foreach (var line in verticalLines)
			{
				line.IsNotNull()?.Destroy(true);
			}

			verticalLines.Clear();
		}

		public float GetKeyPosition(int midiIndex)
		{
			var whiteKeyCount = 0;

			if (NS.debug) Assert.IsTrue(midiIndex >= startIndex && midiIndex < endIndex);

			var startPosition = -totalWidth / 2f + whiteKeyWidth / 2f;

			for (var i = startIndex; i < endIndex; i++)
			{
				if (i == midiIndex)
				{
					if (i.IsWhiteKey())
					{
						return startPosition + whiteKeyCount * whiteKeyWidth;
					}
					else
					{
						return startPosition + whiteKeyCount * whiteKeyWidth - whiteKeyWidth / 2f;
					}
				}

				if (i.IsWhiteKey())
				{
					whiteKeyCount++;
				}
			}

			return 0;
		}

		public float GetKeyWidth(int midiIndex)
		{
			if (NS.debug) Assert.IsTrue(midiIndex >= startIndex && midiIndex < endIndex);
			if (midiIndex.IsWhiteKey()) { return whiteKeyWidth; }
			else { return blackKeyWidth; }
		}
	}
}
