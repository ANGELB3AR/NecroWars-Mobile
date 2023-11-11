using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureWindow : MonoBehaviour
{
    private PlayerHoardCustomizer playerHoardCustomizer;
    private List<CreatureSO> playerStartingHoard;

    [SerializeField] private GameObject dummyCreaturePrefab;
    [SerializeField] Transform dummyCreaturePlacement;
    [SerializeField] float dummyCreatureSpacing;


    private void Awake()
    {
        playerHoardCustomizer = FindObjectOfType<PlayerHoardCustomizer>();
    }

    private void Start()
    {
        playerStartingHoard = playerHoardCustomizer.GetPlayerStartingHoard();
    }

    public void UpdateHoardData()
    {
        foreach (var creatureConfig in playerStartingHoard)
        {
            DummyCreature dummyCreature = Instantiate(dummyCreaturePrefab, dummyCreaturePlacement).GetComponent<DummyCreature>();
            dummyCreature.transform.position = dummyCreaturePlacement.childCount * dummyCreatureSpacing * Vector3.right;
            dummyCreature.SetCreatureConfig(creatureConfig);
        }

        ShiftDummyCreatures();
    }

    public void ShiftDummyCreatures(bool moveLeft = true)
    {
        dummyCreaturePlacement.Translate(Vector3.left * dummyCreatureSpacing);
    }
}
