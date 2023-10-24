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
    [SerializeField] Vector2 minHoardPlacementBounds = new Vector2();
    [SerializeField] Vector2 maxHoardPlacementBounds = new Vector2();
    [SerializeField] AnimationCurve difficultyCurve;
    [SerializeField] AnimationCurve hoardQuantityCurve;
    [SerializeField] AnimationCurve hoardCapacityCurve;
    [SerializeField] AnimationCurve creatureDifficultyCurve;
    [SerializeField] GameObject hoardPrefab;
    [SerializeField] Hoard playerHoard;
    [SerializeField] GameObject creatureBasePrefab;
    [SerializeField] CreatureSO[] playerStartingHoard;

    private HoardSpawner hoardSpawner;

    private int currentLevel;
    private float difficultyRating;
    private int hoardQuantity;
    private int hoardCapacity;
    private int creatureDifficultyRating;
    private float creatureTotalWeight;

    private int currentNumberOfHoards;

    public static readonly string CURRENT_LEVEL_KEY = "Level";
    public Dictionary<int, CreatureSO> creatureDB;

    public event Action OnHoardDefeated;

    private void Awake()
    {
        hoardSpawner = GetComponent<HoardSpawner>();
    }

    private void Start()
    {
        StartNewLevel();
    }

    public int GetStartingHoardQuantity()
    {
        return hoardQuantity;
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

        //CalculateDifficultySettings();

        //for (int i = 0; i < hoardQuantity; i++)
        //{
        //    GenerateHoard(hoardCapacity);
        //}

        //currentNumberOfHoards = hoardQuantity;

        //GeneratePlayerHoard();
        //PrintLevelData();
    }

    private void CalculateDifficultySettings()
    {
        difficultyRating = difficultyCurve.Evaluate(currentLevel);

        hoardQuantity = Mathf.FloorToInt(hoardQuantityCurve.Evaluate(difficultyRating));
        hoardCapacity = Mathf.FloorToInt(hoardCapacityCurve.Evaluate(difficultyRating));
        creatureDifficultyRating = Mathf.FloorToInt(creatureDifficultyCurve.Evaluate(difficultyRating));

        CalculateCreatureTotalWeight();
    }

    private void CalculateCreatureTotalWeight()
    {
        List<int> eligibleCreatureIndexes = new List<int>();
        for (int i = 0; i < creatureDB.Count; i++)
        {
            if (i <= creatureDifficultyRating && creatureDB.ContainsKey(i))
            {
                eligibleCreatureIndexes.Add(i);
            }
        }

        float totalWeight = 0f;
        foreach (int i in eligibleCreatureIndexes)
        {
            float weight = creatureDB[i].spawnWeight.Evaluate(difficultyRating);
            totalWeight += weight;
        }
        creatureTotalWeight = totalWeight;
    }

    private void GenerateHoard(int numberOfCreaturesToGenerate)
    {
        Vector3 hoardPlacement = new Vector3(Random.Range(minHoardPlacementBounds.x, maxHoardPlacementBounds.x), 0f, Random.Range(minHoardPlacementBounds.y, maxHoardPlacementBounds.y));

        GameObject newHoardInstance = Instantiate(hoardPrefab, hoardPlacement, Quaternion.identity);
        Hoard newHoard = newHoardInstance.GetComponent<Hoard>();

        newHoard.OnHoardDied += HandleHoardDied;

        float cumulativeOdds = 0f;

        for (int i = 0; i < numberOfCreaturesToGenerate; i++)
        {
            CreatureSO prospectiveCreature = creatureDB[Random.Range(0, Mathf.FloorToInt(creatureDifficultyRating))];

            float odds = prospectiveCreature.spawnWeight.Evaluate(difficultyRating) / creatureTotalWeight;
            cumulativeOdds += odds;

            if (cumulativeOdds <= Random.value)
            {
                newHoard.CreateNewCreature(creatureBasePrefab, prospectiveCreature);
            }
        }

        if (newHoard.GetCreaturesInHoard().Count == 0)
        {
            Destroy(newHoard.gameObject);
            //GenerateHoard(1);
        }
    }

    private void GeneratePlayerHoard()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();

        Hoard playerHoardInstance = Instantiate(playerHoard, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Hoard>();

        playerController.SetPlayerHoard(playerHoardInstance);

        foreach (CreatureSO creatureConfig in playerStartingHoard)
        {
            Creature creatureInstance = playerHoardInstance.CreateNewCreature(creatureBasePrefab, creatureConfig);

            creatureInstance.GetHealthComponent().SetIsResurrected(true);
            creatureInstance.ChangeMaterial(creatureInstance.GetResurrectionMaterial());
        }
    }

    private void HandleHoardDied(Hoard hoard)
    {
        currentNumberOfHoards--;

        hoard.OnHoardDied -= HandleHoardDied;

        OnHoardDefeated?.Invoke();

        if (currentNumberOfHoards != 0) { return; }

        GameManager.Instance.UpdateGameState(GameState.GameWon);
    }

    private void PrintLevelData()
    {
        Debug.Log($"****LEVEL SUMMARY****\nCurrent Level: {currentLevel}\nDifficulty Rating: {difficultyRating}\nHoard Quantity: {hoardQuantity}" +
                    $"\nHoard Capacity: {hoardCapacity}\nCreature Difficulty Rating: {creatureDifficultyRating}");
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
