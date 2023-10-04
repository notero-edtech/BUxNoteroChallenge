/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace ForieroEngine.Music.NotationSystem.Classes
{
	public static class VectorExtensions
	{
		public static IEnumerable<T> MergeNew<T> (this T[] arrayInitial, T[] arrayToAppend) where T : System.Collections.IEnumerable
		{
			if (arrayToAppend == null) {
				throw new ArgumentNullException ("The appended object cannot be null");
			}
			if ((arrayInitial is string) || (arrayToAppend is string)) {
				throw new ArgumentException ("The argument must be an enumerable");
			}

			T[] ret = new T[arrayInitial.Length + arrayToAppend.Length];
			arrayInitial.CopyTo (ret, 0);
			arrayToAppend.CopyTo (ret, arrayInitial.Length);

			return ret;
		}

        public static void FindRect(this List<UIVertex[]> verts, ref Rect rect)
        {
            foreach (UIVertex[] v in verts)
            {
                FindRect(v, ref rect);
            }
        }

        public static void FindRect(this UIVertex[] verts, ref Rect rect)
        {
            foreach (UIVertex v in verts)
            {
                FindRect(v, ref rect);
            }
        }

        public static void FindRect(this UIVertex vert, ref Rect rect)
        {
            if (vert.position.x > rect.xMax)
            {
                rect.xMax = vert.position.x;
            }

            if (vert.position.x < rect.xMin)
            {
                rect.xMin = vert.position.x;
            }

            if (vert.position.y > rect.yMax)
            {
                rect.yMax = vert.position.y;
            }

            if (vert.position.y < rect.yMin)
            {
                rect.yMin = vert.position.y;
            }
        }

    }
}
