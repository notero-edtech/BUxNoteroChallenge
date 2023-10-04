/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Add")]
    [Tooltip("")]
    public class NSPAddStave : NSPAbstractObjectAdd
    {
        public StaveEnum staveEnum;
        public FsmBool background;
        public FsmColor backgroundColor;

        public override void Reset()
        {
            base.Reset();
            staveEnum = StaveEnum.Undefined;
            background = true;
            backgroundColor = Color.white;
        }

        public override void OnEnter()
        {
            if (NS.instance == null)
            {
                Finish();
                Debug.LogError("NS.instance missing!");
                return;
            }

            NSStave stave = this.AddObject<NSStave>();

            stave.options.staveEnum = staveEnum;
            stave.options.background = background.IsNone || background.Value;
            stave.options.backgroundColor = backgroundColor.IsNone ? Color.white : backgroundColor.Value;
            
            this.CreateNSPObjectWrapper(stave);
            this.Commit(stave);

            Finish();
        }
    }
}
