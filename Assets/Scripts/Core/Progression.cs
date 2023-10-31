using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
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

    private int currentLevel;

    private int currentNumberOfHoards;

    public static readonly string CURRENT_LEVEL_KEY = "Level";

    public event Action OnHoardDefeated;

    private void Awake()
    {
        hoardSpawner = GetComponent<HoardSpawner>();
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
    }


    private void HandleHoardDied(Hoard hoard)
    {
        currentNumberOfHoards--;

        hoard.OnHoardDied -= HandleHoardDied;

        OnHoardDefeated?.Invoke();

        if (currentNumberOfHoards != 0) { return; }

        GameManager.Instance.UpdateGameState(GameState.GameWon);
    }
}
