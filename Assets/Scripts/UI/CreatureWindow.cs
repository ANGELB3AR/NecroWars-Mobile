using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CreatureWindow : MonoBehaviour
{
    private PlayerHoardCustomizer playerHoardCustomizer;
    private List<CreatureSO> playerStartingHoard;
    private int activeChild;

    [SerializeField] private GameObject dummyCreaturePrefab;
    [SerializeField] Transform dummyCreaturePlacement;
    [SerializeField] float dummyCreatureSpacing;
    [SerializeField] Vector3 cameraOffset;
    [SerializeField] float cameraMovementSpeed = 0.5f;
    [SerializeField] float activeCreatureScale = 1.75f;


    private void Awake()
    {
        playerHoardCustomizer = FindObjectOfType<PlayerHoardCustomizer>();
    }

    private void Start()
    {
        playerStartingHoard = playerHoardCustomizer.GetPlayerStartingHoard();
        activeChild = 0;
    }

    private void ShiftDummyCreatures()
    {
        var pos = dummyCreaturePlacement.GetChild(activeChild).position;

        dummyCreaturePlacement.GetChild(activeChild).DOScale(activeCreatureScale, .5f);
        
        Camera.main.transform.DOMove(pos + cameraOffset, cameraMovementSpeed);
    }

    private void ClearHoardFromScreen()
    {
        for (int i = 0; i < dummyCreaturePlacement.childCount; i++)
        {
            var child = dummyCreaturePlacement.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    #region Public Methods

    public void UpdateHoardData()
    {
        ClearHoardFromScreen();

        playerStartingHoard = playerHoardCustomizer.GetPlayerStartingHoard();

        foreach (var creatureConfig in playerStartingHoard)
        {
            DummyCreature dummyCreature = Instantiate(dummyCreaturePrefab, dummyCreaturePlacement).GetComponent<DummyCreature>();
            dummyCreature.transform.position = dummyCreaturePlacement.childCount * dummyCreatureSpacing * Vector3.right;
            dummyCreature.SetCreatureConfig(creatureConfig);
        }

        ShiftDummyCreatures();
    }

    public void GetNextChild()
    {
        dummyCreaturePlacement.GetChild(activeChild).DOScale(1, cameraMovementSpeed);

        activeChild++;

        if (activeChild > dummyCreaturePlacement.childCount - 1)
        {
            activeChild = 0;
        }

        ShiftDummyCreatures();
    }

    public void GetPreviousChild()
    {
        dummyCreaturePlacement.GetChild(activeChild).DOScale(1, cameraMovementSpeed);

        activeChild--;

        if (activeChild < 0)
        {
            activeChild = dummyCreaturePlacement.childCount - 1;
        }

        ShiftDummyCreatures();
    }

    public void BuyNewHoardSlot()
    {
        playerHoardCustomizer.AddNewCreature();

        UpdateHoardData();
    }

    public void UpgradeActiveCreature()
    {
        playerHoardCustomizer.UpgradeCreature(activeChild);

        UpdateHoardData();
    }
   
    #endregion
}
