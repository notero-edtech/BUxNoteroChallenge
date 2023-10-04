/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
   public interface IAudioProvider
   {
      string Id { get; }
      void EnableAudioProvider();
      void DisableAudioProvider();
      void InitAudioClip(AudioClip clip);
   }
}
