// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using ForieroEngine.Extensions;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Foriero")]
    [Tooltip("Create Object")]
    public class CreateObjectAction : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject to position.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.FsmString)]
        public FsmString objectName;

        public FsmString scaleObjectName;

        //[RequiredField]
        [Tooltip("Use this variable to store created object.")]
        [UIHint(UIHint.FsmGameObject)]
        public FsmGameObject createdObject;

        public bool viewPort;
        public FsmVector3 startPosition;
        public FsmVector3 startRotation;
        public FsmVector3 startScale;

        public FsmFloat time;
        public FsmFloat delay;
        public Ease easing;

        public FsmVector3 endPosition;
        public FsmVector3 endRotation;
        public FsmVector3 endScale;

        //GameObject go;

        static List<GameObject> cache = new List<GameObject>();

        public override void Reset()
        {
            //go = null;
            objectName = new FsmString { UseVariable = true };
            scaleObjectName = new FsmString { UseVariable = true };
            createdObject = new FsmGameObject { UseVariable = true };

            time = 1f;
            delay = 0f;
            easing = Ease.InOutSine;

            viewPort = true;
            startPosition = new FsmVector3 { UseVariable = true };
            startRotation = new FsmVector3 { UseVariable = true };
            startScale = new FsmVector3 { UseVariable = true };
            endPosition = new FsmVector3 { UseVariable = true };
            endRotation = new FsmVector3 { UseVariable = true };
            endScale = new FsmVector3 { UseVariable = true };
        }

        public override void OnEnter()
        {
            //go = Fsm.GetOwnerDefaultTarget(gameObject);

            GameObject prefab = null;
            foreach (GameObject o in cache)
            {
                if (o.name.Equals(objectName.Value))
                {
                    prefab = o;
                    break;
                }
            }

            if (prefab == null)
            {
                string resPath = "Foriero/Prefabs/" + objectName.Value;
                prefab = Resources.Load(resPath, typeof(GameObject)) as GameObject;
                if (prefab == null)
                {
                    Debug.LogError("PREFAB NOT FOUND : " + resPath);
                    Finish();
                    return;
                }
                else
                {
                    cache.Add(prefab);
                }
            }

            GameObject instance = GameObject.Instantiate(prefab) as GameObject;
            GameObject scaleInstance = instance;

            if (!scaleObjectName.IsNone)
            {
                Transform t = instance.transform.Search(scaleObjectName.Value);
                if (t != null) scaleInstance = t.gameObject;
            }

            if (!startPosition.IsNone)
            {
                if (viewPort)
                {
                    instance.transform.position = Camera.main.ViewportToWorldPoint(startPosition.Value);
                }
                else
                {
                    instance.transform.position = startPosition.Value;
                }
            }
            if (!startRotation.IsNone) instance.transform.eulerAngles = startRotation.Value;
            if (!startScale.IsNone) scaleInstance.transform.localScale = startScale.Value;

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
                //				);	
                //				
                //				HOTween.To(scaleInstance.transform, time.Value, new TweenParms()
                //					.Delay(delay.IsNone ? 0 : delay.Value)
                //					.Ease(easing)
                //					.Prop("localScale", endScale.IsNone ? instance.transform.localScale : endScale.Value)
                //				);	
            }

            if (!createdObject.IsNone) createdObject.Value = instance;
            Finish();
        }
    }
}