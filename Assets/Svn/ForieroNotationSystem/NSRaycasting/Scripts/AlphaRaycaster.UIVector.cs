/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using ForieroEngine.Music.NotationSystem.Classes;

public partial class AlphaRaycaster : GraphicRaycaster
{

    /*private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float as_x = p.x - a.x;
        float as_y = p.y - a.y;

        bool s_ab = (b.x - a.x) * as_y - (b.y - a.y) * as_x >= 0;

        if ((c.x - a.x) * as_y - (c.y - a.y) * as_x >= 0 == s_ab) return false;

        if ((c.x - b.x) * (p.y - b.y) - (c.y - b.y) * (p.x - b.x) >= 0 != s_ab) return false;

        return true;
    }*/


    static float sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    static bool PointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        bool b1, b2, b3;

        b1 = sign(pt, v1, v2) <= 0;
        b2 = sign(pt, v2, v3) <= 0;
        b3 = sign(pt, v3, v1) <= 0;

        return ((b1 == b2) && (b2 == b3));
    }

    private bool AlphaCheckVector(GameObject obj, UIVector vector, AlphaRaycasterCheck objAlphaCheck, Vector2 pointerPos, OuterEdgeDetection mode)
    {
        if (mode == OuterEdgeDetection.ClosestRect || mode == OuterEdgeDetection.NativeRect)
        {
            return false;
        }

        if (mode == OuterEdgeDetection.Fill)
        {
            Debug.LogError("Vector unsupported detection mode" + mode.ToString());
            return false;
        }

        var pos = Input.mousePosition;

        /*var rect = RectTransformUtility.PixelAdjustRect(vector.rectTransform, vector.canvas);

        if (pos.x >= rect.xMin && pos.y >= rect.yMin && pos.x >= rect.xMax && pos.y <= rect.yMin)
        {
            return false;
        }*/

        var t = obj.transform;
        var cam = Camera.main;

        var verts = vector.raycastVerts;
        foreach (var quad in verts)
        {
            int i = 0;
            while (i < quad.Length)
            {
                var q1 = t.TransformPoint(quad[i].position); i++;
                var q2 = t.TransformPoint(quad[i].position); i++;
                var q3 = t.TransformPoint(quad[i].position); i++;
                var q4 = t.TransformPoint(quad[i].position); i++;

                if (PointInTriangle(pos, q3, q2, q1) || PointInTriangle(pos, q4, q3, q1))
                {
                    Debug.Log("HIT VECTORR");
                    return false;
                }

            }
        }

        return true;
    }

}
