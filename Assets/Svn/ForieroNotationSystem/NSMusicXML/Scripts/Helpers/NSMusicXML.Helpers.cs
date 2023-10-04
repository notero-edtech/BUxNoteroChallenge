/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;

namespace ForieroEngine.Music.MusicXML
{
    public class MeasureTime
    {
        private float _totalDivisions = 0;
        public float totalDivisions => _totalDivisions;

        public float previousDivisions = 0;
        private float _divisions = 0;
        public float divisions
        {
            get => _divisions;
            set
            {
                previousDivisions = _divisions;
                _divisions = value;
                _totalDivisions = Mathf.Max(_totalDivisions, _divisions);
            }
        }

        private float _totalTime = 0;
        public float totalTime => _totalTime;

        public float previousTime = 0;
        private float _time = 0;
        public float time
        {
            get => _time;
            set
            {
                previousTime = _time;
                _time = value;
                _totalTime = Mathf.Max(_totalTime, _time);
            }
        }
    }
}
