/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTickerSystem : NS
    {
        public override void LoadMidi(byte[] bytes)
        {
            NSPlayback.playbackState = NSPlayback.PlaybackState.Stop;
        }
    }
}
