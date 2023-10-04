using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Sound Manager")]
    [Tooltip("Stop Music")]
    public class SMStopMusicAction : FsmStateAction
    {
        public FsmString groupId;
        public FsmBool all;

        public override void Reset()
        {
            groupId = new FsmString { UseVariable = true };
            all = new FsmBool { UseVariable = true };
        }

        public override void OnEnter()
        {
            if (!groupId.IsNone) SM.StopMusic(groupId.Value);
            if (!all.IsNone && all.Value) SM.StopAllMusic();
            Finish();
        }
    }
}