/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingLeftRightSystem : NS
    {
        public override void LoadMusicXML(byte[] bytes)
        {
            NSPlayback.playbackState = NSPlayback.PlaybackState.Stop;
            ScorePartwise.LoadMusicXML(this, bytes);
            Init();
            NSPlayback.OnSongInitialized?.Invoke();
            NSRollingLeftRightSystemBouncingBall.Init(0, NSRollingLeftRightSystemBouncingBall.Bouncing.Top);
            NSRollingLeftRightSystemBouncingBall.Init(1, NSRollingLeftRightSystemBouncingBall.Bouncing.Bottom);
            if(!NSPlayback.Interaction.midiChannelInteractive[0]) SetStaveAlpha(0,0.5f);
            if(!NSPlayback.Interaction.midiChannelInteractive[1]) SetStaveAlpha(1,0.5f);
        }
    }
}
