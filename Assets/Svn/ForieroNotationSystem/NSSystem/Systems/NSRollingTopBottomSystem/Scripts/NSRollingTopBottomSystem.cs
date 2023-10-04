/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using ForieroEngine.MIDIUnified;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;
using UnityEngine.Assertions;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTopBottomSystem : NS
    {
        public readonly NSRollingTopBottomSystemSpecificSettings specificSettings;
        public static int shortenDurationBarsForPixels = 2;

        public NSRollingTopBottomSystem(NSBehaviour nsBehaviour, NSRollingTopBottomSystemSettings settings)
            : base(nsBehaviour, settings.nsSystemSettings, settings.nsSystemSpecificSettings)
        {
            this.specificSettings = settings.nsSystemSpecificSettings;
        }

        public override NSObjectCheckEnum CheckAddObjectConstraints<T>(NSObject parent)
        {
            return NSObjectCheckEnum.Undefined;
        }

        public override NSObjectCheckEnum CheckSetObjectConstraints<T>(NSObject parent)
        {
            return NSObjectCheckEnum.Undefined;
        }

        public class Options : INSObjectOptions<Options>
        {
            public bool shifted = true;
            public bool notes = true;
            public bool durationBars = true;

            public void Reset()
            {
                shifted = true;
                notes = true;
                durationBars = true;
            }

            public void CopyValuesFrom(Options o)
            {
                shifted = o.shifted;
                notes = o.notes;
                durationBars = o.durationBars;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public override void Reset()
        {
            Debug.Log("CALLING RESET");
            DestroyChildren();

            base.Reset();
            options.Reset();
        }

        public override void Commit()
        {

        }

        public override void Destroy(bool silent = false)
        {

        }

        public override void Update()
        {
            var fixX = (this.nsBehaviour.fixedCanvasRT.rect.size.x / 2f - this.nsBehaviour.fixedCanvasRT.rect.size.x / 2f * this.nsBehaviour.GetWorldOrthographicSizeRatio());

            switch (NSSettingsStatic.canvasRenderMode)
            {
                case CanvasRenderMode.Screen: UpdateScreenMode(); break;
                case CanvasRenderMode.World: UpdateWorldMode(); break;
            }

            nsBehaviour.rollingPlaybackUpdater.UpdatePassables();
        }

        public override void ZoomChanged(float zoom)
        {
            switch (NSSettingsStatic.canvasRenderMode)
            {
                case CanvasRenderMode.Screen: FixZoomScreenMode(); break;
                case CanvasRenderMode.World: FixZoomWorldMode(); break;
            }
        }

        public override void Init()
        {
            DestroyVerticalLines();
            if (specificSettings.trebleLines || specificSettings.bassLines)
            {
                if(specificSettings.trebleLines && specificSettings.bassLines) CreateMiddleCLine();
                if(specificSettings.trebleLines) CreateTrebleLines();
                if(specificSettings.bassLines) CreateBassLines();
            }
            else
            {
                CreateVerticalLines();
            }
            
            base.Init();
            
            ZoomChanged(NSPlayback.Zoom);
            nsBehaviour.rollingPlaybackUpdater.InitPassables();
        }
    }
}
