/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSPageLayoutTickerSystem : NS
    {
        public override void LoadMusicXML(byte[] bytes)
        {
            NSPlayback.playbackState = NSPlayback.PlaybackState.Stop;
            ScorePartwise.LoadMusicXML(this, bytes);
            Init();
            NSPlayback.OnSongInitialized?.Invoke();
        }
    }
}
