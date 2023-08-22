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

    private int currentLevel;
    private float difficultyRating;

    [HideInInspector]
    public const string CURRENT_LEVEL_KEY = "Level";
    public Dictionary<int, Creature> creatureDB;

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
            Creature nextCreature = creatureDB[Random.Range(Mathf.FloorToInt(creatureDifficultyRatingBounds.x * difficultyRating), Mathf.FloorToInt(creatureDifficultyRatingBounds.y * difficultyRating))];

            newHoard.AddToHoard(nextCreature);
        }
    }
}
