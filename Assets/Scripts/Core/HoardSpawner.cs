using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

public class HoardSpawner : SerializedMonoBehaviour
{
    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine, KeyLabel = "Creature Type", ValueLabel = "Spawn Curve")]
    [SerializeField] Dictionary<CreatureSO, AnimationCurve> creatureSpawnCurves = new Dictionary<CreatureSO, AnimationCurve>();

    [SerializeField] AnimationCurve creatureCountCurve;
    [SerializeField] AnimationCurve creaturesPerHoardCurve;
    [SerializeField] AnimationCurve fluctuationThreshold;

    [SerializeField] Vector2 minHoardPlacementBounds = new Vector2();
    [SerializeField] Vector2 maxHoardPlacementBounds = new Vector2();

    [SerializeField] GameObject hoardPrefab;
    [SerializeField] Hoard playerHoard;


    public void CreateRandomizedHoards(int currentLevel)
    {
        List<CreatureSO> creaturesToSpawn;
        int creatureCount;
        
        // Select pseudo-random creatures to spawn
        SelectRandomizedCreatures(currentLevel, out creaturesToSpawn, out creatureCount);

        // Split Creatures into Hoards
        List<List<CreatureSO>> hoards = SplitCreaturesIntoHoards(currentLevel, creaturesToSpawn, ref creatureCount);

        // Spawn all hoards
        foreach (var hoard in hoards)
        {
            SpawnHoard(hoard);
        }

#if UNITY_EDITOR
        LogHoardData(hoards, currentLevel);
#endif
    }

    private void SelectRandomizedCreatures(int currentLevel, out List<CreatureSO> creaturesToSpawn, out int creatureCount)
    {
        float totalWeight = 0f;

        // Calculate total weight of all creatures based on spawn curves for current level
        foreach (var spawnCurve in creatureSpawnCurves)
        {
            AnimationCurve curve = spawnCurve.Value;
            float weight = curve.Evaluate(currentLevel);
            totalWeight += weight;
        }

        creaturesToSpawn = new List<CreatureSO>();
        creatureCount = Mathf.RoundToInt(creatureCountCurve.Evaluate(currentLevel));
        for (int i = 0; i < creatureCount; i++)
        {
            creaturesToSpawn.Add(SelectRandomCreature(currentLevel, totalWeight));
        }
    }

    private List<List<CreatureSO>> SplitCreaturesIntoHoards(int currentLevel, List<CreatureSO> creaturesToSpawn, ref int creatureCount)
    {
        int creaturesPerHoard = Mathf.RoundToInt(creaturesPerHoardCurve.Evaluate(currentLevel));
        int creaturesPerHoardFluctuationThreshold = Mathf.RoundToInt(fluctuationThreshold.Evaluate(currentLevel));

        List<List<CreatureSO>> hoards = new List<List<CreatureSO>>();
        int hoardCount = Mathf.CeilToInt(creatureCount / creaturesPerHoard);
        int totalCreaturesAdded = 0;

        for (int i = 0; i < hoardCount; i++)
        {
            List<CreatureSO> hoard = new List<CreatureSO>();

            int creaturesThisHoard =
            Mathf.Min(creatureCount,
            Random.Range(creaturesPerHoard - creaturesPerHoardFluctuationThreshold,
            creaturesPerHoard + creaturesPerHoardFluctuationThreshold + 1));
            creaturesThisHoard = Mathf.Clamp(creaturesThisHoard, 1, creatureCount);

            for (int j = 0; j < creaturesThisHoard; j++)
            {
                if (totalCreaturesAdded < creaturesToSpawn.Count)
                {
                    hoard.Add(creaturesToSpawn[totalCreaturesAdded]);
                    totalCreaturesAdded++;
                }
            }

            creatureCount -= creaturesThisHoard;
            hoards.Add(hoard);
        }

        return hoards;
    }

    private void SpawnHoard(List<CreatureSO> hoard)
    {
        Vector3 hoardPlacement = new Vector3(Random.Range(minHoardPlacementBounds.x, maxHoardPlacementBounds.x), 0f, Random.Range(minHoardPlacementBounds.y, maxHoardPlacementBounds.y));

        GameObject newHoardInstance = Instantiate(hoardPrefab, hoardPlacement, Quaternion.identity);
        Hoard newHoard = newHoardInstance.GetComponent<Hoard>();

        foreach (var creature in hoard)
        {
            newHoard.CreateNewCreature(creature);
        }
    }

    private CreatureSO SelectRandomCreature(int currentLevel, float totalWeight)
    {
        // Generate random number between 0 and the total weight
        float randomValue = Random.Range(0f, totalWeight);

        // Subtract weights from random number until a creature is selected
        foreach (var spawnCurve in creatureSpawnCurves)
        {
            CreatureSO creature = spawnCurve.Key;
            AnimationCurve curve = spawnCurve.Value;
            float weight = curve.Evaluate(currentLevel);

            randomValue -= weight;

            if (randomValue <= 0f)
            {
                return creature;
            }
        }

        return null;
    }

    public void SpawnPlayerHoard(CreatureSO[] playerStartingHoard)
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();

        Hoard playerHoardInstance = Instantiate(playerHoard, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Hoard>();

        playerController.SetPlayerHoard(playerHoardInstance);

        foreach (CreatureSO creature in playerStartingHoard)
        {
            Creature creatureInstance = playerHoardInstance.CreateNewCreature(creature);

            creatureInstance.GetHealthComponent().SetIsResurrected(true);
            creatureInstance.ChangeMaterial(creatureInstance.GetResurrectionMaterial());
        }
    }

    public void LogHoardData(List<List<CreatureSO>> hoards, int currentLevel)
    {
        Dictionary<string, int> creatureTypeCounts = new Dictionary<string, int>();
        int totalCreatures = 0;
        int specifiedCreatureCount;
        int specifiedHoardCount;

        specifiedCreatureCount = Mathf.RoundToInt(creatureCountCurve.Evaluate(currentLevel));
        specifiedHoardCount = Mathf.RoundToInt(specifiedCreatureCount / Mathf.RoundToInt(creaturesPerHoardCurve.Evaluate(currentLevel)));

        Debug.Log("Level: " + currentLevel);
        Debug.Log("Total hoards: " + hoards.Count + " (specified: " + specifiedHoardCount + ")");

        // Iterate over each hoard
        for (int i = 0; i < hoards.Count; i++)
        {
            List<CreatureSO> hoard = hoards[i];
            Debug.Log("Hoard " + (i + 1) + " has " + hoard.Count + " creatures");

            // Count each type of creature in the hoard
            foreach (CreatureSO creature in hoard)
            {
                if (!creatureTypeCounts.ContainsKey(creature.creatureName))
                {
                    creatureTypeCounts[creature.creatureName] = 0;
                }

                creatureTypeCounts[creature.creatureName]++;
                totalCreatures++;
            }
        }

        Debug.Log("Total creatures: " + totalCreatures + " (specified: " + specifiedCreatureCount + ")");

        // Log the count of each type of creature
        foreach (KeyValuePair<string, int> entry in creatureTypeCounts)
        {
            Debug.Log("Creature type: " + entry.Key + ", Count: " + entry.Value);
        }
    }
}
