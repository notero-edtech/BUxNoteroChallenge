using System;
using UnityEngine;
using UnityEngine.UI;

public class BASS24TimeStretchingAndPitchShifting : MonoBehaviour
{
    public MidiSeqKaraokeScript midiScript;

    public Slider speedSlider;
    public Slider semitoneSlider;

    public Text speedText;
    public Text semitoneText;

    public Text playPauseText;

    public Text timeText;

    public void Play()
    {
        if (midiScript.State == MidiSeqStates.Playing)
        {
            midiScript.Pause();
        }
        else
        {
            midiScript.Play(false);
        }
    }

    public void Stop()
    {
        midiScript.Stop();
    }

    public void OnSpeedSliderChange()
    {
        midiScript.speed = speedSlider.value;
        speedText.text = speedSlider.value.ToString("0.000");
    }

    public void OnSemitoneSliderChange()
    {
        midiScript.semitone = (int)semitoneSlider.value;
        semitoneText.text = semitoneSlider.value.ToString("0");
    }

    TimeSpan t;
    DateTime dateTime;
    string time = "";

    MidiSeqStates state = MidiSeqStates.None;

    void Update()
    {
        t = TimeSpan.FromSeconds(midiScript.time);
        dateTime = new DateTime(t.Ticks);

        time = dateTime.ToString("mm:ss.f");

        if (time != timeText.text)
        {
            timeText.text = time;
        }

        if (state != midiScript.State)
        {
            state = midiScript.State;

            switch (state)
            {
                case MidiSeqStates.None:
                    playPauseText.text = "Play";
                    break;
                case MidiSeqStates.Finished:
                case MidiSeqStates.Pausing:
                    playPauseText.text = "Play";
                    break;
                case MidiSeqStates.Playing:
                case MidiSeqStates.PickUpBar:
                    playPauseText.text = "Pause";
                    break;
            }
        }
    }
}
