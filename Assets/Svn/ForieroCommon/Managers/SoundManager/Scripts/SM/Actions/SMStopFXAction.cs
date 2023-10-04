using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Sound Manager")]
    [Tooltip("Stop FX Loop")]
    public class SMStopFXAction : FsmStateAction
    {       
        public FsmString fxId;
        public FsmString groupId;
        public FsmFloat delay;

        public override void Reset()
        {
            fxId = new FsmString { UseVariable = true };
            groupId = new FsmString { UseVariable = true };
            delay = new FsmFloat { UseVariable = true };
        }

        public override void OnEnter()
        {
            if (fxId.IsNone)
            {
                SM.StopAllLoops();
            }
            else
            {                
                SM.StopLoop(fxId.Value, groupId.IsNone ? "" : groupId.Value, delay.IsNone ? 0 : delay.Value);
            }
            Finish();
        }
    }
}