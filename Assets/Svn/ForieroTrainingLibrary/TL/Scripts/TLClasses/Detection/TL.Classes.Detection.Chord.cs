/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.Training;
using UnityEngine;

namespace ForieroEngine.Music.Detection
{
    public class ChordDetection
    {
        private static HashSet<int> _chordState = new HashSet<int>();
        private static Dictionary<int, double> _keyRemovals = new Dictionary<int, double>();

        public static void InputPress(int pitch)
        {
            _chordState.Add(pitch);

            UnityEngine.Debug.Log("adding pitch " + pitch + " (" + _chordState.Count + ")");
        }

        public static void InputRelease(int pitch)
        {
            UnityEngine.Debug.Log("off pitch " + pitch);

            var currentTime = TL.Providers.Metronome.totalTime;
            _keyRemovals[pitch] = currentTime;
            //_chordState.Remove(note);
        }

        public static HashSet<int> InputUpdate(double bpm)
        {
            List<int> pitchesToRemove = null;

            var currentTime = TL.Providers.Metronome.totalTime;

            var delay = TL.Utilities.Rhythms.BeatsToSeconds(bpm) * 2;

            foreach (var entry in _keyRemovals)
            {
                var time = entry.Value;
                var delta = currentTime - time;
                if (delta > delay)
                {
                    if (pitchesToRemove == null)
                    {
                        pitchesToRemove = new List<int>();
                    }

                    pitchesToRemove.Add(entry.Key);
                }
            }

            if (pitchesToRemove != null)
            {
                foreach (var pitch in pitchesToRemove)
                {
                    _chordState.Remove(pitch);
                    UnityEngine.Debug.Log("removing pitch " + pitch);

                    _keyRemovals.Remove(pitch);
                }

            }

            return _chordState;
        }
    }
}
