using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Progression : SerializedMonoBehaviour
{
    [Header("Testing")]
    [SerializeField] bool debugging = false;
    [ShowIf(nameof(debugging))]
    [SerializeField] int testLevel = 1;
    [Header("Player Hoard")]
    [SerializeField] CreatureSO[] playerStartingHoard;

    private HoardSpawner hoardSpawner;
    private PlayerHoardHealthUI playerHoardHealthUI;

    private int currentLevel;

    public static readonly string CURRENT_LEVEL_KEY = "Level";


    private void Awake()
    {
        hoardSpawner = GetComponent<HoardSpawner>();
        playerHoardHealthUI = FindFirstObjectByType<PlayerHoardHealthUI>();
    }

    private void Start()
    {
        StartNewLevel();
    }

    private void StartNewLevel()
    {
        currentLevel = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1);

#if UNITY_EDITOR
        if (debugging)
        {
            currentLevel = testLevel;
        }
#endif

        hoardSpawner.CreateRandomizedHoards(currentLevel);
        hoardSpawner.SpawnPlayerHoard(playerStartingHoard);

        playerHoardHealthUI.InitializePlayerHoardHealth(playerStartingHoard);
    }
}
