/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Add")]
    [Tooltip("")]
    public class NSPAddPage : NSPAbstractObjectAdd
    {
        [ObjectType(typeof(Sprite))] 
        public FsmObject sprite;
        public FsmColor color;
        public FsmInt width;
        public FsmInt height;
        public FsmBool sliced;
        
        public override void Reset()
        {
            base.Reset();
            sprite = null;
            color = null;
            width = 0;
            height = 0;
            sliced = false;
        }

        public override void OnEnter()
        {
            if (NS.instance == null) { Finish(); Debug.LogError("NS.instance missing!"); return; }

            var page = this.AddObject<NSPage>();

            page.options.sprite = sprite.IsNone ? null : sprite.Value as Sprite;
            page.options.color = color.IsNone ? Color.white : color.Value;
            page.options.sliced = !sliced.IsNone && sliced.Value;
            this.CreateNSPObjectWrapper(page);
            this.Commit(page);
            
            Finish();
        }
    }
}
