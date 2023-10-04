/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Position")]
    [Tooltip("")]
    public class NSPRectAlign : NSPAbstractObject
    {
        [UIHint(UIHint.FsmEnum)]
        [ObjectType(typeof(PivotEnum))]
        public FsmEnum rectAlign;

        public override void Reset()
        {
            base.Reset();

            rectAlign = new FsmEnum { UseVariable = true };
        }

        public override void OnEnter()
        {
            if ((o.Value as NSPObjectWrapper))
            {
                if ((o.Value as NSPObjectWrapper).o != null)
                {
                    (o.Value as NSPObjectWrapper).o.pivot = rectAlign.IsNone ? PivotEnum.MiddleCenter : (PivotEnum)rectAlign.Value;
                    (o.Value as NSPObjectWrapper).o.rectTransform.SetPivotAndAnchors((o.Value as NSPObjectWrapper).o.pivot.ToPivotAndAnchors());
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
