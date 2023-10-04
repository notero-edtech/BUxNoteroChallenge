/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.Music.NotationSystem.Classes;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSPageLayoutTickerSystem : NS
    {
        public readonly NSPageLayoutTickerSystemSpecificSettings specificSettings;
        public static int shortenDurationBarsForPixels = 2;
        
        public NSPageLayoutTickerSystem(NSBehaviour nsBehaviour, NSPageLayoutTickerSystemSettings settings)
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
       
        public override void Init()
        {
           base.Init();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        public override void ZoomChanged(float zoom)
        {
            throw new NotImplementedException();
        }
    }
}
