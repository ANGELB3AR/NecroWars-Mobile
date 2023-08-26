using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

public class Progression : SerializedMonoBehaviour
{
    [SerializeField] Vector2 hoardQuantityBounds = new Vector2();
    [SerializeField] Vector2 hoardCapacityBounds = new Vector2();
    [SerializeField] Vector2 minHoardPlacementBounds = new Vector2();
    [SerializeField] Vector2 maxHoardPlacementBounds = new Vector2();
    [SerializeField] Vector2 creatureDifficultyRatingBounds = new Vector2();
    [SerializeField] AnimationCurve difficultyCurve;
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
    }

    private void StartNewLevel()
    {
        currentLevel = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1);

        CalculateDifficultyRating();

        int hoardQuantity = Random.Range(Mathf.FloorToInt(hoardQuantityBounds.x * difficultyRating), Mathf.FloorToInt(hoardQuantityBounds.y * difficultyRating));

        for (int i = 0; i < hoardQuantity; i++)
        {
            GenerateHoard();
        }

        GeneratePlayerHoard();
    }

    private void CalculateDifficultyRating()
    {
        difficultyRating = difficultyCurve.Evaluate(currentLevel);
    }

    private void GenerateHoard()
    {
        int hoardCapacity = Random.Range(Mathf.FloorToInt(hoardCapacityBounds.x * difficultyRating), Mathf.FloorToInt(hoardCapacityBounds.y * difficultyRating));
        
        Vector3 hoardPlacement = new Vector3(Random.Range(minHoardPlacementBounds.x, maxHoardPlacementBounds.x), 0f, Random.Range(minHoardPlacementBounds.y, maxHoardPlacementBounds.y));

        GameObject newHoardInstance = Instantiate(hoardPrefab, hoardPlacement, Quaternion.identity);
        Hoard newHoard = newHoardInstance.GetComponent<Hoard>();

        for (int i = 0; i < hoardCapacity; i++)
        {
            GameObject creaturePrefab = creatureDB[Random.Range(Mathf.FloorToInt(creatureDifficultyRatingBounds.x * difficultyRating), Mathf.FloorToInt(creatureDifficultyRatingBounds.y * difficultyRating))];

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
}
