/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingLeftRightSystem : NS
    {
        public readonly NSRollingLeftRightSystemSpecificSettings specificSettings;
        public static int shortenDurationBarsForPixels = 2;

        public NSRollingLeftRightSystem(NSBehaviour nsBehaviour, NSRollingLeftRightSystemSettings settings)
            : base(nsBehaviour, settings.nsSystemSettings, settings.nsSystemSpecificSettings)
        {
            this.specificSettings = settings.nsSystemSpecificSettings;
        }

        public override NSObjectCheckEnum CheckAddObjectConstraints<T>(NSObject parent) { return NSObjectCheckEnum.Undefined; }
        public override NSObjectCheckEnum CheckSetObjectConstraints<T>(NSObject parent) { return NSObjectCheckEnum.Undefined; }

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
            DestroyChildren();
            base.Reset();
            options.Reset();
        }

        public override void Commit() { base.Commit(); }

        public override void Init()
        {
            base.Init();
            NSSettingsStatic.notationOffset = specificSettings.notationOffset;
            ZoomChanged(NSPlayback.Zoom);
            nsBehaviour.rollingPlaybackUpdater.InitPassables();
        }

        public override void Update() { nsBehaviour.rollingPlaybackUpdater.UpdatePassables(); }
        public override void ZoomChanged(float zoom)
        {
            switch (NSSettingsStatic.canvasRenderMode)
            {
                case CanvasRenderMode.Screen: FixScreenMode(); break;
                case CanvasRenderMode.World: FixWorldMode(); break;
            }
        }
    }
}
