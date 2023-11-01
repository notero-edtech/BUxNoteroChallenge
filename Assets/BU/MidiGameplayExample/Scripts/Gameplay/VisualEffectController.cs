using BU.Gameplay.Scoring;
using BU.MidiGameplay.UI;
using Notero.MidiAdapter;
using Notero.MidiGameplay.Bot;
using Notero.MidiGameplay.Core;
using Notero.RaindropGameplay.Core;
using Notero.RaindropGameplay.Core.Scoring;
using Notero.Unity.AudioModule;
using Notero.Unity.MidiNoteInfo;
using Notero.Unity.UI.VirtualPiano;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace BU.MidiGameplay.Gameplay
{
    public class VisualEffectController : MonoBehaviour
    {
        public DemoRaindropGameController m_DemoGameController;
        
        [SerializeField] protected GameObject[] Mascot;
        [SerializeField] protected Transform[] SpawnPoint;

        
        void Start()
        {
            Instantiate(Mascot[0], SpawnPoint[0].position, SpawnPoint[0].rotation);
            m_DemoGameController.m_ScoringController.OnScoreUpdated.AddListener(MascotSpawner);
        }
        
        // Update is called once per frame
        void Update()
        {
            
        }
        
        private void MascotSpawner(SelfResultInfo Info)
        {
            if (Info.AccuracyPercent >= 15f) Instantiate(Mascot[1], SpawnPoint[1].position, SpawnPoint[1].rotation);
            if (Info.AccuracyPercent >= 30f) Instantiate(Mascot[2], SpawnPoint[2].position, SpawnPoint[2].rotation);
            if (Info.AccuracyPercent >= 45f) Instantiate(Mascot[3], SpawnPoint[3].position, SpawnPoint[3].rotation);
            if (Info.AccuracyPercent >= 60f) Instantiate(Mascot[4], SpawnPoint[4].position, SpawnPoint[4].rotation);
            if (Info.AccuracyPercent >= 75f) Instantiate(Mascot[5], SpawnPoint[5].position, SpawnPoint[5].rotation);
        }
    }
}

