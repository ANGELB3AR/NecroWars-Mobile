using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

public class Progression : SerializedMonoBehaviour
{
    private HoardSpawner hoardSpawner;

    [Header("Testing")]
    [SerializeField] bool debugLevel = false;
    [ShowIf(nameof(debugLevel))]
    [SerializeField] int testLevel = 1;

    [Header("Level Data")]
    [ShowInInspector]
    private int currentLevel;
    [ShowInInspector]
    private LevelConfig currentLevelConfig;
    [SerializeField] LevelConfig[] levelConfigs;


    public static readonly string CURRENT_LEVEL_KEY = "Level";

    public event Action OnHoardDefeated;


    private void Awake()
    {
        hoardSpawner = GetComponent<HoardSpawner>();
    }

    private void Start()
    {
        ConfigureNewLevel();
    }

    private void ConfigureNewLevel()
    {
        currentLevel = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1);

        if (debugLevel)
        {
            currentLevel = testLevel;
        }

        currentLevelConfig = levelConfigs[currentLevel - 1];

        hoardSpawner.SetUpNewLevel(currentLevelConfig);
    }


    private void HandleHoardDied(Hoard hoard)
    {
        //currentNumberOfHoards--;

        //hoard.OnHoardDied -= HandleHoardDied;

        //OnHoardDefeated?.Invoke();

        //if (currentNumberOfHoards != 0) { return; }

        //GameManager.Instance.UpdateGameState(GameState.GameWon);
    }

    private void PrintLevelData()
    {
        //Debug.Log($"****LEVEL SUMMARY****\nCurrent Level: {currentLevel}\nDifficulty Rating: {difficultyRating}\nHoard Quantity: {hoardQuantity}" +
        //            $"\nHoard Capacity: {hoardCapacity}\nCreature Difficulty Rating: {creatureDifficultyRating}");
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
