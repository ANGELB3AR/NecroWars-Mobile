using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Progression : MonoBehaviour
{
    [SerializeField] Vector2 hoardQuantityBounds = new Vector2();
    [SerializeField] Vector2 hoardCapacityBounds = new Vector2();
    [SerializeField] Vector2 minHoardPlacementBounds = new Vector2();
    [SerializeField] Vector2 maxHoardPlacementBounds = new Vector2();
    [SerializeField] AnimationCurve difficultyCurve;
    [SerializeField] GameObject hoardPrefab;

    private int currentLevel;
    private float difficultyRating;

    public const string CURRENT_LEVEL_KEY = "Level";

    private void Start()
    {
        StartNewLevel();
    }

    private void StartNewLevel()
    {
        currentLevel = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1);

        CalculateDifficultyRating();

        int hoardQuantity = Random.Range(Mathf.FloorToInt(hoardCapacityBounds.x * difficultyRating), Mathf.FloorToInt(hoardCapacityBounds.y * difficultyRating));

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
            // Select a Creature from CreatureTypeDB within the bounds of the difficulty rating
            // Add the selected creature to the hoard
        }
    }
}
