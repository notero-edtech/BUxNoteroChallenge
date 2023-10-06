using UnityEngine;

namespace Notero.Unity.AudioModule
{
    public interface IAdjustableAudioClip
    {
        /// <summary>
        /// A container for audio data.
        /// </summary>
        public AudioClip AudioClip { get; set; }
        /// <summary>
        /// Volume value between 0 and 1.
        /// </summary>
        public float Volume { get; set; }
        /// <summary>
        /// Is the audio clip looping?
        /// </summary>
        public bool IsLoop { get; set; }
        /// <summary>
        /// Target channel for audio output.
        /// </summary>
        public AudioChannel Channel { get; set; }
        /// <summary>
        /// Returns the current load state of the audio data associated with an AudioClip.
        /// </summary>
        public AudioDataLoadState LoadState { get; }
    }
}
