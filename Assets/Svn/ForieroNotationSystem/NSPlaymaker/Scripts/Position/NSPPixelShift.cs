/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Position")]
    [Tooltip("")]
    public class NSPPixelShift : NSPAbstractObject
    {
        [Tooltip("")]
        public FsmVector2 pixelShift;

        [Tooltip("")]
        public FsmBool recursive;

        public override void Reset()
        {
            base.Reset();

            pixelShift = new FsmVector2 { UseVariable = true };
            recursive = new FsmBool { UseVariable = true };
        }

        public override void OnEnter()
        {
            if ((o.Value as NSPObjectWrapper))
            {
                if ((o.Value as NSPObjectWrapper).o != null)
                {
                    (o.Value as NSPObjectWrapper).o.PixelShift(
                        pixelShift.IsNone ? Vector2.zero : pixelShift.Value,
                        recursive.IsNone ? true : recursive.Value
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
