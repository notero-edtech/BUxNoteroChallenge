/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Add")]
    [Tooltip("")]
    public class NSPAddRest : NSPAbstractObjectAdd
    {
        public RestEnum restEnum;

        public override void Reset()
        {
            base.Reset();

            restEnum = RestEnum.Undefined;
        }

        public override void OnEnter()
        {
            if (NS.instance == null)
            {
                Finish();
                Debug.LogError("NS.instance missing!");
                return;
            }

            NSRest rest = this.AddObject<NSRest>();
               
            rest.options.restEnum = restEnum;

            this.CreateNSPObjectWrapper(rest);
            this.Commit(rest);

            Finish();
        }
    }
}
