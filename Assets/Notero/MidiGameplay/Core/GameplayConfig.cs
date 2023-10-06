using UnityEngine;

namespace Notero.MidiGameplay.Core
{
    [CreateAssetMenu(fileName = "New GameplayConfig", menuName = "Hendrix/Gameplay/GameplayConfig")]
    public class GameplayConfig : ScriptableObject
    {
        [SerializeField]
        private float m_SpawnPointPos = 730;

        [SerializeField]
        private float m_RaindropScrollSpeed = 150;

        [Header("Normal mode")]
        [SerializeField]
        private int m_GoodTimeMs = 400;

        [SerializeField]
        private int m_PerfectTimeMs = 350;

        [SerializeField]
        private int m_PerfectScore = 1000;

        [SerializeField]
        private int m_GoodScore = 500;

        [SerializeField]
        private int m_ThreeStarAccuracy = 80;

        [SerializeField]
        private int m_TwoStarAccuracy = 70;

        [SerializeField]
        private int m_OneStarAccuracy = 50;

        [Header("Hold on mode")]
        [SerializeField]
        private int m_HoldOnErrorTimeMs = 0;

        [SerializeField]
        private int m_HoldOnPerfectTimeMs = 150;

        public float SpawnPointPos => m_SpawnPointPos;
        public float RaindropScrollSpeed => m_RaindropScrollSpeed;

        // normal
        public int GoodTimeMs => m_GoodTimeMs;
        public int PerfectTimeMs => m_PerfectTimeMs;
        public int PerfectScore => m_PerfectScore;
        public int GoodScore => m_GoodScore;
        public int ThreeStarAccuracy => m_ThreeStarAccuracy;
        public int TwoStarAccuracy => m_TwoStarAccuracy;
        public int OneStarAccuracy => m_OneStarAccuracy;

        // hold on
        public int HoldOnErrorTimeMs => m_HoldOnErrorTimeMs;
        public int HoldOnPerfectTimeMs => m_HoldOnPerfectTimeMs;
    }
}