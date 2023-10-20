using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BossList : MonoBehaviour
{
    private static BossList instance;
    [Header("Boss Prefabs")]
    public List<Image> bossPrefabs;
    public int bossIndex;

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
        bossIndex = Random.Range(0, 2);
        Debug.Log(bossIndex);
        Debug.Log(bossPrefabs[bossIndex].name);
    }
}
