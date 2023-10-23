using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

public class HoardSpawner : SerializedMonoBehaviour
{
    [Header("Hoard Placement")]
    [SerializeField] Vector2 minHoardPlacementBounds = new Vector2();
    [SerializeField] Vector2 maxHoardPlacementBounds = new Vector2();

    [Header("Prefabs")]
    [SerializeField] GameObject hoardPrefab;
    [SerializeField] Hoard playerHoard;
    [SerializeField] GameObject creatureBasePrefab;
    
    [SerializeField] CreatureSO[] playerStartingHoard;

    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine, KeyLabel = "Difficulty Tier", ValueLabel = "Potential Hoard Configs")]
    [SerializeField] Dictionary<int, HoardConfig[]> HoardConfigDB = new Dictionary<int, HoardConfig[]>();

    public void SetUpNewLevel(LevelConfig currentLevelConfig)
    {
        HoardConfig[] currentTier;

        for (int tier = 0; tier < currentLevelConfig.difficultyTierHoards.Length; tier++)
        {
            for (int i = 0; i < currentLevelConfig.difficultyTierHoards[tier]; i++)
            {
                currentTier = HoardConfigDB[tier + 1];
                HoardConfig hoardConfig = currentTier[Random.Range(0, currentTier.Length)];

                GenerateHoard(hoardConfig);
            }
        }

        GeneratePlayerHoard();
    }

    private void GenerateHoard(HoardConfig hoardConfig)
    {
        Vector3 hoardPlacement = new Vector3(Random.Range(minHoardPlacementBounds.x, maxHoardPlacementBounds.x), 0f, Random.Range(minHoardPlacementBounds.y, maxHoardPlacementBounds.y));

        GameObject newHoardInstance = Instantiate(hoardPrefab, hoardPlacement, Quaternion.identity);
        Hoard newHoard = newHoardInstance.GetComponent<Hoard>();

        foreach (CreatureSO creatureType in hoardConfig.hoardConfiguration.Keys)
        {
            for (int i = 0; i < hoardConfig.hoardConfiguration[creatureType]; i++)
            {
                newHoard.CreateNewCreature(creatureBasePrefab, creatureType);
            }
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