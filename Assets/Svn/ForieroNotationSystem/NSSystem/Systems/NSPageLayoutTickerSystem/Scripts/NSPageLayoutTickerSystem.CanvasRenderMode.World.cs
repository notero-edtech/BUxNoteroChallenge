/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem.Classes;
using ForieroEngine.Music.NotationSystem.Extensions;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Systems
{
	public partial class NSPageLayoutTickerSystem : NS
	{
		public void FixWorldMode()
		{
			var fix = (this.nsBehaviour.fixedCanvasRT.rect.size.x / 2f - this.nsBehaviour.fixedCanvasRT.rect.size.x / 2f * this.nsBehaviour.GetWorldOrthographicSizeRatio());

			var directionFix = NSPlayback.NSRollingPlayback.rollingMode == NSPlayback.NSRollingPlayback.RollingMode.Left ? -1f : 1f;

			this.nsBehaviour.fixedCameraRT.anchoredPosition = new Vector2(fix * directionFix, this.nsBehaviour.fixedCameraRT.anchoredPosition.y);

			if (ns.IsNull()) return;

			var parts = this.GetObjectsOfType<NSPart>(false);

			foreach (NSPart part in parts)
			{
				if (part.pool != PoolEnum.NS_FIXED) continue;
				
				var staves = part.GetObjectsOfType<NSStave>(false);

				foreach (NSStave stave in staves)
				{
					if (stave.pool != PoolEnum.NS_FIXED) continue;
					
					stave.rectWidth = (ns.fixedCanvas.transform as RectTransform).GetWidth() / NSPlayback.Zoom;
					float halfShift = ((ns.fixedCanvas.transform as RectTransform).GetWidth() - stave.rectWidth) / 2f * directionFix;
					stave.SetPositionX(halfShift, false, false);
					stave.UpdateRectWidthAndHeight();

					if (stave.staveLines != null)
					{
						foreach (NSObjectVector line in stave.staveLines.lines)
						{
							line.SetPositionX(stave.GetPositionX(false) - stave.rectWidth / 2f, true, false);
						}

						if (stave.leftBarLine)
						{
							stave.leftBarLine.SetPositionX(stave.GetPositionX(false) - stave.rectWidth / 2f, true, false);
						}

						if (stave.rightBarLine)
						{
							stave.rightBarLine.SetPositionX(stave.GetPositionX(false) + stave.rectWidth / 2f, true, false);
						}

						if (stave.rightSystemBracket)
						{
							stave.rightSystemBracket.SetPositionX(stave.GetPositionX(false) + stave.rectWidth / 2f, true, false);
						}

						if (stave.leftSystemBracket)
						{
							stave.leftSystemBracket.SetPositionX(stave.GetPositionX(false) - stave.rectWidth / 2f, true, false);
						}

						if (stave.backgroundImage != null)
						{
							stave.backgroundImage.SetPositionX(stave.GetPositionX(false), false, false);
						}
					}
				}
			}
		}
	}
}
