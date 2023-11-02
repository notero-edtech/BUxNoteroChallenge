using System;
using System.Collections;
using System.Collections.Generic;
using BU.QuizExample.Scripts;
using DataStore.Quiz;
using Newtonsoft.Json;
using Notero.QuizConnector;
using Notero.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BU.RRTT.QuizExample.Scripts.BossSystem
{
    public class BossList : MonoBehaviour
    {
        private static BossList instance;
        [Header("Boss Prefabs")]
        public List<Image> bossPrefabs;

        public byte bossIndex;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        private void OnEnable()
        {
            bossIndex = (byte) Random.Range(0, 2);
            Debug.Log(bossIndex);
            Debug.Log(bossPrefabs[bossIndex].name);
        }
    }
}


