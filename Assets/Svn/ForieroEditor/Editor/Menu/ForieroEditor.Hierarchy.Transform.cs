using ForieroEditor.Extensions;
using UnityEditor;
using UnityEngine;

namespace ForieroEditor.Menu
{
    public static partial class MenuItems
    {
        [MenuItem("GameObject/Integrator Tool 2D/Center Pivot", false, 1)]
        public static void CenterPivot()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                o.transform.CenterPivot(false, false);
            }
        }


        [MenuItem("GameObject/Integrator Tool 2D/Center Pivots", false, 1)]
        public static void CenterPivots()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                o.transform.CenterPivot(true, false);
            }
        }

        [MenuItem("GameObject/Integrator Tool 2D/Center Pivot 2D", false, 1)]
        public static void CenterPivot2D()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                o.transform.CenterPivot(false, true);
            }
        }


        [MenuItem("GameObject/Integrator Tool 2D/Center Pivots 2D", false, 1)]
        public static void CenterPivots2D()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                o.transform.CenterPivot(true, true);
            }
        }
    }
}
