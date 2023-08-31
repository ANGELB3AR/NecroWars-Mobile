using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodKnight : Creature
{
    public override void BonusAttack()
    {
        if (!bonusAttackReady) { return; }

        base.BonusAttack();

        Debug.Log("BONUS ATTACK!");
    }
}
