/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTickerSystem : NS
    {
        NSWaveTrack waveTrack = null;

        // what is the total time ? 
        public override void LoadMusic(float[] samples, int channels, float totalTime)
        {
            NSPlayback.playbackState = NSPlayback.PlaybackState.Stop;

            /*var clip = Resources.Load<AudioClip>("test");
            samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);
            channels = clip.channels;
            totalTime = clip.length;*/

            waveTrack.IsNotNull()?.Destroy();

            // ns.pixelLenght is calculated when parsing MusicXML
            // OR
            // we need to calculate it nx.pixelLenght = clip.totalTime * ns.pixelsPerSeconds

            if (NS.debug)
            {
                Debug.Log("NS PixelLenght : " + ns.pixelLenght.ToString());
                Debug.Log("NS PixelsPerSecond : " + NSPlayback.NSTickerPlayback.pixelsPerSecond.ToString());
                Debug.Log("AUDIO Time : " + totalTime.ToString());
                Debug.Log("TRACK PixelLenght : " + (NSPlayback.NSTickerPlayback.pixelsPerSecond * totalTime).ToString());
            }

            NSPlayback.Time.TotalTime = totalTime;

            waveTrack = ns.AddObject<NSWaveTrack>(PoolEnum.NS_MOVABLE, PivotEnum.MiddleLeft);
            waveTrack.options.totalTime = totalTime;
            waveTrack.options.height = 200;
            waveTrack.options.samples = samples;
            waveTrack.options.channels = channels;
            waveTrack.options.background = true;
            waveTrack.options.backgroundColor = Color.black;
            waveTrack.Commit();

            waveTrack.PixelShiftY(200, true);
        }
    }
}
