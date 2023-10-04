/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Linq;
using System.Text.RegularExpressions;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;
using ForieroEngine.Music.NotationSystem.Extensions;
using UnityEngine;
using static ForieroEngine.Music.NotationSystem.NSPlayback.NSRollingPlayback;

public class NSRollingTopBottomSystemBouncingBall : MonoBehaviour
{
    public static void Init(int index, Bouncing bouncing = Bouncing.Right)
    {
        var staves = NS.instance.GetObjectsOfType<NSStave>(true).Where(x => x.pool == PoolEnum.NS_FIXED).ToList();
        foreach (var s in staves)
        {
            if (s.options.index == index)
            {
                var nsCircle = s.AddObject<NSCircle>(PoolEnum.NS_MOVABLE_OVERLAY);
                nsCircle.rectTransform.SetPivotAndAnchors(rollingMode == RollingMode.Top ? new Vector2(0.5f, 1f) : new Vector2(0.5f, 0f));
                nsCircle.SetScale(1.5f);
                nsCircle.Commit();
                
                var b = nsCircle.rectTransform.gameObject.AddComponent<NSRollingTopBottomSystemBouncingBall>();
                b.stave = s;
                b.bouncing = bouncing;
                b.nsCircle = nsCircle;
                b.nsCircle.onDestroyed += () => Destroy(b);
            }
        }
    }

    public enum Bouncing { Left, Right }

    private Bouncing bouncing = Bouncing.Right;
    public NSCircle nsCircle;
    private NSBehaviour nsB => NSBehaviour.instance;
    private NS ns => nsB.ns;
    private NSSystemSettings nsSystemSettings => nsB != null ? nsB.ns?.nsSystemSettings : null;
    
    private float alpha = 0.75f;

    private Color CircleColor => bouncing switch
    {
        Bouncing.Right => Color.green.A(alpha),
        Bouncing.Left => Color.yellow.A(alpha),
        _ => Color.white.A(alpha)
    };
   
    private NSStave stave;
    private NSObject current;
    private NSObject next;
    private Vector2 screenPoint;
    private Vector2 localPointInRT;

    private float y = 0;
    private float height = 50f;
    private float distance = 0f;
    private float distanceNormalized = 0f;
    private float pixelTimeCurrent = 0f;
    private float pixelTimeNext = 0f;
    private float xCurrent = 0f;
    private float xNext = 0f;
    private float xMax = 0f;
    private float xMin = 0f;
    private float x = 0f;
    private float sine = 0f;
    private float lineSizeY = 0f;
    private NSRollingPlaybackUpdater.StaveMidiState staveMidiState;

    private void Start()
    {
        nsCircle.SetColor(CircleColor);
    }
    
    private void Update()
    {
        if (stave.IsNull()) return;
        
        screenPoint = RectTransformUtility.WorldToScreenPoint(nsB.fixedCamera, nsB.hitZoneRT.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(nsB.fixedPoolRT, screenPoint, nsB.fixedCamera, out localPointInRT);

        y = (nsB.movableOverlayPoolRT.GetHeight() / 2f - Mathf.Abs(localPointInRT.y)) * (rollingMode == RollingMode.Top ? -1f : 1f);
        nsCircle.SetPositionY(y, false, false);
        
        for (var i = 0; i < nsB.rollingPlaybackUpdater.stavePositionStates.Count; i++)
        {
            staveMidiState = nsB.rollingPlaybackUpdater.stavePositionStates[i];
            if (staveMidiState.index != stave.options.index) continue;
            
            //Debug.Log("Current : " + staveMidiState.current.Count + " Next : " + staveMidiState.next.Count);
            current = bouncing == Bouncing.Right
                ? staveMidiState.current.LastOrDefault()
                : staveMidiState.current.FirstOrDefault();
            next = bouncing == Bouncing.Right
                ? staveMidiState.next.LastOrDefault()
                : staveMidiState.next.FirstOrDefault();
                
            lineSizeY = ns.LineSize * (rollingMode == RollingMode.Top ? -1f : 1f);
                
            pixelTimeCurrent = current == null ? 0f.ToTimePixels() : current.pixelTime;
            pixelTimeNext = next == null ? NSPlayback.Time.TotalTime.ToTimePixels() : next.pixelTime;
            distance = pixelTimeNext - pixelTimeCurrent;
                
            xCurrent = current == null ? (distance <= 1 ? next.GetPositionX(true) : 0) : current.GetPositionX(true);
            xNext = next == null ? (distance <= 1 ? xCurrent : 0) : next.GetPositionX(true);

            distanceNormalized = Mathf.Clamp(distance == 0 ? 0 : (1 - (pixelTimeNext - pixelPosition) / distance), 0f, 1f);
                
            sine = Mathf.Sin(distanceNormalized * Mathf.PI);
            x = xCurrent + (xNext - xCurrent) * distanceNormalized;
            nsCircle.SetPositionX(-nsB.movableCameraRT.anchoredPosition.x + x, false, true);
            nsCircle.SetPositionY(y + lineSizeY + 100f * sine * (rollingMode == RollingMode.Top ? -1f : 1f), false, true);
        }
    }
}
