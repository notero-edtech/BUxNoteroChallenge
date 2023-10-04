// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using DG.Tweening;
using UnityEngine;
using ForieroEngine.Extensions;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Foriero")]
    [Tooltip("Destroy Object")]
    public class DestroyObjectAction : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject to position.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("")]
        [UIHint(UIHint.FsmGameObject)]
        public FsmGameObject storedObject;

        public FsmString scaleObjectName;

        public bool viewPort;

        public FsmFloat time;
        public FsmFloat delay;
        public Ease easing;

        public FsmVector3 endPosition;
        public FsmVector3 endRotation;
        public FsmVector3 endScale;

        GameObject go;

        //static List<GameObject> cache = new List<GameObject>();

        public override void Reset()
        {
            go = null;
            scaleObjectName = new FsmString { UseVariable = true };

            time = 1f;
            delay = 0f;
            easing = Ease.InOutSine;

            viewPort = true;
            endPosition = new FsmVector3 { UseVariable = true };
            endRotation = new FsmVector3 { UseVariable = true };
            endScale = new FsmVector3 { UseVariable = true };
        }

        bool finished1 = false;
        bool finished2 = false;

        public override void OnEnter()
        {
            finished1 = false;
            finished2 = false;
            if (storedObject.IsNone)
            {
                go = Fsm.GetOwnerDefaultTarget(gameObject);
            }
            else
            {
                go = storedObject.Value;
            }

            if (go == null)
            {
                Finish();
                return;
            }

            GameObject instance = go;
            GameObject scaleInstance = instance;

            if (!scaleObjectName.IsNone)
            {
                Transform t = instance.transform.Search(scaleObjectName.Value);
                if (t != null) scaleInstance = t.gameObject;
            }

            if (!time.IsNone)
            {
                Vector3 toPosition = instance.transform.position;
                if (!endPosition.IsNone)
                {
                    if (viewPort)
                    {
                        toPosition = Camera.main.ViewportToWorldPoint(endPosition.Value);
                    }
                    else
                    {
                        toPosition = endPosition.Value;
                    }
                }

                //				HOTween.To(instance.transform, time.Value, new TweenParms()
                //					.Delay(delay.IsNone ? 0 : delay.Value)
                //					.Ease(easing)
                //					.Prop("position", toPosition)
                //					.Prop("eulerAngles", endRotation.IsNone ? instance.transform.localEulerAngles : endRotation.Value)
                //					.OnComplete(()=>{ finished1 = true; })
                //				);	
                //				
                //				HOTween.To(scaleInstance.transform, time.Value, new TweenParms()
                //					.Delay(delay.IsNone ? 0 : delay.Value)
                //					.Ease(easing)
                //					.Prop("localScale", endScale.IsNone ? instance.transform.localScale : endScale.Value)
                //					.OnComplete(()=>{ finished2 = true; })
                //				);	
            }

        }

        public override void OnUpdate()
        {
            if (finished1 && finished2)
            {
                Finish();
            }
        }
    }
}