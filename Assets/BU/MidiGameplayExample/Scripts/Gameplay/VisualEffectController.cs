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
using UnityEngine.Serialization;

namespace BU.MidiGameplay.Gameplay
{
    public class VisualEffectController : MonoBehaviour
    {
        public DemoRaindropGameController m_DemoGameController;
        
        [SerializeField] protected GameObject[] Mascot;
        [SerializeField] protected Transform[] spawnPoint;
        
        void Start()
        {
            Instantiate(Mascot[0], spawnPoint[0].position, spawnPoint[0].rotation);
            m_DemoGameController.m_ScoringController.OnScoreUpdated.AddListener(MascotSpawner);
        }
        
        // Update is called once per frame
        void Update()
        {
            
        }
        
        private void MascotSpawner(SelfResultInfo info)
        {
            if (info.AccuracyPercent >= 15f) Instantiate(Mascot[1], spawnPoint[1].position, spawnPoint[1].rotation);
            if (info.AccuracyPercent >= 30f) Instantiate(Mascot[2], spawnPoint[2].position, spawnPoint[2].rotation);
            if (info.AccuracyPercent >= 45f) Instantiate(Mascot[3], spawnPoint[3].position, spawnPoint[3].rotation);
            if (info.AccuracyPercent >= 60f) Instantiate(Mascot[4], spawnPoint[4].position, spawnPoint[4].rotation);
            if (info.AccuracyPercent >= 75f) Instantiate(Mascot[5], spawnPoint[5].position, spawnPoint[5].rotation);
        }
    }
}

