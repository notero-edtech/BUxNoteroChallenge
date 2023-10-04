using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Sound Manager")]
    [Tooltip("Play FX")]
    public class SMPlayFXAction : FsmStateAction
    {
        [RequiredField]
        public FsmString fxId;
        public FsmString groupId;
        public FsmFloat volume;
        public FsmFloat delay;
        public FsmBool loop;

        public override void Reset()
        {
            fxId = new FsmString { UseVariable = true };
            groupId = new FsmString { UseVariable = true };
            volume = new FsmFloat { UseVariable = true };
            delay = new FsmFloat { UseVariable = true };
            loop = new FsmBool { UseVariable = true };
        }

        public override void OnEnter()
        {
            if (!fxId.IsNone)
            {
                if (!loop.IsNone && loop.Value) SM.PlayLoop(fxId.Value, volume.IsNone ? -1 : volume.Value, groupId.IsNone ? "" : groupId.Value, delay.IsNone ? 0 : delay.Value);
                else SM.PlayFX(fxId.Value, volume.IsNone ? -1 : volume.Value, groupId.IsNone ? "" : groupId.Value, delay.IsNone ? 0 : delay.Value);
            }
            Finish();
        }
    }
}