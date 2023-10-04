/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class TLUnityMetronomePendulumWeight : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TLUnityMetronomeControl metronomeControl;

    public float weightYMin = 0.5f;
    public float weightYMax = 2.15f;
  
    private int _lastBpm;
    private Tweener _tweener;
    private RectTransform _rt;

    private void Awake()
    {
        _rt = transform as RectTransform;
    }
    
    private void Update()
    {
        if (_lastBpm == TLUnityMetronome.instance.bpm) return;
        
        _lastBpm = TLUnityMetronome.instance.bpm;
        SetWeightPosition(_lastBpm);
    }

    private void OnDestroy() { _tweener?.Kill(); }

    private void SetWeightPosition(int bpm)
    {
        var normalizedPosition = 1 - (bpm - metronomeControl.beatsMin) / (float)(metronomeControl.beatsMax - metronomeControl.beatsMin);
        var y = normalizedPosition * (weightYMax - weightYMin) + weightYMin;

        _tweener?.Kill();
        
        _tweener = _rt.DOAnchorPos(new Vector2(_rt.anchoredPosition.x, y), 0.2f).SetEase(Ease.InOutSine);
    }

    private void ClampLocalPosition()
    {
        _rt.anchoredPosition = new Vector2(_rt.anchoredPosition.x, Mathf.Clamp(_rt.anchoredPosition.y, weightYMin, weightYMax));
        var normalizedPosition = 1 - (_rt.anchoredPosition.y - weightYMin) / (weightYMax - weightYMin);
        var bpm = Mathf.RoundToInt(normalizedPosition * (metronomeControl.beatsMax - metronomeControl.beatsMin) + metronomeControl.beatsMin);
        _lastBpm = bpm;
        metronomeControl.SetBPM(bpm);
    }

    #region Interface Implementations

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (TLUnityMetronome.instance.state != TLUnityMetronome.State.Stop) return;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (TLUnityMetronome.instance.state != TLUnityMetronome.State.Stop) return;
        if (Input.touchCount > 1) return;
        _rt.anchoredPosition += Vector2.up * eventData.delta.y;
        ClampLocalPosition();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (TLUnityMetronome.instance.state != TLUnityMetronome.State.Stop) return;
    }

    #endregion
}
