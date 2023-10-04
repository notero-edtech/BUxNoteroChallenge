/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Common")]
    [Tooltip("")]
    public class NSPInitialized : FsmStateAction
    {
        public override void OnUpdate()
        {
            if (NS.instance != null)
            {
                Finish();
            }
        }
    }
}
