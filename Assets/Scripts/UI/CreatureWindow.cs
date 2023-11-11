using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureWindow : MonoBehaviour
{
    private PlayerHoardCustomizer playerHoardCustomizer;
    private List<CreatureSO> playerStartingHoard;

    [SerializeField] private GameObject dummyCreaturePrefab;


    private void Awake()
    {
        playerHoardCustomizer = FindObjectOfType<PlayerHoardCustomizer>();
    }

    private void Start()
    {
        playerStartingHoard = playerHoardCustomizer.GetPlayerStartingHoard();
    }

    private void OnEnable()
    {
        foreach (var creatureConfig in playerStartingHoard)
        {
            Instantiate(dummyCreaturePrefab, transform);
            DummyCreature dummyCreature = dummyCreaturePrefab.GetComponent<DummyCreature>();
            dummyCreature.SetCreatureConfig(creatureConfig);
        }
    }
}
