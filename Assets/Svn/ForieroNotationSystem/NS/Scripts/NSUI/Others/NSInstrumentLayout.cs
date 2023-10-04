/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Interfaces;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using KeySignatureEnum = ForieroEngine.Music.NotationSystem.KeySignatureEnum;

public class NSInstrumentLayout : MonoBehaviour
{
    public NSBehaviour nsBehaviour;
    public NSBackgroundDrag nsBackgroundDrag;
    public RectTransform midiControllersCanvasRT;
    public RectTransform midiInstrumentsRT;
    [FormerlySerializedAs("midiControllersContainerRT")] public RectTransform midiControllersRT;
    
    private IMidiSender _nsMidiSender;
    private List<IMidiInstrument> _midiInstruments = new ();
    private List<IMidiController> _midiControllers = new ();
    private List<IMidiReceiver> _midiReceivers = new ();
    private IMidiReceiver _midiReceiver;

    private MidiEvents _me = new (); 
    
    private void Awake()
    {
        NSPlayback.onSystemChanged += OnSystemChanged;
        NSPlayback.OnZoomChanged += OnZoomChanged;
        NSPlayback.OnPlaybackStateChanged += OnPlaybackStateChanged;
        NSPlayback.OnTimeSignatureChanged += OnTimeSignatureChanged;
        NSPlayback.OnKeySignatureChanged += OnKeySignatureChanged;
        NSPlayback.OnSongInitialized += OnSongInitialized;
        _nsMidiSender = nsBehaviour as IMidiSender;
        _me.AddSender(_nsMidiSender);
        _me.ShortMessageEvent += MeOnShortMessageEvent;
        
        nsBackgroundDrag.OnBeginDragEvent += (p) => InternalUpdate();
        nsBackgroundDrag.OnDragEvent += (p) => InternalUpdate();
        nsBackgroundDrag.OnEndDragEvent += (p) => InternalUpdate();
        
        Instantiate(NSInstrumentsSettings.instance.instruments[0].prefab, midiInstrumentsRT);
        Instantiate(NSInstrumentsSettings.instance.instruments[0].controllers[0].prefab, midiControllersRT);

        _midiInstruments = this.GetComponentsInChildren<IMidiInstrument>().ToList();
        _midiControllers = this.GetComponentsInChildren<IMidiController>().ToList();
        _midiReceivers = this.GetComponentsInChildren<IMidiReceiver>().ToList();
    }

    IEnumerator Start()
    {
        if (_midiInstruments.Count == 0) yield break;
        _midiInstruments[0].Hide(false);
        yield return null;
        yield return null;
        yield return null;
        _midiInstruments[0].Show(true);
    }

    private void MeOnShortMessageEvent(int aCommand, int aData1, int aData2, int aDeviceId)
    {
        for(var i = 0; i<_midiReceivers.Count;i++) _midiReceivers[i]?.OnMidiMessageReceived(aCommand, aData1, aData2, aDeviceId);
    }

    private void OnSongInitialized()
    {
        UpdateHitZone();
    }

    private void OnDestroy()
    {
        NSPlayback.onSystemChanged -= OnSystemChanged;
        NSPlayback.OnZoomChanged -= OnZoomChanged;
        NSPlayback.OnPlaybackStateChanged -= OnPlaybackStateChanged;
        NSPlayback.OnTimeSignatureChanged -= OnTimeSignatureChanged;
        NSPlayback.OnKeySignatureChanged -= OnKeySignatureChanged;
        NSPlayback.OnSongInitialized += OnSongInitialized;
        _me.RemoveAllSenders();
    }

    public KeySignatureEnum keySignatureEnum = KeySignatureEnum.Undefined;
    public KeyModeEnum keyModeEnum = KeyModeEnum.Undefined;

    public void InitPiano()
    {
        switch (NSSettingsStatic.keysNamesEnum)
        {
            case ToneNamesEnum.Undefined:
                MIDITheorySettings.instance.theorySystem = ForieroEngine.MIDIUnified.TheorySystemEnum.Undefined;
                MIDITheorySettings.instance.keySignature = ForieroEngine.MIDIUnified.KeySignatureEnum.CMaj_AMin;
                break;
            case ToneNamesEnum.ToneNames:
                MIDITheorySettings.instance.theorySystem = ForieroEngine.MIDIUnified.TheorySystemEnum.ToneNames;
                MIDITheorySettings.instance.keySignature = (ForieroEngine.MIDIUnified.KeySignatureEnum)keySignatureEnum;
                break;
            case ToneNamesEnum.SolfegeFixedSymbolic:
            case ToneNamesEnum.SolfegeFixed:
                MIDITheorySettings.instance.theorySystem = ForieroEngine.MIDIUnified.TheorySystemEnum.SolfegeFixed;
                MIDITheorySettings.instance.keySignature = ForieroEngine.MIDIUnified.KeySignatureEnum.CMaj_AMin;
                break;
            case ToneNamesEnum.SolfegeMovableSymbolic:
            case ToneNamesEnum.SolfegeMovable:
                MIDITheorySettings.instance.theorySystem = ForieroEngine.MIDIUnified.TheorySystemEnum.SolfegeMovable;
                MIDITheorySettings.instance.keySignature = (ForieroEngine.MIDIUnified.KeySignatureEnum)keySignatureEnum;
                break;
        }
    }

