using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NewBonusAttack", menuName = "Create New Bonus Attack")]
public class BonusAttackSO : SerializedScriptableObject
{
    [EnumToggleButtons]
    public AimType aimType;

    [ProgressBar(0,20)]
    public float range;

    [InlineEditor]
    public IBonusAttackEffect[] bonusAttackEffects;
}

public enum AimType
{
    Line_FirstHit,
    Line_HitAll,
    SurroundingArea
}