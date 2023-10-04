/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem.Extensions;
using TMPro;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    /// <summary>
    /// We need to add some nice vector library
    /// </summary>
    public class NSObjectVector : NSObject, INSColorable
    {
        UIVector _vector;

        public UIVector vector
        {
            get
            {
                if (!_vector) { _vector = rectTransform.gameObject.GetComponent<UIVector>(); }
                return _vector;
            }
        }

        public override void Commit()
        {
            base.Commit();

            vector.raycastTarget = selectable;

            vector.Commit();
        }

        public override void Reset()
        {
            base.Reset();

            vector.vectorEnum = VectorEnum.Undefined;
        }

        #region INSColorable implementation

        public void SetColor(Color color)
        {
            this.color = vector.color = color;
            // THIS WILL TRIGGER VERTEX RECALCULATION WHICH WE DON"T WNAT //
            vector.Commit();
        }

        public void SetAlpha(float alpha)
        {
            this.color = vector.color = vector.color.A(alpha);
            // THIS WILL TRIGGER VERTEX RECALCULATION WHICH WE DON"T WNAT //
            vector.Commit();
        }
        public Color GetColor() => vector.color;
        
        #endregion
    }

}
