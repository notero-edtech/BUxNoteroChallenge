/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem.Classes;
using ForieroEngine.Music.NotationSystem.Extensions;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTopBottomSystem : NS
    {
        public void FixZoomScreenMode()
        {
            var fix = (this.nsBehaviour.fixedCanvasRT.rect.size.y / 2f - this.nsBehaviour.fixedCanvasRT.rect.size.y / 2f * this.nsBehaviour.GetWorldOrthographicSizeRatio());

            foreach (var line in this.verticalLines)
            {
                if (!line) continue;
                line.rectTransform.SetRectTop(fix);
                line.rectTransform.SetRectBottom(fix);
            }
        }

        public void UpdateScreenMode()
        {

        }
    }
}
