using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public interface IBonusAttackEffect
{
    public void ApplyBonusAttackEffect(Health[] healths, bool isPlayer, Creature attacker);
}
