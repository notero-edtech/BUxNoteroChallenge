/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Event/AlphaRaycaster Check"), ExecuteInEditMode]
public class AlphaRaycasterCheck : MonoBehaviour
{
    [Range(-0.5f, 0.5f), Tooltip("Texture regions with opacity (alpha) lower than alpha threshold won't react to input events.")]
    public float alphaThreshold = 0.0f;
    [Tooltip("Whether material tint color should affect alpha threshold.")]
    public bool includeMaterialAlpha;

    [Tooltip("Detect outer edges.")]
    public AlphaRaycaster.OuterEdgeDetection outerEdgeDetection = AlphaRaycaster.OuterEdgeDetection.NativeRect;

    public void Commit()
    {

    }
}
