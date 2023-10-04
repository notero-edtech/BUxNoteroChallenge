/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using UnityEngine;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;


namespace HutongGames.PlayMaker.Actions
{
    public abstract partial class NSPAbstractObjectAdd : NSPAbstractObject
    {
        [ActionSection("Base")]
        [UIHint(UIHint.FsmEnum)]
        [ObjectType(typeof(PivotEnum))]
        public FsmEnum rectAlign;

        [UIHint(UIHint.FsmEnum)]
        [ObjectType(typeof(PoolEnum))]
        public FsmEnum pool;

        [UIHint(UIHint.FsmEnum)]
        [ObjectType(typeof(PivotEnum))]
        public FsmEnum pivot;

        [Tooltip("")]
        public FsmString id;

        [Tooltip("")]
        public FsmString tag;

        [Tooltip("")]
        public FsmBool commit;

        [UIHint(UIHint.Variable)]
        [ObjectType(typeof(NSPObjectWrapper))]
        [Tooltip("")]
        public FsmObject parent;

        public override void Reset()
        {
            base.Reset();

            rectAlign = new FsmEnum { UseVariable = true };
            pool = new FsmEnum { UseVariable = true };
            pivot = new FsmEnum { UseVariable = true };
            id = new FsmString { UseVariable = true };
            tag = new FsmString { UseVariable = true };

            commit = false;
        }

        public T AddObject<T>(GameObject prefab = null) where T : NSObject, new()
        {
            T t = default(T);
            if (parent.IsNone || parent.Value == null || (parent.Value as NSPObjectWrapper).o == null)
            {
                t = NS.instance.AddObject<T>(
                    pool.IsNone ? PoolEnum.NS_MOVABLE : (PoolEnum)pool.Value,
                    pivot.IsNone ? PivotEnum.MiddleCenter : (PivotEnum)pivot.Value,
                    id.IsNone ? "" : id.Value,
                    tag.IsNone ? "" : tag.Value,
                    prefab
                     );
            }
            else
            {
               t = (parent.Value as NSPObjectWrapper).o.AddObject<T>(
                    pool.IsNone ? PoolEnum.NS_MOVABLE : (PoolEnum)pool.Value, 
                    pivot.IsNone ? PivotEnum.MiddleCenter : (PivotEnum)pivot.Value,
                    id.IsNone ? "" : id.Value,
                    tag.IsNone ? "" : tag.Value,
                    prefab
                     );
            }
            return t;
        }

        public void Commit<T>(T t) where T : NSObject
        {
            if (!commit.IsNone && commit.Value)
            {
                t.Commit();
            }
        }
    }
}
