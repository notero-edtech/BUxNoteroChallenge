/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using System.Diagnostics;
using ForieroEngine.Music.MusicXML.Xsd;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {
        public partial class Analyzes
        {
            public static void Analyze(scorepartwise score, float pixelsPerSmallestNote)
            {
                var watch = Stopwatch.StartNew();

                NSPlayback.ResetMeasures();
                ScorePartwise.Analyze(score);

                watch.Stop();
                if (NS.debug) Debug.Log("ANALYZED in : " + watch.Elapsed.TotalSeconds.ToString());

                Time.TotalTime = ScorePartwise.totalTime;
                if (NS.debug) Debug.Log("ANALYZES TotalTime : " + Time.TotalTime.ToString("F2") + "s");
                NSRollingPlayback.pixelsPerSecond = Mathf.Clamp(1.0f / ScorePartwise.shortestTime * pixelsPerSmallestNote, 200, float.MaxValue);
                if (NS.debug) Debug.Log("ANALYZES PixelsPerSecond : " + NSRollingPlayback.pixelsPerSecond.ToString("F2") + "px");
                measures = ScorePartwise.Measures;
                TPQN = ScorePartwise.TPQN ;

                OnSongAnalyzed?.Invoke();
            }
        }
    }
}
