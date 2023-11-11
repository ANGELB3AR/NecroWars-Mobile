using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zindeaxx.SoundSystem;

public class DummyCreature : MonoBehaviour
{
    private SoundManager soundManager;

    [SerializeField] CreatureSO creatureConfig = null;
    BonusAttackSO bonusAttackConfig = null;


    private void Awake()
    {
        soundManager = GetComponent<SoundManager>();
    }

    public void SetCreatureConfig(CreatureSO creatureConfig)
    {
        this.creatureConfig = creatureConfig;
        InitializeConfig();
    }
    private void InitializeConfig()
    {
        if (creatureConfig == null)
        {
            Debug.LogWarning("Creature config not found");
            return;
        }

        Instantiate(creatureConfig.creatureModel, this.gameObject.transform);

        if (creatureConfig.hasBonusAttack)
        {
            bonusAttackConfig = creatureConfig.bonusAttackConfig;
        }
    }
}
