/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem.Classes;
using ForieroEngine.Music.NotationSystem.Extensions;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTopBottomSystem : NS
    {
        public void FixZoomWorldMode()
        {
            var fixX = (this.nsBehaviour.fixedCanvasRT.rect.size.x / 2f - this.nsBehaviour.fixedCanvasRT.rect.size.x / 2f * this.nsBehaviour.GetWorldOrthographicSizeRatio());
            var fixY = (this.nsBehaviour.fixedCanvasRT.rect.size.y / 2f - this.nsBehaviour.fixedCanvasRT.rect.size.y / 2f * this.nsBehaviour.GetWorldOrthographicSizeRatio());

            foreach (NSObjectVector line in this.verticalLines)
            {
                if (line)
                {
                    line.rectTransform.SetRectTop(fixY);
                    line.rectTransform.SetRectBottom(fixY);
                }
            }

            var parts = this.GetObjectsOfType<NSPart>(false);

            foreach (var part in parts)
            {
                var barlinenumbers = part.GetObjectsOfType<NSBarLineNumber>(false);

                foreach (var barlinenumber in barlinenumbers)
                {

                }
            }
        }

        public void UpdateWorldMode()
        {
            var fixX = (this.nsBehaviour.fixedCanvasRT.rect.size.x / 2f - this.nsBehaviour.fixedCanvasRT.rect.size.x / 2f * this.nsBehaviour.GetWorldOrthographicSizeRatio());

            nsBehaviour.fixedCameraRT.anchoredPosition = new Vector2(
                Mathf.Clamp(nsBehaviour.fixedCameraRT.anchoredPosition.x, -fixX, fixX),
                nsBehaviour.fixedCameraRT.anchoredPosition.y
            );

            nsBehaviour.movableCameraRT.anchoredPosition = new Vector2(
                Mathf.Clamp(nsBehaviour.movableCameraRT.anchoredPosition.x, -fixX, fixX),
                nsBehaviour.movableCameraRT.anchoredPosition.y
            );
        }
    }
}
