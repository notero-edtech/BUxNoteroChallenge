/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Linq;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;
using ForieroEngine.Music.NotationSystem.Extensions;
using UnityEngine;
using static ForieroEngine.Music.NotationSystem.NSPlayback.NSRollingPlayback;

public class NSRollingLeftRightSystemBouncingBall : MonoBehaviour
{
    public static void Init(int index, Bouncing bouncing = Bouncing.Top)
    {
        var staves = NS.instance.GetObjectsOfType<NSStave>(true).Where(x => x.pool == PoolEnum.NS_FIXED).ToList();
        foreach (var s in staves)
        {
            if (s.options.index == index)
            {
                var nsCircle = s.AddObject<NSCircle>(PoolEnum.NS_MOVABLE_OVERLAY);
                nsCircle.rectTransform.SetPivotAndAnchors(rollingMode == RollingMode.Left ? new Vector2(0, 0.5f) : new Vector2(1, 0.5f));
                nsCircle.SetScale(1.5f);
                nsCircle.Commit();
                
                var b = nsCircle.rectTransform.gameObject.AddComponent<NSRollingLeftRightSystemBouncingBall>();
                b.stave = s;
                b.bouncing = bouncing;
                b.nsCircle = nsCircle;
                b.nsCircle.onDestroyed += () => Destroy(b);
            }
        }
    }

    public enum Bouncing
    {
        Top,
        Bottom
    }

    private Bouncing bouncing = Bouncing.Bottom;
    public NSCircle nsCircle;
    private NSBehaviour nsB => NSBehaviour.instance;
    private NS ns => nsB.ns;
    private NSSystemSettings nsSystemSettings => nsB != null ? nsB.ns?.nsSystemSettings : null;
    
    private float alpha = 0.75f;

    private Color CircleColor => bouncing switch
    {
        Bouncing.Top => Color.green.A(alpha),
        Bouncing.Bottom => Color.yellow.A(alpha),
        _ => Color.white.A(alpha)
    };
   
    private NSStave stave;
    private NSObject current;
    private NSObject next;
    private Vector2 screenPoint;
    private Vector2 localPointInRT;

    private float height = 50f;
    private float distance = 0f;
    private float distanceNormalized = 0f;
    private float pixelTimeCurrent = 0f;
    private float pixelTimeNext = 0f;
    private float yCurrent = 0f;
    private float yNext = 0f;
    private float yMax = 0f;
    private float yMin = 0f;
    private float y = 0f;
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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(nsB.movableOverlayPoolRT, screenPoint, nsB.movableCamera, out localPointInRT);

        nsCircle.SetPositionX((nsB.movableOverlayPoolRT.GetWidth() / 2f - Mathf.Abs(localPointInRT.x)) * (rollingMode == RollingMode.Left ? 1f : -1f), false, false);
        for (var i = 0; i < nsB.rollingPlaybackUpdater.stavePositionStates.Count; i++)
        {
            staveMidiState = nsB.rollingPlaybackUpdater.stavePositionStates[i];
            if (staveMidiState.index != stave.options.index) continue;
            
            //Debug.Log("Current : " + staveMidiState.current.Count + " Next : " + staveMidiState.next.Count);
            current = bouncing == Bouncing.Top
                ? staveMidiState.current.LastOrDefault()
                : staveMidiState.current.FirstOrDefault();
            next = bouncing == Bouncing.Top
                ? staveMidiState.next.LastOrDefault()
                : staveMidiState.next.FirstOrDefault();
                
            lineSizeY = ns.LineSize * (bouncing == Bouncing.Top ? 1f : -1f);

            pixelTimeCurrent = current == null ? 0f.ToTimePixels() : current.pixelTime;
            pixelTimeNext = next == null ? NSPlayback.Time.TotalTime.ToTimePixels() : next.pixelTime;
            distance = pixelTimeNext - pixelTimeCurrent;
            yCurrent = current == null ? (distance <= 1 ? next.GetPositionY(true) : -lineSizeY) : current.GetPositionY(true);
            yNext = next == null ? (distance <= 1 ? yCurrent : -lineSizeY) : next.GetPositionY(true);

            distanceNormalized = Mathf.Clamp(distance == 0 ? 0 : (1 - (pixelTimeNext - pixelPosition) / distance),
                0f,
                1f);
            yMax = Mathf.Max(yCurrent, yNext);
            yMin = Mathf.Min(yCurrent, yNext);
            y = 0;
            sine = Mathf.Sin(distanceNormalized * Mathf.PI);
                
            if (distanceNormalized <= 0.5f)
            {
                y = bouncing switch
                {
                    Bouncing.Top => yCurrent + (yMax - yCurrent + height) * sine + lineSizeY,
                    Bouncing.Bottom => yCurrent + (yMin - yCurrent - height) * sine + lineSizeY,
                    _ => y
                };
            }
            else
            {
                y = bouncing switch
                {
                    Bouncing.Top => y = yNext + (yMax - yNext + height) * sine + lineSizeY,
                    Bouncing.Bottom => y = yNext + (yMin - yNext - height) * sine + lineSizeY,
                    _ => y
                };
            }

            nsCircle.SetPositionY(-nsB.movableCameraRT.anchoredPosition.y + y, false, true);
        }
    }
}