    private void OnTimeSignatureChanged(int part, int stave, ForieroEngine.Music.NotationSystem.Classes.NSTimeSignature.Options options)
    {
    }

    private void OnKeySignatureChanged(int part, int stave, ForieroEngine.Music.NotationSystem.Classes.NSKeySignature.Options options)
    {
        if (part == 0 && stave == 0)
        {
            keySignatureEnum = options.keySignatureEnum;
            keyModeEnum = options.keyModeEnum;
        }

        InitPiano();
    }

    private void OnPlaybackStateChanged(NSPlayback.PlaybackState state)
    {
        //Piano.singleton.AllKeysUp();
    }

    private float Zoom() => NSPlayback.Zoom * nsBehaviour.fixedCanvasRT.GetWidth() /
                            nsBehaviour.backgroundCanvasRT.anchoredPosition.x;

    private void OnZoomChanged(float zoom)
    {
        switch (NSPlayback.NSRollingPlayback.rollingMode)
        {
            case NSPlayback.NSRollingPlayback.RollingMode.Undefined:
            case NSPlayback.NSRollingPlayback.RollingMode.Left:
            case NSPlayback.NSRollingPlayback.RollingMode.Right:
                foreach (var m in _midiControllers) m.Transform.localScale = Vector3.one;
                break;
            case NSPlayback.NSRollingPlayback.RollingMode.Bottom:
            case NSPlayback.NSRollingPlayback.RollingMode.Top:
                foreach (var m in _midiControllers) m.Transform.localScale = Vector3.one * NSPlayback.Zoom;
                break;
        }
        
        if(MidiControllers.Selected != null && MidiControllers.Selected.Hidden) MidiControllers.Selected.Hide(false);
        
        UpdateHitZone();
        InternalUpdate();
    }

    private void OnSystemChanged(SystemEnum system)
    {
        
        foreach (var m in _midiControllers) m.Align(NSPlayback.ControllerAlignment);
        
        if (MidiControllers.Selected != null)
        {
            if (MidiControllers.Selected.Hidden) MidiControllers.Selected.Hide(false);
            else MidiControllers.Selected.Show(false);
        }

        InternalUpdate();
    }

    private void Update() { UpdateHitZone(); }

    private Vector3 _screenPoint;
    private Vector2 _localPoint;
    private float _sign;
    private float _height;
    private void UpdateHitZone()
    {
        if (nsBehaviour.hitZoneRT == null) return;
        if (NSPlayback.NSRollingPlayback.rollingMode is not (NSPlayback.NSRollingPlayback.RollingMode.Top or NSPlayback.NSRollingPlayback.RollingMode.Bottom)) return;
        
        _screenPoint = Vector3.zero;
        _sign = 1f;
        
        switch (NSPlayback.NSRollingPlayback.rollingMode)
        {
            case NSPlayback.NSRollingPlayback.RollingMode.Top: _screenPoint = MidiControllers.Selected.GetWorldPosition(ControllerPosition.Bottom); _sign = -1f; break;
            case NSPlayback.NSRollingPlayback.RollingMode.Bottom: _screenPoint = MidiControllers.Selected.GetWorldPosition(ControllerPosition.Top);; _sign = 1f; break;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(nsBehaviour.fixedCanvasRT, _screenPoint, nsBehaviour.fixedCamera, out _localPoint);
        _height = (nsBehaviour.fixedCanvasRT.GetHeight() / 2f - Mathf.Abs(_localPoint.y)) * _sign;
        if (!Mathf.Approximately(nsBehaviour.hitZoneRT.anchoredPosition.y, _height)) { nsBehaviour.hitZoneRT.anchoredPosition = nsBehaviour.hitZoneRT.anchoredPosition.SetY(_height); }
    }

    private void InternalUpdate()
    {
        _screenPoint = nsBehaviour.fixedCamera.WorldToScreenPoint(Vector3.zero);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(midiControllersCanvasRT, _screenPoint, null, out _localPoint);
        switch (NSPlayback.NSRollingPlayback.rollingMode)
        {
            case NSPlayback.NSRollingPlayback.RollingMode.Top: 
            case NSPlayback.NSRollingPlayback.RollingMode.Bottom: 
                midiControllersRT.anchoredPosition = new Vector2(_localPoint.x, midiControllersRT.anchoredPosition.y);
                break;
            case NSPlayback.NSRollingPlayback.RollingMode.Left: break;
            case NSPlayback.NSRollingPlayback.RollingMode.Right: break;
            case NSPlayback.NSRollingPlayback.RollingMode.Undefined:
                midiControllersRT.anchoredPosition = new Vector2(0, midiControllersRT.anchoredPosition.y);
                break;
        }
    }
}
