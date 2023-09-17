using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CreatureData : ScriptableObject
{
    public string creatureName;
    public GameObject creatureModel;
    public Material resurrectedMaterial;
    public float attackCooldown;
    [PropertyRange(0.1f, 5f)]
    public float attackRange = 0.1f;

    public float attackDamage;
    public bool hasBonusAttack;
    [ShowIf(nameof(hasBonusAttack))]
    public float bonusAttackChargeTime;
    public AnimationCurve spawnProbabilityCurve;
}
