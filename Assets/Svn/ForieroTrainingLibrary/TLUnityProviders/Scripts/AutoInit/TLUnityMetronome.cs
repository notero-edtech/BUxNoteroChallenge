/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;
using ForieroEngine.MIDIUnified;
using ForieroEngine.Music.NotationSystem;
using UnityEngine;

public partial class TLUnityMetronome : MonoBehaviour
{
    // PUBLIC STATIC //
    public static TLUnityMetronome instance;

    // PUBLIC //
    public enum State
    {
        Stop,
        Start,
        Running,
    }

    public State state = State.Stop;

    public Action<int, double> OnBeat;
    public Action<int, double> OnSubdivision;
    public Action OnStart;
    public Action OnStop;

    public float normalizedTime = 0f;
    public float pendulumAngle = 0f;
    public bool pendulumOnset = false;

    // PRIVATE //

    private int currentBeats = 0;
    private float currentBPM = 0f;
    private int currentSubdivisions = 0;

    private int valueBeats = 0;
    private float valueBPM = 0f;
    private int valueSubdivisions = 0;

    private BeatItem[] beatItems = Array.Empty<BeatItem>();

    private double startTime = 0;
    private double measureStartTime = 0;

    private double pickupBarTime = 0;
    private double pendulumOnsetTime = 0;
    private double nextBeatTime = 0;
    private bool nextBeatFlag = false;

    private float avgFrameTime = 0;
    private int prevBeatIndex = -1;
    private int nextBeatIndex = 0;
    private double prevBeatTime = 0;
    private BeatItem prevBeatItem = null;
    private BeatItem nextBeatItem = null;

    private void Awake()
    {
        instance = this;
    }

    public double StartMetronome()
    {
        // MIDIPercussionSettings.instance.rhythm = (PercussionEnum)TL.Exercises.settings.rhythm;
        // MIDIPercussionSettings.instance.rhythmVolume = TL.Exercises.settings.rhythmAttack.ToVolume();
        // MIDIPercussionSettings.instance.metronomeHeavy = (PercussionEnum)TL.Exercises.settings.metronomHeavy;
        // MIDIPercussionSettings.instance.metronomeHeavyVolume = TL.Exercises.settings.metronomHeavyAttack.ToVolume();
        // MIDIPercussionSettings.instance.metronomeLight = (PercussionEnum)TL.Exercises.settings.metronomLight;
        // MIDIPercussionSettings.instance.metronomeLightVolume = TL.Exercises.settings.metronomLightAttack.ToVolume();
        // MIDIPercussionSettings.instance.metronomeSubdivision =
        //     (PercussionEnum)TL.Exercises.settings.metronomSubdivision;
        // MIDIPercussionSettings.instance.metronomeSubdivisionVolume =
        //     TL.Exercises.settings.metronomSubdivisionAttack.ToVolume();

        nextBeatTime = double.MaxValue;

        pickupBarTime = pickupBar ? 60.0 / bpm * beats : 0;
        pendulumOnsetTime = pendulumOnset ? 60.0 / bpm + (60.0 / bpm) / 2.0 : 0;

        currentBPM = valueBPM = bpm;
        currentBeats = valueBeats = beats;
        currentSubdivisions = valueSubdivisions = subdivision;

        InitBeats();

        state = State.Start;
        OnStart?.Invoke();
        state = State.Running;

        InitMetronome();

        prevBeatIndex = -1;
        nextBeatIndex = 0;

        return pickupBarTime;
    }

    public void StopMetronome()
    {
        MIDIPercussion.CancelScheduledPercussion();

        beatItems = Array.Empty<BeatItem>();

        state = State.Stop;
        OnStop?.Invoke();

        prevBeatIndex = -1;
        prevBeatItem = null;
        nextBeatIndex = 0;
        nextBeatItem = null;
    }

    public void MuteMetronome()
    {
    }

    public void UnMuteMetronome()
    {
    }

    private struct ScheduledEvent
    {
        public double dspTime;
        public Action OnScheduledEvent;
    }

    List<ScheduledEvent> scheduledEvents = new List<ScheduledEvent>();

    public void ScheduleEvent(double time, Action OnScheduledEvent)
    {
        scheduledEvents.Add(new ScheduledEvent()
            { dspTime = AudioSettings.dspTime + time, OnScheduledEvent = OnScheduledEvent });
        scheduledEvents.Sort((a, b) => { return -1 * a.dspTime.CompareTo(b.dspTime); });
    }

    public void CancelScheduledEvents()
    {
        scheduledEvents.Clear();
        MIDIPercussion.CancelScheduledPercussion();
    }

    private bool valuesChanged =>
        !Mathf.Approximately(valueBPM, bpm) || valueBeats != beats || valueSubdivisions != subdivision;

    private void InitBeats()
    {
        MIDIPercussion.CancelScheduledPercussion();

        beatItems = new BeatItem[beats];

        for (int i = 0; i < beats; i++)
        {
            BeatItem beatItem = new BeatItem();
            beatItems[i] = beatItem;

            if (i == 0)
            {
                beatItem.Init(
                    BeatType.Heavy,
                    bpm,
                    subdivision
                );
            }
            else
            {
                beatItem.Init(
                    BeatType.Light,
                    bpm,
                    subdivision
                );
            }

            for (var s = 0; s < subdivision; s++)
            {
                var subBeatItem = new BeatItem();
                subBeatItem.Init(
                    BeatType.Subdivision,
                    bpm,
                    subdivision
                );

                beatItem.subdivisions.Add(subBeatItem);
            }
        }
    }

