using UnityEngine;
using System.Collections;
using DG.Tweening;

public static partial class UnityExtensions {
    public static IEnumerator AnimateTo(this Color aColor,
                                     Color aTo,
                                     float aDuration,
                                     float aDelay,
                                     System.Func<float, float, float, float> anEaseFunc,
                                     System.Action<Color> aCallback,
                                     System.Action onStarted,
                                     System.Action onCompleted,
                                     bool aReverse,
                                     string aCategory,
                                     string anId)
    {
        var fromValue = aColor;
        var time = 0f;
        var timeClamp01 = 0f;
        var duration = aDuration;
        var delay = aDelay;

        CancelSignal cs = null;
        cs = GetFreeCancelSignal();
        cs.id = anId;
        cs.category = aCategory;

        while (delay > 0)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        onStarted?.Invoke();

        while (duration > 0f)
        {
            if (cs != null)
            {
                if (!cs.paused)
                {
                    time += Time.deltaTime;
                    duration -= Time.deltaTime;
                }
            }
            else
            {
                time += Time.deltaTime;
                duration -= Time.deltaTime;
            }

            timeClamp01 = aReverse ? 1 - Mathf.Clamp01(time / aDuration) : Mathf.Clamp01(time / aDuration);
            aColor.r = anEaseFunc(fromValue.r, aTo.r, timeClamp01);
            aColor.g = anEaseFunc(fromValue.g, aTo.g, timeClamp01);
            aColor.b = anEaseFunc(fromValue.b, aTo.b, timeClamp01);
            aColor.a = anEaseFunc(fromValue.a, aTo.a, timeClamp01);

            if (cs.cancel == true) break;

            aCallback(aColor);

            if (duration > 0f) yield return null;
        }

        onCompleted?.Invoke();

        cs.id = "";
        cs.cancel = false;
        cs.paused = false;
        cs.free = true;
        yield break;
    }
}
