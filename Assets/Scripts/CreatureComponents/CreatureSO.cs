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
    public float attackCooldown;
    public float attackRange;
    public float attackDamage;
    public bool hasBonusAttack;
    public float bonusAttackChargeTime;
}