    private void InitMetronome()
    {
        startTime = AudioSettings.dspTime;
        startTime += pendulumOnset ? pendulumOnsetTime : 0;
        measureStartTime = startTime;
        for (var i = 0; i < beats; i++)
        {
            beatItems[i].PlayScheduled(startTime + i * 60.0f / bpm);
        }

        startTime += pickupBar ? pickupBarTime : 0;
    }

    private void Update()
    {
        avgFrameTime = Time.time / (float)Time.frameCount;

        for (var i = scheduledEvents.Count - 1; i >= 0; i--)
        {
            if (scheduledEvents[i].dspTime <= AudioSettings.dspTime)
            {
                scheduledEvents[i].OnScheduledEvent?.Invoke();
                scheduledEvents.RemoveAt(i);
            }
        }

        if (state == State.Stop)
        {
            totalTime = measureTime = normalizedTime = 0;
            pendulumAngle = 0;
            return;
        }

        totalTime = AudioSettings.dspTime - startTime;
        measureTime = AudioSettings.dspTime - measureStartTime;

        if (state != State.Running) return;

        if (nextBeatTime < AudioSettings.dspTime && nextBeatFlag)
        {
            currentBPM = bpm;
            currentBeats = beats;
            currentSubdivisions = subdivision;
            startTime = pendulumAngle >= 0 ? nextBeatTime - 60.0 / currentBPM : nextBeatTime;
            nextBeatTime = double.MaxValue;
            nextBeatFlag = false;

            totalTime = AudioSettings.dspTime - startTime;
        }

        normalizedTime = (float)totalTime / (60f / currentBPM);
        pendulumAngle = NS.Easing.Sinusoidal.InOut(normalizedTime) - 0.5f;

        if (pendulumOnsetTime > 0) pendulumOnsetTime -= Time.deltaTime;
        if (pendulumOnsetTime > 0) return;

        for (var i = 0; i < beatItems.Length; i++)
        {
            if (beatItems[i].scheduleTime - AudioSettings.dspTime < avgFrameTime && !beatItems[i].invoked)
            {
                beatItems[i].invoked = true;

                if ((i + 1) == 1)
                {
                    measureStartTime = AudioSettings.dspTime;
                }

                prevBeatIndex = i;
                prevBeatTime = beatItems[i].scheduleTime;
                prevBeatItem = beatItems[i];

                nextBeatIndex = i + 1;
                if (nextBeatIndex >= beatItems.Length) nextBeatIndex = 0;
                nextBeatItem = beatItems[nextBeatIndex];

                OnBeat?.Invoke(i + 1, 60.0f / bpm);
            }

            if (AudioSettings.dspTime >= beatItems[i].scheduleTime)
            {
                if (!beatItems[i].invoked)
                {
                    beatItems[i].invoked = true;

                    if ((i + 1) == 1)
                    {
                        measureStartTime = AudioSettings.dspTime;
                    }

                    prevBeatIndex = i;
                    prevBeatTime = beatItems[i].scheduleTime;
                    prevBeatItem = beatItems[i];

                    nextBeatIndex = i + 1;
                    if (nextBeatIndex >= beatItems.Length) nextBeatIndex = 0;
                    nextBeatItem = beatItems[nextBeatIndex];

                    OnBeat?.Invoke(i + 1, 60.0f / bpm);
                }

                beatItems[i].PlayScheduled(beatItems[i].scheduleTime + beatItems.Length * 60.0f / bpm);
            }
        }

        if (prevBeatItem != null)
        {
            for (var s = 0; s < prevBeatItem.subdivision; s++)
            {
                var subTime = prevBeatTime + (s + 1) * 60.0 / prevBeatItem.bpm / (prevBeatItem.subdivision + 1.0);

                if (subTime - AudioSettings.dspTime < avgFrameTime && !prevBeatItem.subdivisions[s].invoked)
                {
                    prevBeatItem.subdivisions[s].invoked = true;
                    OnSubdivision?.Invoke(s + 1, subTime);
                }

                if (AudioSettings.dspTime >= subTime)
                {
                    if (!prevBeatItem.subdivisions[s].invoked)
                    {
                        prevBeatItem.subdivisions[s].invoked = true;
                        OnSubdivision?.Invoke(s + 1, subTime);
                    }
                }
            }
        }

        if (valuesChanged)
        {
            if (nextBeatItem == null)
            {
                nextBeatIndex = 0;
                nextBeatItem = beatItems[nextBeatIndex];
            }

            if (nextBeatItem.scheduleTime < AudioSettings.dspTime) nextBeatItem.scheduleTime += 60.0f / currentBPM;

            valueBPM = bpm;
            valueBeats = beats;
            valueSubdivisions = subdivision;

            nextBeatFlag = true;

            InitBeats();

            prevBeatItem?.PlayScheduled(prevBeatTime);
            nextBeatTime = nextBeatItem.scheduleTime;

            for (var i = 0; i < beats; i++)
            {
                var time = nextBeatItem.scheduleTime + (i - nextBeatIndex) * 60.0f / bpm;
                if (AudioSettings.dspTime >= time || time < nextBeatItem.scheduleTime)
                {
                    beatItems[i].PlayScheduled(time + beats * 60.0f / bpm);
                }
                else
                {
                    beatItems[i].PlayScheduled(time);
                }
            }
        }
    }
}
