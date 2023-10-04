/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Add")]
    [Tooltip("")]
    public class NSPAddKeySignature : NSPAbstractObjectAdd
    {
        public KeySignatureEnum keySignatureEnum;

        public override void Reset()
        {
            base.Reset();

            keySignatureEnum = KeySignatureEnum.Undefined;
        }

        public override void OnEnter()
        {
            if (NS.instance == null)
            {
                Finish();
                Debug.LogError("NS.instance missing!");
                return;
            }

            NSKeySignature keySignature = this.AddObject<NSKeySignature>();

            keySignature.options.keySignatureEnum = keySignatureEnum;

            this.CreateNSPObjectWrapper(keySignature);
            this.Commit(keySignature);

            Finish();
        }
    }
}
