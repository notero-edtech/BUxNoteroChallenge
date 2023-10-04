/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Add")]
    [Tooltip("")]
    public class NSPAddAccidental : NSPAbstractObjectAdd
    {
        public AccidentalEnum accidentalEnum;

        public override void Reset()
        {
            base.Reset();

            accidentalEnum = AccidentalEnum.Undefined;
        }

        public override void OnEnter()
        {
            if (NS.instance == null)
            {
                Finish();
                Debug.LogError("NS.instance missing!");
                return;
            }

            NSAccidental accidental = this.AddObject<NSAccidental>();

            accidental.options.accidentalEnum = accidentalEnum;

            this.CreateNSPObjectWrapper(accidental);
            this.Commit(accidental);

            Finish();
        }
    }
}
