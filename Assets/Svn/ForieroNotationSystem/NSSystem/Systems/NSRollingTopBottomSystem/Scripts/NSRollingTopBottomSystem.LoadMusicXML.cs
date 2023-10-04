/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTopBottomSystem : NS
    {
        public override void LoadMusicXML(byte[] bytes)
        {
            NSPlayback.playbackState = NSPlayback.PlaybackState.Stop;
            ScorePartwise.LoadMusicXML(this, bytes);
            Init();
            
            NSRollingTopBottomSystemBouncingBall.Init(0, NSRollingTopBottomSystemBouncingBall.Bouncing.Right);
            NSRollingTopBottomSystemBouncingBall.Init(1, NSRollingTopBottomSystemBouncingBall.Bouncing.Left);
            if(!NSPlayback.Interaction.midiChannelInteractive[0]) SetStaveAlpha(0,0.5f);
            if(!NSPlayback.Interaction.midiChannelInteractive[1]) SetStaveAlpha(1,0.5f);
            
            NSPlayback.OnSongInitialized?.Invoke();
        }
    }
}
