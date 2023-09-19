using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NewBonusAttack", menuName = "Create New Bonus Attack")]
public class BonusAttackSO : SerializedScriptableObject
{
    public AimType aimType;
    public BonusAttackEffect[] bonusAttackEffects;
}

public enum AimType
{
    Line_FirstHit,
    Line_HitAll,
    SurroundingArea
}