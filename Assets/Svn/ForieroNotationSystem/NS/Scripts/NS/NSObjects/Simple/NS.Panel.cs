/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSPanel : NSObjectImage
    {
        public class Options : INSObjectOptions<Options>
        {
            public Sprite sprite = null;
            public bool sliced = false;
            public Color color = Color.white;
           
            public void Reset()
            {
                sprite = null;
                sliced = false;
                color = Color.white;
            }

            public void CopyValuesFrom(Options o)
            {
                this.sprite = o.sprite;
                this.sliced = o.sliced;
                this.color = o.color;
            }
        }

        public readonly Options options = new ();
        public static readonly Options _options = new ();

        public override void Commit()
        {
            base.Commit();
            this.image.sprite = options.sprite;
            this.image.type = options.sliced ? Image.Type.Sliced : Image.Type.Simple;
            this.image.color = options.color;
        }

        public override void Reset()
        {
            base.Reset();
            options.Reset();
            this.image.sprite = null;
            this.image.type = Image.Type.Simple;
            this.image.color = Color.black;
            this.rectWidth = 1;
            this.rectHeight = 1;
        }
    }
}
