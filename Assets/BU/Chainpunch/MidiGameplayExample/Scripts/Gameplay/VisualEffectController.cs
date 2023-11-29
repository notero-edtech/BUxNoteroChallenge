using System;
using Notero.RaindropGameplay.Core;
using Notero.RaindropGameplay.Core.Scoring;
using UnityEngine;

namespace BU.Chainpunch.MidiGameplay.Gameplay
{
    [Serializable]
    public class MascotData
    {
        public GameObject MascotPrefab;
        public bool isSpawned = false;
        public Transform SpawnPoint;
    }
    public class VisualEffectController : MonoBehaviour
    {
        public BaseScoringProcessor m_ScoringController;
        
        public MascotData[] MascotList;
        
        public void Setup()
        {
            var instant = MascotList[0];
            Instantiate(instant.MascotPrefab, instant.SpawnPoint.position, instant.SpawnPoint.rotation);
            instant.isSpawned = true;
            m_ScoringController.OnScoreUpdated.AddListener(MascotSpawner);
        }
        
        private void MascotSpawner(SelfResultInfo Info)
        {
            switch (Info.AccuracyPercent)
            {
                case >= 15f and < 30f when !MascotList[1].isSpawned :
                    Instantiate(MascotList[1].MascotPrefab, MascotList[1].SpawnPoint.position, MascotList[1].SpawnPoint.rotation);
                    MascotList[1].isSpawned = true;
                    break;
                case >= 30f and < 45f when !MascotList[2].isSpawned:
                    Instantiate(MascotList[2].MascotPrefab, MascotList[2].SpawnPoint.position, MascotList[2].SpawnPoint.rotation);
                    MascotList[2].isSpawned = true;
                    break;
                case >= 45f and < 60f when !MascotList[3].isSpawned:
                    Instantiate(MascotList[3].MascotPrefab, MascotList[3].SpawnPoint.position, MascotList[3].SpawnPoint.rotation);
                    MascotList[3].isSpawned = true;
                    break;
                case >= 60f and < 75f when !MascotList[4].isSpawned:
                    Instantiate(MascotList[4].MascotPrefab, MascotList[4].SpawnPoint.position, MascotList[4].SpawnPoint.rotation);
                    MascotList[4].isSpawned = true;
                    break;
                case >= 75f and < 95f when !MascotList[5].isSpawned:
                    Instantiate(MascotList[5].MascotPrefab, MascotList[5].SpawnPoint.position, MascotList[5].SpawnPoint.rotation);
                    MascotList[5].isSpawned = true;
                    break;
                case >= 95f and < 100f when !MascotList[6].isSpawned:
                    Instantiate(MascotList[6].MascotPrefab, MascotList[6].SpawnPoint.position, MascotList[6].SpawnPoint.rotation);
                    MascotList[6].isSpawned = true;
                    break;
            }
        }
    }
}

