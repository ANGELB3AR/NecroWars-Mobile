using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class HoardSpawner : SerializedMonoBehaviour
{
    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine, KeyLabel = "Creature Type", ValueLabel = "Spawn Curve")]
    [SerializeField] Dictionary<CreatureSO, AnimationCurve> creatureSpawnCurves = new Dictionary<CreatureSO, AnimationCurve>();

    public void CreateRandomizedHoards(int currentLevel)
    {
        float totalWeight = 0f;

        foreach (var spawnCurve in creatureSpawnCurves)
        {
            AnimationCurve curve = spawnCurve.Value;
            float weight = curve.Evaluate(currentLevel);
            totalWeight += weight;
        }


    }
}
