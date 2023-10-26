using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

public class HoardSpawner : SerializedMonoBehaviour
{
    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine, KeyLabel = "Creature Type", ValueLabel = "Spawn Curve")]
    [SerializeField] Dictionary<CreatureSO, AnimationCurve> creatureSpawnCurves = new Dictionary<CreatureSO, AnimationCurve>();

    [SerializeField] int numberOfCreatures = 9;
    [SerializeField] int creaturesPerHoard = 3;

    public void CreateRandomizedHoards(int currentLevel)
    {
        float totalWeight = 0f;

        // Calculate total weight of all creatures based on spawn curves for current level
        foreach (var spawnCurve in creatureSpawnCurves)
        {
            AnimationCurve curve = spawnCurve.Value;
            float weight = curve.Evaluate(currentLevel);
            totalWeight += weight;
        }

        List<CreatureSO> creaturesToSpawn = new List<CreatureSO>();

        for (int i = 0; i < numberOfCreatures; i++)
        {
            creaturesToSpawn.Add(SelectRandomCreature(currentLevel, totalWeight));
        }

        // Split Creatures into Hoards
        List<List<CreatureSO>> hoards = new List<List<CreatureSO>>();
        int hoardCount = Mathf.CeilToInt(numberOfCreatures / creaturesPerHoard);

        for (int i = 0; i < hoardCount; i++)
        {
            List<CreatureSO> hoard = new List<CreatureSO>();

            for (int j = 0; j < creaturesPerHoard; j++)
            {
                int index = i * creaturesPerHoard + j;

                if (index < creaturesToSpawn.Count)
                {
                    hoard.Add(creaturesToSpawn[index]);
                }
            }

            hoards.Add(hoard);
        }

        foreach (var hoard in hoards)
        {
            SpawnHoard();
        }
    }

    private void SpawnHoard()
    {
        throw new System.NotImplementedException();
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
}
