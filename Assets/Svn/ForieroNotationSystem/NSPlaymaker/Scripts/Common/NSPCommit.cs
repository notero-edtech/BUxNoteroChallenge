/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Common")]
    [Tooltip("")]
    public class NSPCommit : NSPAbstractObject
    {
        public override void OnEnter()
        {
            if ((o.Value as NSPObjectWrapper))
            {
                if ((o.Value as NSPObjectWrapper).o != null)
                {
                    (o.Value as NSPObjectWrapper).o.Commit();
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
