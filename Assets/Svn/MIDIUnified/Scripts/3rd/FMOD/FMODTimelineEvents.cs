using System;
using System.Runtime.InteropServices;
using AOT;

using UnityEngine;
using Debug = UnityEngine.Debug;

#if FMOD
using FMOD;
using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
#endif

public class FMODTimelineEvents : IDisposable
{
    #if FMOD
    [StructLayout(LayoutKind.Sequential)]
    class TimelineInfo
    {
        public int bar = 0;
        public int beat = 0;
        public int position = 0;
        public float tempo = 0;
        public int timesignatureNominator = 0;
        public int timesignatureDenominator = 0;
        public bool markerChanged = false;
        public StringWrapper marker = new StringWrapper();
        public int markerPosition = 0;
    }
    
    private TimelineInfo _timelineInfo = new TimelineInfo();
    private GCHandle _timelineHandle;

    private EVENT_CALLBACK _callback;
    public EventInstance EventInstance => _ei;
    private EventInstance _ei;
    private int _markerPosition;

    public string Marker => _timelineInfo.marker;
    public int MarkerPosition => _timelineInfo.markerPosition;
    public string MarkerPositionString => TimeSpan.FromMilliseconds(MarkerPosition).ToString(@"mm\:ss\.fff");
    public int Bar => _timelineInfo.bar;
    public int Beat => _timelineInfo.beat;
    public int Position => _timelineInfo.position;
    public float Tempo => _timelineInfo.tempo;
    public int TimesignatureNominator => _timelineInfo.timesignatureNominator;
    public int TimesignatureDenominator => _timelineInfo.timesignatureDenominator;

    public Action<string, int> OnMarker;
    public Action<int, int> OnBar;
    public Action<int, int> OnBeat;
    public Action<float, int> OnTempo;
    public Action<int, int, int> OnTimeSignature;

    private TimelineInfo _lastTimeLineInfo = new TimelineInfo();
    
    public void InvokeEvents()
    {
        if (_timelineInfo.markerChanged)
        {
            _lastTimeLineInfo.marker = _timelineInfo.marker;
            _lastTimeLineInfo.markerPosition = _timelineInfo.markerPosition;
            OnMarker?.Invoke(_timelineInfo.marker, _timelineInfo.markerPosition);
        }
        
        if (_lastTimeLineInfo.bar != _timelineInfo.bar)
        {
            _lastTimeLineInfo.bar = _timelineInfo.bar;
            OnBar?.Invoke(_timelineInfo.bar, _timelineInfo.position);
        }
        
        if (_lastTimeLineInfo.beat != _timelineInfo.beat)
        {
            _lastTimeLineInfo.beat = _timelineInfo.beat;
            OnBeat?.Invoke(_timelineInfo.beat, _timelineInfo.position);
        }
        
        if (!Mathf.Approximately(_lastTimeLineInfo.tempo,_timelineInfo.tempo))
        {
            _lastTimeLineInfo.tempo = _timelineInfo.tempo;
            OnTempo?.Invoke(_timelineInfo.tempo, _timelineInfo.position);
        }
        
        if (_lastTimeLineInfo.timesignatureNominator != _timelineInfo.timesignatureDenominator ||
            _lastTimeLineInfo.timesignatureNominator != _timelineInfo.timesignatureNominator)
        {
            _lastTimeLineInfo.timesignatureNominator = _timelineInfo.timesignatureNominator;
            _lastTimeLineInfo.timesignatureDenominator = _timelineInfo.timesignatureDenominator;
            OnTimeSignature?.Invoke(_timelineInfo.timesignatureNominator, _timelineInfo.timesignatureDenominator, _timelineInfo.position);
        }
    }
    
    public FMODTimelineEvents(EventInstance ei)
    {
        if (!ei.isValid()) return;
        _ei = ei;        
        _timelineInfo = new TimelineInfo();

        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        _callback = new EVENT_CALLBACK(BeatEventCallback);
        
        // Pin the class that will store the data modified during the callback
        _timelineHandle = GCHandle.Alloc(_timelineInfo, GCHandleType.Pinned);
        // Pass the object through the userdata of the instance
        _ei.setUserData(GCHandle.ToIntPtr(_timelineHandle));

        _ei.setCallback(_callback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT | EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
    }

    ~FMODTimelineEvents() => Dispose();
    public void Dispose()
    {
        if(_ei.isValid()) _ei.setUserData(IntPtr.Zero);
        _timelineHandle.Free();
    }
   
    [MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    static RESULT BeatEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        //if(Thread.CurrentThread != mainThread ) Debug.LogWarning("BeatEventCallback is on its own thread!!!");
        
        EventInstance instance = new EventInstance(instancePtr);
        
        // Retrieve the user data
        IntPtr timelineInfoPtr;
        RESULT result = instance.getUserData(out timelineInfoPtr);
        if (result != RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                {
                    var parameter = (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_BEAT_PROPERTIES));
                    timelineInfo.bar = parameter.bar;
                    timelineInfo.beat = parameter.beat;
                    timelineInfo.position = parameter.position;
                    timelineInfo.tempo = parameter.tempo;
                    timelineInfo.timesignatureNominator = parameter.timesignatureupper;
                    timelineInfo.timesignatureDenominator = parameter.timesignaturelower;
                }
                    break;
                case EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                {
                    var parameter = (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_MARKER_PROPERTIES));
                    timelineInfo.markerChanged = true;
                    timelineInfo.marker = parameter.name;
                    timelineInfo.markerPosition = parameter.position;
                }
                    break;
            }
        }
        return RESULT.OK;
    }
    #else
    public void Dispose() { }
    #endif
}