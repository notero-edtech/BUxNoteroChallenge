using System;
using ForieroEngine;
using UnityEngine;
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Foriero")]
    public class ReleaseVersionAction : FsmStateAction
    {
        public FsmEvent demo;
        public FsmEvent early;
        public FsmEvent full;

        public override void Reset()
        {
            demo = null;
            early = null;
            full = null;
        }

        public override void OnEnter()
        {
            switch (Publishing.ReleaseVersion)
            {
                case Publishing.ReleaseVersions.NONE:
                    break;
                case Publishing.ReleaseVersions.DEMO:
                    if (demo != null) Fsm.Event(demo);
                    if(Foriero.debug) Debug.Log("VERSION DEMO");
                    break;
                case Publishing.ReleaseVersions.EARLY:
                    if (early != null) Fsm.Event(early);
                    if(Foriero.debug) Debug.Log("VERSION EARLY");
                    break;
                case Publishing.ReleaseVersions.FULL:
                    if (full != null) Fsm.Event(full);
                    if(Foriero.debug) Debug.Log("VERSION FULL");
                    break;
            }
            Finish();
        }
    }
}
