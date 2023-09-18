using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Creature", menuName = "Create New Creature")]
public class CreatureSO : SerializedScriptableObject
{
    public string creatureName;
    public GameObject creatureModel;
    public AnimationCurve spawnWeight;
    public Material resurrectedMaterial;
    public float attackCooldown;
    public float attackRange;
    public float attackDamage;
    public bool hasBonusAttack;
    public float bonusAttackChargeTime;
}
