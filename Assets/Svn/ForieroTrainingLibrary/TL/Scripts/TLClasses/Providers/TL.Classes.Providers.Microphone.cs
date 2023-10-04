/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training.Classes.Providers
{
    public class MicrophoneProvider
    {
        public bool recording = false;

        public virtual void Reset()
        {
            throw new NotImplementedException("Method not implmeneted : Reset!");
        }

        public virtual void Start(string deviceName = null)
        {
            throw new NotImplementedException("Method not implmeneted : Start!");
        }

        public virtual void Stop(string deviceName = null)
        {
            throw new NotImplementedException("Method not implmeneted : Stop!");
        }

        public void Initialize(int sampleRate)
        {
            TL.Detection.Initialize(sampleRate);
        }

        public void Update(float[] samples)
        {
            TL.Detection.Update(samples);
        }
    }
}
