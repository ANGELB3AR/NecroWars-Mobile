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

}
