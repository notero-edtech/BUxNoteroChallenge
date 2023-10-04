/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;
using ForieroEngine.Music.NotationSystem.Extensions;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class NSUIMetronomeBeatLights : MonoBehaviour
{
    public GameObject PREFAB_MetronomeBeatLight;
    
    public float distance = 5f;
    public float beatSize = 30f;
    public float subdivisionSize = 20f;

    private int lastSubdivision = 0;
    private int lastBeats = -1;

    private List<Beat> beats = new List<Beat>();
    
    private Beat currentBeat = null;

    private class Beat
    {
        public NSUIMetronomeBeatLight light;
        public List<NSUIMetronomeBeatLight> subLights = new List<NSUIMetronomeBeatLight>();
    }

    private void Awake()
    {
        NSPlayback.OnBeatChanged += OnBeat;
        NSPlayback.OnTimeSignatureChanged += OnTimeSignatureChanged;
        NSPlayback.OnSongInitialized += OnSongInitialized;
        //NSPlayback.OnSubdivision += OnSubdivision;
    }

    private void OnSongInitialized() { }

    void Start()
    {
        OnTimeSignatureChanged(0,0, null);
    }
   
    private void OnDestroy()
    {
        NSPlayback.OnBeatChanged -= OnBeat;
        NSPlayback.OnTimeSignatureChanged -= OnTimeSignatureChanged;
        NSPlayback.OnSongInitialized -= OnSongInitialized;
        //TLUnityMetronome.instance.OnSubdivision -= OnSubdivision;
    }

    private void OnTimeSignatureChanged(int partIndex, int staveIndex, NSTimeSignature.Options options)
    {
        lastBeats = options == null ? 4 : options.timeSignatureStruct.numerator;
        CreateLights();
    }

    private void OnBeat(NSPlayback.Beat beat)
    {
        if (beat.number < 0 || beat.number > beats.Count) return;
        
        currentBeat = beats[beat.number - 1];
        currentBeat.light.DoLight(0.5f);
    }

    // void OnSubdivision(int subdivision, double time)
    // {
    //     if (currentBeat != null)
    //     {
    //         int index = subdivision - 1;
    //         if (index >= 0 && index < currentBeat.subLighs.Count) currentBeat.subLighs[index].DoLight(0.5f);
    //     }
    // }

    // private void Update()
    // {
    //     if (lastSubdivision != TLUnityMetronome.instance.subdivision || lastBeats != TLUnityMetronome.instance.beats)
    //     {
    //         lastSubdivision = TLUnityMetronome.instance.subdivision;
    //         lastBeats = TLUnityMetronome.instance.beats;
    //         CreateLights();
    //     }
    // }

    private void CreateLights()
    {
        foreach (var beat in beats)
        {
            foreach (var l in beat.subLights) { Destroy(l.gameObject); }
            Destroy(beat.light.gameObject);
        }

        beats.Clear();

        float startPosition = 0;
        float cumPosition = 0;
        GameObject lastGO = null;

        for (var b = 0; b < lastBeats; b++)
        {
            var beatGO = Instantiate(PREFAB_MetronomeBeatLight, Vector3.zero, Quaternion.identity, transform) as GameObject;
            (beatGO.transform as RectTransform).anchoredPosition = new Vector2(cumPosition, 0);
            lastGO = beatGO;

            (beatGO.transform as RectTransform).SetSize(Vector2.one * beatSize);

            var beat = new Beat();
            beat.light = beatGO.GetComponent<NSUIMetronomeBeatLight>();

            beats.Add(beat);

            cumPosition += beatSize + distance;

            for (int s = 0; s < lastSubdivision; s++)
            {
                var subdivisionGO = Instantiate(PREFAB_MetronomeBeatLight,Vector3.zero, UnityEngine.Quaternion.identity, transform) as GameObject;
                (subdivisionGO.transform as RectTransform).anchoredPosition = new Vector2(cumPosition, 0);
                lastGO = subdivisionGO;

                (subdivisionGO.transform as RectTransform).SetSize(Vector2.one * subdivisionSize);
                beat.subLights.Add(subdivisionGO.GetComponent<NSUIMetronomeBeatLight>());

                cumPosition += subdivisionSize + distance;
            }
        }

        var half = lastGO.transform.localPosition.x / 2f;

        foreach (var beat in beats)
        {
            beat.light.transform.localPosition += Vector3.left * half;
            foreach (var l in beat.subLights)
            {
                l.transform.localPosition += Vector3.left * half;
            }
        }
    }
}
