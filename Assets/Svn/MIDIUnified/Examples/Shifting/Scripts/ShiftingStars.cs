using System.Collections;
using System.Collections.Generic;
using ForieroEngine.MIDIUnified;
using UnityEngine;

public class ShiftingStars : MonoBehaviour
{

    public GameObject PREFAB_stars;
    public MidiSeqKaraokeScript seq;

    List<ParticleSystem> systems = new List<ParticleSystem>();

    MidiEvents midiEvents = new MidiEvents();

    void Start()
    {
        midiEvents.AddSender(seq);
        midiEvents.NoteOnEvent += (aMidiId, aValue, aChannel) =>
        {
            var system = Instantiate(PREFAB_stars).GetComponent<ParticleSystem>();

            var color = MidiConversion.GetToneColorFromMidiIndex(aMidiId);

            var c = system.colorOverLifetime;

            Gradient grad = new Gradient();
            grad.SetKeys(new GradientColorKey[] { new GradientColorKey(color, 1.0f), new GradientColorKey(color, 0.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });

            c.color = grad;

            systems.Add(system);
        };
    }

    void Update()
    {
        for (int i = systems.Count - 1; i >= 0; i--)
        {
            if (!systems[i].isPlaying)
            {
                Destroy(systems[i].gameObject);
                systems.RemoveAt(i);
            }
        }
    }
}
