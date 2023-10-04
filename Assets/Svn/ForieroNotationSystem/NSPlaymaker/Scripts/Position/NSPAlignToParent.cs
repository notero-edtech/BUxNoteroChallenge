/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Position")]
    [Tooltip("")]
    public class NSPAlignToParent : NSPAbstractObject
    {
        [Tooltip("")]
        public FsmBool relative;

        [Tooltip("")]
        public FsmBool recursive;

        public override void Reset()
        {
            base.Reset();

            recursive = new FsmBool { UseVariable = true };
            relative = new FsmBool { UseVariable = true };
        }

        public override void OnEnter()
        {
            if ((o.Value as NSPObjectWrapper))
            {
                if ((o.Value as NSPObjectWrapper).o != null)
                {
                    (o.Value as NSPObjectWrapper).o.AlignToParent(
                        recursive.IsNone ? true : recursive.Value,
                        relative.IsNone ? true : relative.Value
                    );
                }
                else
                {

                }
            }
            else
            {

            }

            Finish();
        }
    }
}
