/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using DG.Tweening;
using ForieroEngine.Music.Training;
using UnityEngine;
using UnityEngine.UI;

public class TLUnityMetronomePendulum : MonoBehaviour
{
    public Transform pendulumTransform;

    Tweener tweenerRotation = null;
    float flipRotation = 1f;

    Vector3 eulerOffset = Vector3.zero;
    Quaternion quaternionOffset = Quaternion.identity;

    public const float minRising = 10f;
    public const float maxRising = 45f;
    public const float minBPM = 20f;
    public const float maxBPM = 208f;

    float normalizedBPM { get { return 1 - (TL.Providers.metronome.bpm - minBPM) / (maxBPM - minBPM); } }
    float rising { get { return ((maxRising - minRising) * normalizedBPM + minRising); } }
    Vector3 risingVector3 { get { return new Vector3(0, 0, rising); } }

    private void Awake()
    {
        TLUnityMetronome.instance.OnStart += OnStart;
        TLUnityMetronome.instance.OnStop += OnStop;
        TLUnityMetronome.instance.OnBeat += OnBeat;
    }

    private void Start()
    {
        eulerOffset = pendulumTransform.eulerAngles;
        quaternionOffset = pendulumTransform.rotation;
    }

    private void OnDestroy()
    {
        TLUnityMetronome.instance.OnStart -= OnStart;
        TLUnityMetronome.instance.OnStop -= OnStop;
        TLUnityMetronome.instance.OnBeat -= OnBeat;

        if (tweenerRotation != null)
        {
            tweenerRotation.Kill();
        }
    }

    void OnStart()
    {
        tweenerRotation?.Kill();
        flipRotation = -1f;
        //pendulumTransform.eulerAngles = eulerOffset - risingVector3;
    }

    void OnStop()
    {
        tweenerRotation?.Kill();
        flipRotation = 1f;
        pendulumTransform.eulerAngles = eulerOffset;
    }

    void OnBeat(int beat, double beatDuration)
    {

    }

    private void Update()
    {
        if (TLUnityMetronome.instance.state == TLUnityMetronome.State.Running)
        {
            pendulumTransform.eulerAngles = new Vector3(eulerOffset.x, eulerOffset.y, eulerOffset.z + TLUnityMetronome.instance.pendulumAngle * rising * 2f);
        }
    }
}
