using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureWindow : MonoBehaviour
{
    private PlayerHoardCustomizer playerHoardCustomizer;
    private List<CreatureSO> playerStartingHoard;
    private int activeChild;

    [SerializeField] private GameObject dummyCreaturePrefab;
    [SerializeField] Transform dummyCreaturePlacement;
    [SerializeField] float dummyCreatureSpacing;
    [SerializeField] Vector3 cameraOffset;


    private void Awake()
    {
        playerHoardCustomizer = FindObjectOfType<PlayerHoardCustomizer>();
    }

    private void Start()
    {
        playerStartingHoard = playerHoardCustomizer.GetPlayerStartingHoard();
        activeChild = 0;
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

    private void ShiftDummyCreatures()
    {
        var pos = dummyCreaturePlacement.GetChild(activeChild).position;

        Camera.main.transform.position = pos + cameraOffset;
    }

    public void GetNextChild()
    {
        activeChild++;

        if (activeChild > dummyCreaturePlacement.childCount - 1)
        {
            activeChild = 0;
        }

        ShiftDummyCreatures();
    }

    public void GetPreviousChild()
    {
        activeChild--;

        if (activeChild < 0)
        {
            activeChild = dummyCreaturePlacement.childCount - 1;
        }

        ShiftDummyCreatures();
    }

}
