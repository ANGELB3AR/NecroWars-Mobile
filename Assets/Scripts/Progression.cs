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
    [SerializeField] float hoardQuantity;
    [SerializeField] float hoardCapacity;
    [SerializeField] Vector2 minHoardPlacementBounds = new Vector2();
    [SerializeField] Vector2 maxHoardPlacementBounds = new Vector2();
    [SerializeField] float creatureDifficultyRating;
    [SerializeField] AnimationCurve difficultyCurve;
    [SerializeField] AnimationCurve hoardQuantityCurve;
    [SerializeField] AnimationCurve hoardCapacityCurve;
    [SerializeField] AnimationCurve creatureDifficultyCurve;
    [SerializeField] GameObject hoardPrefab;
    [SerializeField] GameObject playerHoard;
    [SerializeField] GameObject[] playerStartingHoard;

    private int currentLevel;
    private float difficultyRating;

    [HideInInspector]
    public const string CURRENT_LEVEL_KEY = "Level";
    public Dictionary<int, GameObject> creatureDB;

    private void Start()
    {
        StartNewLevel();
        PrintLevelData();
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

        GeneratePlayerHoard();

        Debug.Log($"Starting level {currentLevel}");
    }

    private void CalculateDifficultySettings()
    {
        difficultyRating = difficultyCurve.Evaluate(currentLevel);

        hoardQuantity = hoardQuantityCurve.Evaluate(difficultyRating);
        hoardCapacity = hoardCapacityCurve.Evaluate(difficultyRating);
        creatureDifficultyRating = creatureDifficultyCurve.Evaluate(difficultyRating);

    }

    private void GenerateHoard()
    {
        Vector3 hoardPlacement = new Vector3(Random.Range(minHoardPlacementBounds.x, maxHoardPlacementBounds.x), 0f, Random.Range(minHoardPlacementBounds.y, maxHoardPlacementBounds.y));

        GameObject newHoardInstance = Instantiate(hoardPrefab, hoardPlacement, Quaternion.identity);
        Hoard newHoard = newHoardInstance.GetComponent<Hoard>();

        for (int i = 0; i < hoardCapacity; i++)
        {
            GameObject creaturePrefab = creatureDB[Random.Range(0, Mathf.FloorToInt(creatureDifficultyRating))];

            newHoard.CreateNewCreature(creaturePrefab);
        }
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
        }
    }

    private void PrintLevelData()
    {
        Debug.Log($"****LEVEL SUMMARY****\nCurrent Level: {currentLevel}\nDifficulty Rating: {difficultyRating}\nHoard Quantity: {hoardQuantity}" +
                    $"\nHoard Capacity: {hoardCapacity}\nCreature Difficulty Rating: {creatureDifficultyRating}");
    }
}
