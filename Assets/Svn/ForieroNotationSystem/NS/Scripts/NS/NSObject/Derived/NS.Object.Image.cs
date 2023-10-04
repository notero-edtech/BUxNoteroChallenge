/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSObjectImage : NSObject, INSColorable
    {
        private Image _image;

        public Image image
        {
            get
            {
                if (!_image) { _image = rectTransform.gameObject.GetComponent<Image>(); }
                return _image;
            }
        }

        public override void Commit()
        {
            base.Commit();
            image.raycastTarget = base.selectable;
        }

        public override void Reset()
        {
            base.Reset();
            image.sprite = null;
        }

        #region INSColorable implementation

        public void SetColor(Color color) => image.color = color;
        public void SetAlpha(float alpha) => image.color = image.color.A(alpha);
        public Color GetColor() => image.color;
        
        #endregion
    }
}
