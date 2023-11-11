using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class PlayerHoardCustomizer : SerializedMonoBehaviour
{
    [SerializeField] CreatureSO[] creatureUpgradeHeirarchy;
    [SerializeField] private List<CreatureSO> playerStartingHoard = new List<CreatureSO>();
    [SerializeField] CreatureSO startingCreature;


    public List<CreatureSO> GetPlayerStartingHoard()
    {
        return playerStartingHoard;
    }

    [Button]
    public void AddNewCreature()
    {
        playerStartingHoard.Add(startingCreature);
    }

    public void UpgradeCreature(int creatureIndex)
    {
        CreatureSO currentCreature = playerStartingHoard[creatureIndex];

        // Find the index of creatureUpgradeHeirarchy equivalent to currentCreature
        int upgradeIndex = Array.IndexOf(creatureUpgradeHeirarchy, currentCreature);

        if (upgradeIndex == creatureUpgradeHeirarchy.Length - 1)
        {
            Debug.Log("Unable to upgrade anymore");
            return;
        }

        CreatureSO newCreature = creatureUpgradeHeirarchy[upgradeIndex + 1];

        playerStartingHoard[creatureIndex] = newCreature;
    }
}
