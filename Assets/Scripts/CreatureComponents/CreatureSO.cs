using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Zindeaxx;
using Zindeaxx.SoundSystem;

[CreateAssetMenu(fileName = "New Creature", menuName = "Creature")]
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

    [TabGroup("General Settings")]
    [PropertyTooltip("Maximum health amount at start of round")]
    public float maxHealth = 100f;
    [TabGroup("General Settings")]
    [ProgressBar(0, 10)]
    [PropertyTooltip("How much distance in meters will this creature leave between itself and other creatures")]
    public float stoppingDistance = 2f;

    [TabGroup("Combat", "Normal Attack")]
    [PropertyTooltip("Time in seconds between normal attacks")]
    [ProgressBar(0, 10)]
    public float attackCooldown = 0.5f;
    [TabGroup("Combat", "Normal Attack")]
    [PropertyTooltip("Distance in meters that normal attack can reach")]
    [ProgressBar(0.1f, 4.9f)]
    public float attackRange = 2.5f;
    [TabGroup("Combat", "Normal Attack")]
    [PropertyTooltip("Amount of damage in hit points that normal attack will deal on each hit")]
    public float attackDamage = 5f;
    [TabGroup("Combat", "Normal Attack")]
    [PropertyTooltip("Amount of damage in hit points that normal attack will deal on each hit")]
    public SoundSet attackSFX = null;
    [TabGroup("Combat", "Normal Attack")] 
    public bool hasAttackVFX = false;
    [TabGroup("Combat", "Normal Attack")] 
    [ShowIf(nameof(hasAttackVFX))]
    public ParticleSystem normalAttackVFX;
    [TabGroup("Combat", "Normal Attack")]
    [ShowIf(nameof(hasAttackVFX))]
    [PropertyTooltip("Where on the creature model the VFX should play")]
    public Vector3 attackVFXPosition;
    [TabGroup("Combat", "Normal Attack")]
    [ShowIf(nameof(hasAttackVFX))]
    [PropertyTooltip("Apply a rotation to the VFX")]
    public Vector3 attackVFXRotation;

    [TabGroup("Combat", "Bonus Attack")]
    [PropertyTooltip("Whether or not this creature will have a bonus attack")]
    public bool hasBonusAttack;
    [TabGroup("Combat", "Bonus Attack")]
    [ShowIf(nameof(hasBonusAttack))]
    [PropertyTooltip("Time in seconds for bonus attack to recharge after use")]
    public float bonusAttackChargeTime;
    [TabGroup("Combat", "Bonus Attack")]
    [ShowIf(nameof(hasBonusAttack))]
    [PropertyTooltip("Configuration file for bonus attack. Use 'Create New Bonus Attack' to create a new one")]
    [InlineEditor]
    public BonusAttackSO bonusAttackConfig;
}
