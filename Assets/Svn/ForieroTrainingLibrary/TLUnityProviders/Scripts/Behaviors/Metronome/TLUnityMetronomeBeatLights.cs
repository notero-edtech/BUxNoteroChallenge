/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using UnityEngine;

public class TLUnityMetronomeBeatLights : MonoBehaviour
{
    public GameObject PREFAB_MetronomeBeatLight;

    public float size = 1f;
    public float distance = 0.1f;
    public float beatScale = 1f;
    public float subdivisionScale = 0.5f;

    int lastSubdivision = -1;
    int lastBeats = -1;

    List<Beat> beats = new List<Beat>();

    class Beat
    {
        public TLUnityMetronomeBeatLight light;
        public List<TLUnityMetronomeBeatLight> subLighs = new List<TLUnityMetronomeBeatLight>();
    }

    void Awake()
    {
        TLUnityMetronome.instance.OnBeat += Instance_OnBeat; ;
        TLUnityMetronome.instance.OnSubdivision += Instance_OnSubdivision;
    }

    private void OnDestroy()
    {
        TLUnityMetronome.instance.OnBeat -= Instance_OnBeat; ;
        TLUnityMetronome.instance.OnSubdivision -= Instance_OnSubdivision;
    }

    Beat currentBeat = null;

    void Instance_OnBeat(int beat, double time)
    {
        currentBeat = beats[beat - 1];
        currentBeat.light.DoLight(0.5f);
    }


    void Instance_OnSubdivision(int subdivision, double time)
    {
        if (currentBeat != null)
        {
            int index = subdivision - 1;
            if (index >= 0 && index < currentBeat.subLighs.Count) currentBeat.subLighs[index].DoLight(0.5f);
        }
    }

    private void Update()
    {
        if (lastSubdivision != TLUnityMetronome.instance.subdivision || lastBeats != TLUnityMetronome.instance.beats)
        {
            lastSubdivision = TLUnityMetronome.instance.subdivision;
            lastBeats = TLUnityMetronome.instance.beats;
            CreateLights();
        }
    }

    private void CreateLights()
    {
        foreach (Beat beat in beats)
        {
            foreach (var l in beat.subLighs)
            {
                Destroy(l.gameObject);
            }
            Destroy(beat.light.gameObject);
        }

        beats.Clear();

        float startPosition = 0;
        float cumPosition = 0;
        GameObject lastGO = null;

        for (int b = 0; b < lastBeats; b++)
        {
            var beatGO = Instantiate(
                PREFAB_MetronomeBeatLight,
                transform.position + new Vector3(startPosition + cumPosition, 0, 0),
                Quaternion.identity,
                transform
            ) as GameObject;

            lastGO = beatGO;

            beatGO.transform.localScale = Vector3.one * beatScale;

            var beat = new Beat();
            beat.light = beatGO.GetComponent<TLUnityMetronomeBeatLight>();

            beats.Add(beat);

            cumPosition += size / 2f + size * subdivisionScale / beatScale / 2f + distance;

            for (int s = 0; s < lastSubdivision; s++)
            {
                var subdivisionGO = Instantiate(
                    PREFAB_MetronomeBeatLight,
                    transform.position + new Vector3(startPosition + cumPosition, 0, 0),
                    Quaternion.identity,
                    transform
                ) as GameObject;

                lastGO = subdivisionGO;

                subdivisionGO.transform.localScale = Vector3.one * subdivisionScale;
                beat.subLighs.Add(subdivisionGO.GetComponent<TLUnityMetronomeBeatLight>());

                cumPosition += size * subdivisionScale / beatScale * (s == lastSubdivision - 1 ? 0.5f : 1) + distance;
            }

            cumPosition += size / 2f;
        }

        float half = lastGO.transform.localPosition.x / 2f;

        foreach (Beat beat in beats)
        {
            beat.light.transform.localPosition += Vector3.left * half;
            foreach (var l in beat.subLighs)
            {
                l.transform.localPosition += Vector3.left * half;
            }

        }
    }
}
