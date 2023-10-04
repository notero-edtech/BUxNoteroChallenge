/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem.Classes;
using ForieroEngine.Music.NotationSystem.Extensions;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSPageLayoutTickerSystem : NS
    {
        public void FixScreenMode()
        {
			var directionFix = NSPlayback.NSRollingPlayback.rollingMode == NSPlayback.NSRollingPlayback.RollingMode.Left ? -1f : 1f;

			this.nsBehaviour.fixedCameraRT.anchoredPosition = new Vector2(0, this.nsBehaviour.fixedCameraRT.anchoredPosition.y);

            if (ns == null) return;

            var parts = ns.GetObjectsOfType<NSPart>(false);

            foreach (NSPart part in parts)
            {
                if (part.pool != PoolEnum.NS_FIXED) continue;

                var staves = part.GetObjectsOfType<NSStave>(false);
				
				foreach (NSStave stave in staves)
                {
                    if (stave.pool != PoolEnum.NS_FIXED) continue;

					stave.rectWidth = (ns.fixedCanvas.transform as RectTransform).GetWidth() / NSPlayback.Zoom;					
					stave.SetPositionX(0, false, false);
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

					stave.Arrange(NSPlayback.NSRollingPlayback.rollingMode.ToHorizontalDirectionEnum());
				}
            }
        }
    }
}
