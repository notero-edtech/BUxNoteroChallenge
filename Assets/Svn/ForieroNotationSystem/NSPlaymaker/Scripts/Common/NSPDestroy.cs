/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Common")]
    [Tooltip("")]
    public class NSPDestroy : NSPAbstractObject
    {
        public bool destroyChildren;

        public override void Reset()
        {
            base.Reset();

            destroyChildren = true;
        }

        public override void OnEnter()
        {
            if (o.IsNone)
            {
                Finish();

            }
            else
            {
                if ((o.Value as NSPObjectWrapper))
                {
                    if ((o.Value as NSPObjectWrapper).o != null)
                    {
                        if (destroyChildren)
                        {
                            (o.Value as NSPObjectWrapper).o.DestroyChildren();
                        }

                        (o.Value as NSPObjectWrapper).o.Destroy();
                    }
                    else
                    {

                    }
                }
                else
                {

                }

                o.Value = null;
                Finish();
            }
        }
    }
}
