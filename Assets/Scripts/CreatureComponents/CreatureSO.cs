using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Creature", menuName = "Create New Creature")]
public class CreatureSO : SerializedScriptableObject
{
    [BoxGroup("Main")]
    public string creatureName;
    [BoxGroup("Main")]
    [PreviewField]
    public GameObject creatureModel;
    [BoxGroup("Main")]
    [PreviewField]
    public Material resurrectedMaterial;
    [BoxGroup("Main")]
    public AnimationCurve spawnWeight;

    [TabGroup("Combat", "Normal Attack")]
    [PropertyTooltip("Time in seconds between normal attacks")]
    public float attackCooldown;
    [TabGroup("Combat", "Normal Attack")]
    [PropertyTooltip("Distance in meters that normal attack can reach")]
    [ProgressBar(0.1f, 4.9f)]
    public float attackRange;
    [TabGroup("Combat", "Normal Attack")]
    [PropertyTooltip("Amount of damage normal attack will deal on each hit")]
    [ProgressBar(0, 300)]
    public float attackDamage;

    [TabGroup("Combat", "Bonus Attack")]
    [PropertyTooltip("Whether or not this creature will have a bonus attack")]
    public bool hasBonusAttack;
    [TabGroup("Combat", "Bonus Attack")]
    [ShowIf(nameof(hasBonusAttack))]
    [PropertyTooltip("Time in seconds for bonus attack to recharge after use")]
    public float bonusAttackChargeTime;

    [TabGroup("General Settings")]
    [ProgressBar(0, 1000)]
    public float maxHealth;
    [TabGroup("General Settings")]
    [ProgressBar(0, 10)]
    public float stoppingDistance = 2f;
}
