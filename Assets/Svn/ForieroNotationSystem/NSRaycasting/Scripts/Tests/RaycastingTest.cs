/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RaycastingTest : MonoBehaviour
{
#if DEVELOPMENT_BUILD
    public static Texture detectedTexture;
#endif

    public Canvas canvas;

    public Graphic[] targets;

    string hits = "";

    void OnGUI()
    {
        GUILayout.Label("Scale Factor : " + canvas.scaleFactor);
        GUILayout.Label("Resolution : " + Screen.currentResolution.ToString());

        hits = "";

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);



            foreach (RaycastResult rr in results)
            {
                hits += rr.gameObject.name + ";";
            }
        }

        GUILayout.Label("HIT : " + hits);

#if DEVELOPMENT_BUILD
        if (detectedTexture != null)
        {
            GUI.DrawTexture(new Rect(10, 100, detectedTexture.width * 4, detectedTexture.height * 4), detectedTexture);
        }
#endif
    }

    void OnDrawGizmosSelected()
    {
        foreach (var target in targets)
        {
            Gizmos.color = Color.magenta;

            if (target is TextMeshProUGUI)
            {
                var tmp = ((TextMeshProUGUI)target);

                var rt = tmp.GetComponent<RectTransform>();
                foreach (var cInfo in tmp.textInfo.characterInfo)
                {
                    Vector3 bottomLeft = AlphaRaycaster.Convert(cInfo.bottomLeft, rt, canvas, true);
                    Vector3 topLeft = AlphaRaycaster.Convert(cInfo.topLeft, rt, canvas, true);
                    Vector3 topRight = AlphaRaycaster.Convert(cInfo.topRight, rt, canvas, true);
                    Vector3 bottomRight = AlphaRaycaster.Convert(cInfo.bottomRight, rt, canvas, true);

                    /*            bottomLeft = cam.WorldToScreenPoint(bottomLeft);
                                topLeft = cam.WorldToScreenPoint(topLeft);
                                bottomRight = cam.WorldToScreenPoint(bottomRight);
                                topRight = cam.WorldToScreenPoint(topRight);*/

                    var minX = Mathf.Min(Mathf.Min(bottomLeft.x, bottomRight.x), Mathf.Min(topLeft.x, topRight.x));
                    var minY = Mathf.Min(Mathf.Min(bottomLeft.y, bottomRight.y), Mathf.Min(topLeft.y, topRight.y));
                    var maxX = Mathf.Max(Mathf.Max(bottomLeft.x, bottomRight.x), Mathf.Max(topLeft.x, topRight.x));
                    var maxY = Mathf.Max(Mathf.Max(bottomLeft.y, bottomRight.y), Mathf.Max(topLeft.y, topRight.y));

                    Gizmos.DrawLine(new Vector2(minX, minY), new Vector2(maxX, minY));
                    Gizmos.DrawLine(new Vector2(maxX, minY), new Vector2(maxX, maxY));
                    Gizmos.DrawLine(new Vector2(maxX, maxY), new Vector2(minX, maxY));
                    Gizmos.DrawLine(new Vector2(minX, maxY), new Vector2(minX, minY));
                }


            }

            /*
			 Rect rect;
            Vector3[] corners = new Vector3[4];
            target.GetComponent<RectTransform>().GetWorldCorners(corners);
            for (int i=0; i<3; i++)
            {
                Gizmos.DrawLine(corners[i], corners[(i+1)%2]);

            }*/



            /*if (target is TextMeshProUGUI)
            {
                rect = ((TextMeshProUGUI)target).GetBestFitRect();
            }
            else
            if (target is Text)
            {
                rect = ((Text)target).GetBestFitRect();
            }
            else
            {
                continue;
            }

            //var size = AlphaRaycaster.GetBestFitRect(target);
            //Gizmos.DrawLine(new Vector2(0, 0), new Vector2(size.x, size.y));
            Gizmos.DrawLine(new Vector2(rect.min.x, rect.min.y), new Vector2(rect.max.x, rect.min.y));
            Gizmos.DrawLine(new Vector2(rect.max.x, rect.min.y), new Vector2(rect.max.x, rect.max.y));
            Gizmos.DrawLine(new Vector2(rect.max.x, rect.max.y), new Vector2(rect.min.x, rect.max.y));
            Gizmos.DrawLine(new Vector2(rect.min.x, rect.max.y), new Vector2(rect.min.x, rect.min.y));*/
        }
    }

}
