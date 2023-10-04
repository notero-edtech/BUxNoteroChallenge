/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.SMuFL.Extensions;
using ForieroEngine.Music.SMuFL.Ranges;
using JetBrains.Annotations;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSFingering : NSObjectSMuFL
    {
        public struct Options : INSObjectOptions<Options>
        {
            public int Number;
            public float PositionX;
            public float PositionY;
            public PlacementEnum Placement;
            public void Reset()
            {
                this.Number = 0;
                this.PositionX = 0;
                this.PositionY = 0;
                this.Placement = PlacementEnum.Undefined;
            }

            public void CopyValuesFrom(Options t)
            {
                
            }
        }

        [CanBeNull] public Options? options = new Options();
        [CanBeNull] public static readonly Options? _options = new Options();

        public override void Commit()
        {
            text.SetFontSize(NSSettingsStatic.smuflFontSize / 3);
            var yFont = NSSettingsStatic.smuflFontSize / 3f / 2f;
            if (options == null) return;
            text.SetText(options?.Number.ToString());
            
            var x = (float)options?.PositionX;
            SetPositionX(x, true, true);
            
            var yStave = this.stave.GetPositionY(false);
            var yFinger = (float)(this.ns.Pixels * options?.PositionY);
            var yStaveHeight = this.ns.LineSize * ((int)this.stave.options.staveEnum - 1);
            var y = yStave + yFinger + yStaveHeight / 2f + yFont;
            SetPositionY(y, true, false);
            
            //Debug.Log($"{options?.PositionX} {options?.PositionY} {options?.Placement}");
        }

        public override void Reset()
        {
            base.Reset();
            options?.Reset();
        }
    }
}
