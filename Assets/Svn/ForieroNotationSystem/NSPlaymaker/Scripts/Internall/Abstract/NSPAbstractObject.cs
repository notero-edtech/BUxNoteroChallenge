/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using UnityEngine;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;


namespace HutongGames.PlayMaker.Actions
{
    public abstract partial class NSPAbstractObject : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [ObjectType(typeof(NSPObjectWrapper))]
        [Tooltip("")]
        public FsmObject o;

        public override void Reset()
        {
            o = null;
        }

        public void CreateNSPObjectWrapper<T>(T t) where T : NSObject
        {
            if (!o.IsNone)
            {
                o.Value = NSPObjectWrapper.Instantiate(t, o.Value);
            }
        }
    }
}
