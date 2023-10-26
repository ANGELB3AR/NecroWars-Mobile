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
    [SerializeField] int testLevel = 1;
    [Header("Settings")]
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

    [Button]
    private void CountCreatures()
    {
        Dictionary<string, int> creatureCounts = new Dictionary<string, int>();

        Creature[] allCreatures = FindObjectsOfType<Creature>();

        foreach (Creature creature in allCreatures)
        {
            if (creature.GetDesignatedHoard().isPlayer) { continue; }

            if (creatureCounts.ContainsKey(creature.name))
            {
                creatureCounts[creature.name]++;
            }
            else
            {
                creatureCounts[creature.name] = 1;
            }
        }


        foreach (string creature in creatureCounts.Keys)
        {
            string modifiedName = creature.Substring(9, creature.Length - 16);
            creatureCounts.TryGetValue(creature, out int count);
            
            Debug.Log($"{modifiedName}: {count}");
        }
    }
}
