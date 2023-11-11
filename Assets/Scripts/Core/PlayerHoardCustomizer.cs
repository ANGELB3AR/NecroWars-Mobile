using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerHoardCustomizer : SerializedMonoBehaviour
{
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

    public void UpgradeCreature(int creatureIndex, CreatureSO newCreature)
    {
        playerStartingHoard[creatureIndex] = newCreature;
    }
}
