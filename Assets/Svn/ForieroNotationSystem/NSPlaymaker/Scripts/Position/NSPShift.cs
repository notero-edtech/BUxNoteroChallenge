/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using ForieroEngine.Music.NotationSystem;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Position")]
    [Tooltip("")]
    public class NSPShift : NSPAbstractObject
    {
        [UIHint(UIHint.FsmEnum)]
        [ObjectType(typeof(DirectionEnum))]
        public FsmEnum directionEnum;

        [UIHint(UIHint.FsmEnum)]
        [ObjectType(typeof(ShiftStepEnum))]
        public FsmEnum step;

        [Tooltip("")]
        public FsmFloat multiplicator;

        [Tooltip("")]
        public FsmBool recursive;

        public override void Reset()
        {
            base.Reset();

            multiplicator = new FsmFloat { UseVariable = true };
            directionEnum = new FsmEnum { UseVariable = true };
            step = new FsmEnum { UseVariable = true };
            recursive = new FsmBool { UseVariable = true };
        }

        public override void OnEnter()
        {
            if ((o.Value as NSPObjectWrapper))
            {
                if ((o.Value as NSPObjectWrapper).o != null)
                {
                    (o.Value as NSPObjectWrapper).o.Shift(
                        directionEnum.IsNone ? DirectionEnum.Undefined : (DirectionEnum)directionEnum.Value,
                        recursive.IsNone ? true : recursive.Value,
                        multiplicator.IsNone ? 1f : multiplicator.Value,
                        step.IsNone ? ShiftStepEnum.Whole : (ShiftStepEnum)step.Value
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
