using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

public class Progression : SerializedMonoBehaviour
{
    [Header("Testing")]
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
    [SerializeField] GameObject[] playerStartingHoard;

    private int currentLevel;
    private float difficultyRating;
    private int hoardQuantity;
    private int hoardCapacity;
    private int creatureDifficultyRating;
    private float creatureTotalWeight;

    private int currentNumberOfHoards;

    public static readonly string CURRENT_LEVEL_KEY = "Level";
    public Dictionary<int, CreatureType> creatureDB;

    public event Action OnHoardDefeated;

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
        currentLevel = testLevel;
#endif

        CalculateDifficultySettings();

        for (int i = 0; i < hoardQuantity; i++)
        {
            GenerateHoard();
        }

        currentNumberOfHoards = hoardQuantity;

        GeneratePlayerHoard();
        PrintLevelData();
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
            float weight = creatureDB[i].WeightedScore.Evaluate(difficultyRating);
            totalWeight += weight;
        }
        creatureTotalWeight = totalWeight;
    }

    private void GenerateHoard()
    {
        Vector3 hoardPlacement = new Vector3(Random.Range(minHoardPlacementBounds.x, maxHoardPlacementBounds.x), 0f, Random.Range(minHoardPlacementBounds.y, maxHoardPlacementBounds.y));

        GameObject newHoardInstance = Instantiate(hoardPrefab, hoardPlacement, Quaternion.identity);
        Hoard newHoard = newHoardInstance.GetComponent<Hoard>();

        newHoard.OnHoardDied += HandleHoardDied;

        float cumulativeWeight = 0f;
        for (int i = 0; i < hoardCapacity; i++)
        {
            float randomWeight = Random.Range(0, creatureTotalWeight);



            GameObject creaturePrefab = creatureDB[Random.Range(0, Mathf.FloorToInt(creatureDifficultyRating))];

            newHoard.CreateNewCreature(creaturePrefab);
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

    private void GeneratePlayerHoard()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();

        Hoard playerHoardInstance = Instantiate(playerHoard, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Hoard>();

        playerController.SetPlayerHoard(playerHoardInstance);

        foreach (GameObject creaturePrefab in playerStartingHoard)
        {
            Creature creatureInstance = playerHoardInstance.CreateNewCreature(creaturePrefab);

            creatureInstance.GetHealthComponent().SetIsResurrected(true);
            creatureInstance.ChangeMaterial(creatureInstance.GetResurrectionMaterial());
        }
    }

    private void PrintLevelData()
    {
        Debug.Log($"****LEVEL SUMMARY****\nCurrent Level: {currentLevel}\nDifficulty Rating: {difficultyRating}\nHoard Quantity: {hoardQuantity}" +
                    $"\nHoard Capacity: {hoardCapacity}\nCreature Difficulty Rating: {creatureDifficultyRating}");
    }
}
