using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CreatureData : ScriptableObject
{
    public string creatureName;
    public GameObject creatureModel;
    public Material resurrectedMaterial;
    public AnimationCurve spawnProbabilityCurve;
    [ProgressBar(0.1f, 300f)] [PropertyTooltip("How many seconds between regular attacks")]
    public float attackCooldown = 0.5f;
    [ProgressBar(0.1f, 4.9f)] [PropertyTooltip("How many meters away regular attacks can hit. Limitations are in place to prevent errors.")]
    public float attackRange = 2f;
    [ProgressBar(0, 100)]
    [PropertyTooltip("How much damage a regular attack does")]
    public float attackDamage = 5f;
    public bool hasBonusAttack = false;
    [ShowIf(nameof(hasBonusAttack))] [ProgressBar(0, 300)] [PropertyTooltip("How many seconds between bonus attacks")]
    public float bonusAttackChargeTime;
}
