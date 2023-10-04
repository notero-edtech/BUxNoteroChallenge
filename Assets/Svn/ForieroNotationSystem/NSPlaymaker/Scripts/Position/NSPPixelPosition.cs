/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Position")]
    [Tooltip("")]
    public class NSPPixelPosition : NSPAbstractObject
    {
        [Tooltip("")]
        public FsmVector2 pixelPosition;

        [Tooltip("")]
        public FsmBool recursive;

        [Tooltip("")]
        public FsmBool relative;

        public override void Reset()
        {
            base.Reset();

            pixelPosition = new FsmVector2 { UseVariable = true };
            recursive = new FsmBool { UseVariable = true };
        }

        public override void OnEnter()
        {
            if ((o.Value as NSPObjectWrapper))
            {
                if ((o.Value as NSPObjectWrapper).o != null)
                {
                    (o.Value as NSPObjectWrapper).o.SetPosition(
                        pixelPosition.IsNone ? Vector2.zero : pixelPosition.Value,
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
