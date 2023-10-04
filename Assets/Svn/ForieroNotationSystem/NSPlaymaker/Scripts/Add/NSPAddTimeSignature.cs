/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Add")]
    [Tooltip("")]
    public class NSPAddTimeSignature : NSPAbstractObjectAdd
    {
        public int numerator;
        public int denominator;

        public override void Reset()
        {
            base.Reset();

            numerator = 4;
            denominator = 4;
        }

        public override void OnEnter()
        {
            if (NS.instance == null)
            {
                Finish();
                Debug.LogError("NS.instance missing!");
                return;
            }

            NSTimeSignature timeSignature = this.AddObject<NSTimeSignature>();

            timeSignature.options.timeSignatureStruct = new TimeSignatureStruct(numerator, denominator);

            this.CreateNSPObjectWrapper(timeSignature);
            this.Commit(timeSignature);

            Finish();
        }
    }
}
