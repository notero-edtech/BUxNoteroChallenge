/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public class NSDefaultSystem : NS
    {
        public readonly NSDefaultSystemSpecificSettings specificSettings;
        
        public NSDefaultSystem(NSBehaviour nsBehaviour, NSDefaultSystemSettings settings)
            : base(nsBehaviour, settings.nsSystemSettings, settings.nsSystemSpecificSettings)
        {
            this.specificSettings = settings.nsSystemSpecificSettings;
        }

        public override NSObjectCheckEnum CheckAddObjectConstraints<T>(NSObject parent)
        {
            return NSObjectCheckEnum.Undefined;
        }

        public override NSObjectCheckEnum CheckSetObjectConstraints<T>(NSObject parent)
        {
            return NSObjectCheckEnum.Undefined;
        }

        public override void LoadMidi(byte[] bytes)
        {
            //throw new NotImplementedException();
        }

        public override void LoadMusicXML(byte[] bytes)
        {
            //throw new NotImplementedException();
        }

        public override void LoadMusic(float[] samples, int channels, float totalTime)
        {
            //throw new NotImplementedException();
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Update()
        {
            //throw new NotImplementedException();
        }

        public override void ZoomChanged(float zoom)
        {
            //throw new NotImplementedException();
        }
    }
}
