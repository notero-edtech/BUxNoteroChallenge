/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using ForieroEngine.Music.NotationSystem.Classes;

[AddComponentMenu("Event/Alpha Raycaster"), ExecuteInEditMode]
public partial class AlphaRaycaster : GraphicRaycaster
{
    public enum OuterEdgeDetection
    {
        NativeRect,
        ClosestRect,
        Fill,
        Alpha
    }

    [Header("Alpha test properties")]
    [Range(0, 1), Tooltip("Texture regions of the UI objects with opacity (alpha) lower than alpha threshold won't react to input events.")]
    public float AlphaThreshold = .9f;
    [Tooltip("Whether material tint color of the UI objects should affect alpha threshold.")]
    public bool IncludeMaterialAlpha;
    [Tooltip("When selective mode is active the alpha testing will only execute for UI objects with AlphaCheck component.")]
    public bool SelectiveMode;
    [Tooltip("Show warnings in the console when attempting to alpha test objects with a not-readable texture.")]
    public bool ShowTextureWarnings;

    private List<RaycastResult> toExclude = new List<RaycastResult>();

    protected override void OnEnable()
    {
        base.OnEnable();

        var badGuy = GetComponent<GraphicRaycaster>();
        if (badGuy && badGuy != this) DestroyImmediate(badGuy);
    }

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        base.Raycast(eventData, resultAppendList);

        toExclude.Clear();

#if DEVELOPMENT_BUILD
        RaycastingTest.detectedTexture = null;
#endif

        //var foundGraphics = GraphicRegistry.GetGraphicsForCanvas(this.GetComponent<Canvas>());

        foreach (var result in resultAppendList)
        {
            var objAlphaCheck = result.gameObject.GetComponent<AlphaRaycasterCheck>();
            if (SelectiveMode && !objAlphaCheck) continue;

            OuterEdgeDetection edgeDetection = objAlphaCheck != null ? objAlphaCheck.outerEdgeDetection : OuterEdgeDetection.Alpha;
            float thresold = objAlphaCheck != null ? objAlphaCheck.alphaThreshold : 0.0f;

            try
            {
                var objImage = result.gameObject.GetComponent<Image>();
                if (objImage)
                {
                    if (objImage.sprite && AlphaCheckImage(result.gameObject, objImage, objAlphaCheck, eventData.position, edgeDetection))
                    {
                        toExclude.Add(result);
                    }
                }
                else
                {
                    var objTextMeshPro = result.gameObject.GetComponent<TextMeshProUGUI>();
                    if (objTextMeshPro)
                    {
                        if (AlphaCheckTextMeshPro(result.gameObject, objTextMeshPro, objAlphaCheck, eventData.position, edgeDetection, thresold))
                        {
                            toExclude.Add(result);
                        }
                    }

                    else
                    {
                        var objVector = result.gameObject.GetComponent<UIVector>();
                        if (objVector)
                        {
                            if (AlphaCheckVector(result.gameObject, objVector, objAlphaCheck, eventData.position, edgeDetection))
                            {
                                toExclude.Add(result);
                            }
                        }
                        else
                        {
                            var objText = result.gameObject.GetComponent<Text>();
                            if (objText && objText.font)
                            {
                                if (objText.font.dynamic)
                                {
                                    if (objText && AlphaCheckDynamicText(result.gameObject, objText, objAlphaCheck, eventData.position, edgeDetection, thresold))
                                        toExclude.Add(result);
                                }
                                else
                                {
                                    if (objText && AlphaCheckText(result.gameObject, objText, objAlphaCheck, eventData.position))
                                        toExclude.Add(result);
                                }

                            }
                        }
                    }
                }
            }
            catch (UnityException e)
            {
                if (Application.isEditor && ShowTextureWarnings)
                    Debug.LogWarning(string.Format("Alpha test failed: {0}", e.Message));
            };
        }

        resultAppendList.RemoveAll(r => toExclude.Contains(r));

        /*
        #if UNITY_EDITOR
                foreach (var obj in resultAppendList)
                {
                    var objAlphaCheck = obj.gameObject.GetComponent<AlphaRaycasterCheck>();
                    OuterEdgeDetection mode = objAlphaCheck != null ? objAlphaCheck.outerEdgeDetection : OuterEdgeDetection.Alpha;
                    Debug.Log("Hit: " + obj.gameObject.name + " (" + mode + " mode)");
                }
        #endif
        */
    }

    // Evaluating pointer position relative to UI object local space.
    private Vector3 ScreenToLocalObjectPosition(Vector2 screenPosition, RectTransform objTrs)
    {
        Vector3 pointerGPos;
        if (eventCamera)
        {
            var objPlane = new Plane(objTrs.forward, objTrs.position);
            float distance;
            var cameraRay = eventCamera.ScreenPointToRay(screenPosition);
            objPlane.Raycast(cameraRay, out distance);
            pointerGPos = cameraRay.GetPoint(distance);
        }
        else
        {
            pointerGPos = screenPosition;
            float rotationCorrection = (-objTrs.forward.x * (pointerGPos.x - objTrs.position.x) - objTrs.forward.y * (pointerGPos.y - objTrs.position.y)) / objTrs.forward.z;
            pointerGPos += new Vector3(0, 0, objTrs.position.z + rotationCorrection);
        }
        return objTrs.InverseTransformPoint(pointerGPos);
    }
}
