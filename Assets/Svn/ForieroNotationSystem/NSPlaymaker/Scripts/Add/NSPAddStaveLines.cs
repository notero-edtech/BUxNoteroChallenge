/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Add")]
    [Tooltip("")]
    public class NSPAddStaveLines : NSPAbstractObjectAdd
    {
        public StaveEnum staveEnum;
        public bool background;

        public override void Reset()
        {
            base.Reset();
            staveEnum = StaveEnum.Undefined;
            background = true;
        }

        public override void OnEnter()
        {
            if (NS.instance == null)
            {
                Finish();
                Debug.LogError("NS.instance missing!");
                return;
            }

            NSStaveLines staveLines = this.AddObject<NSStaveLines>();
                   
            staveLines.options.staveEnum = staveEnum;

            this.CreateNSPObjectWrapper(staveLines);
            this.Commit(staveLines);

            Finish();
        }
    }
}
