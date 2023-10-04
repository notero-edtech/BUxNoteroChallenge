using UnityEngine;
using System.Collections;

namespace ForieroEngine.Extensions
{
    public static partial class ForieroEngineExtensions
    {
        public static Rect Add(this Rect aRectA, Rect aRectB)
        {
            return new Rect(aRectA.x + aRectB.x, aRectA.y + aRectB.y, aRectA.width + aRectB.width, aRectA.height + aRectB.height);
        }

        static public string ToStringCSV(this Rect rct)
        {
            return rct.x.ToString() + ";" + rct.y.ToString() + ";" + rct.width.ToString() + ";" + rct.height.ToString();
        }

        static public Rect ParseCSV(this Rect vct, string aStringRect, Rect aDefaultRect)
        {
            string[] indexes = aStringRect.Split(';');
            if (indexes.Length == 4)
            {
                Rect result = new Rect(int.Parse(indexes[0]), int.Parse(indexes[1]), int.Parse(indexes[2]), int.Parse(indexes[3]));
                return result;
            }
            else
            {
                return aDefaultRect;
            }
            //return aDefaultRect;
        }

        static public Rect StayInAppWindow(this Rect rect)
        {
            if (rect.center.y < 0 + rect.height / 2)
                rect.y = 0;
            if (rect.center.y > Screen.height - rect.height / 2)
                rect.y = Screen.height - rect.height;
            if (rect.center.x < 0 + rect.width / 2)
                rect.x = 0;
            if (rect.center.x > Screen.width - rect.width / 2)
                rect.x = Screen.width - rect.width;
            return rect;
        }
    }
}
