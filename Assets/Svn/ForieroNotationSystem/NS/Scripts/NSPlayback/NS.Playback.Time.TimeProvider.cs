/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.MIDIUnified.Plugins;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {                               
        public static partial class Time
        {
            public static ITimeProvider iTimeProvider => TimeProviders.iTimeProvider ?? DSPTimeProvider;
        }              
    }
}
