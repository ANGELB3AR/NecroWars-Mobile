using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class HoardSpawner : SerializedMonoBehaviour
{
    [Header("Hoard Placement")]
    [SerializeField] Vector2 minHoardPlacementBounds = new Vector2();
    [SerializeField] Vector2 maxHoardPlacementBounds = new Vector2();

    [SerializeField] GameObject hoardPrefab;
    [SerializeField] Hoard playerHoard;

    [SerializeField] GameObject creatureBasePrefab;
    [SerializeField] CreatureSO[] playerStartingHoard;

    [SerializeField] Dictionary<int, CreatureSO> creatureDB;

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


}
