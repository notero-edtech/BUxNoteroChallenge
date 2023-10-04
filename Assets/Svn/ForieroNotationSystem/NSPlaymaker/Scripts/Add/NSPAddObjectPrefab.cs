/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Add")]
    [Tooltip("")]
    public class NSPAddObjectPrefab : NSPAbstractObjectAdd
    {
        [UIHint(UIHint.FsmGameObject)]
        [Tooltip("")]
        [RequiredField]
        public FsmGameObject prefab;

        [UIHint(UIHint.FsmFloat)]
        public FsmFloat relativeFontSize;

        [UIHint(UIHint.Variable)]
        [ObjectType(typeof(FsmGameObject))]
        [Tooltip("")]
        public FsmGameObject instance;

        public override void Reset()
        {
            base.Reset();
            prefab = new FsmGameObject { UseVariable = true };
            relativeFontSize = new FsmFloat { UseVariable = true };
        }

        public override void OnEnter()
        {
            if (NS.instance == null)
            {
                Finish();
                Debug.LogError("NS.instance missing!");
                return;
            }

            var nsop = this.AddObject<NSObjectPrefab>(prefab.Value);

            nsop.options.relativeFontSize = relativeFontSize.IsNone ? 1f : relativeFontSize.Value;

            this.CreateNSPObjectWrapper(nsop);
            this.Commit(nsop);

            if (!instance.IsNone) instance.Value = nsop.rectTransform?.gameObject;

            Finish();
        }
    }
}
