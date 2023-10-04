/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Add")]
    [Tooltip("")]
    public class NSPAddClef : NSPAbstractObjectAdd
    {
        public ClefEnum clefEnum;

        public override void Reset()
        {
            base.Reset();

            clefEnum = ClefEnum.Undefined;
        }

        public override void OnEnter()
        {
            if (NS.instance == null)
            {
                Finish();
                Debug.LogError("NS.instance missing!");
                return;
            }

            NSClef clef = this.AddObject<NSClef>();

            clef.options.clefEnum = clefEnum;

            this.CreateNSPObjectWrapper(clef);
            this.Commit(clef);

            Finish();
        }
    }
}
