/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem.Classes;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTickerSystem : NS
    {
        public readonly NSRollingTickerSystemSpecificSettings specificSettings;
        public static int shortenDurationBarsForPixels = 2;
        
        public NSRollingTickerSystem(NSBehaviour nsBehaviour, NSRollingTickerSystemSettings settings)
            : base(nsBehaviour, settings.nsSystemSettings, settings.nsSystemSpecificSettings)
        {
            this.specificSettings = settings.nsSystemSpecificSettings;
        }

        public override NSObjectCheckEnum CheckAddObjectConstraints<T>(NSObject parent) { return NSObjectCheckEnum.Undefined; }
        public override NSObjectCheckEnum CheckSetObjectConstraints<T>(NSObject parent) { return NSObjectCheckEnum.Undefined; }
       
        public override void Init()
        {
            base.Init();
        }

        public override void Update()
        {
            
        }

        public override void ZoomChanged(float zoom)
        {
            
        }
    }
}
