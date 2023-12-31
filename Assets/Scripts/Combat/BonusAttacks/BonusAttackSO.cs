using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Zindeaxx.SoundSystem;

[CreateAssetMenu(fileName = "NewBonusAttack", menuName = "Bonus Attack")]
public class BonusAttackSO : SerializedScriptableObject
{
    [EnumToggleButtons]
    public AimType aimType;

    [ProgressBar(0,20)]
    public float range;

    public SoundSet bonusAttackSFX;

    [InlineEditor]
    public IBonusAttackEffect[] bonusAttackEffects;
}

public enum AimType
{
    Line_FirstHit,
    Line_HitAll,
    SurroundingArea
}
