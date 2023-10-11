namespace Notero.Unity.AudioModule.Settings
{
    public abstract class AudioSettings
    {
        public abstract float MasterVolume { get; set; }
        public abstract float BGMVolume { get; set; }
        public abstract float SFXVolume { get; set; }
        public abstract float MidiPlaybackVolume { get; set; }

        public abstract void Save();

        public abstract void Reset();
    }
}
