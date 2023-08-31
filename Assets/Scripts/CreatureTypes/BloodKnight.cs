using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodKnight : Creature
{
    public void BonusAttack()
    {
        base.BonusAttack();

        Debug.Log("BONUS ATTACK!");
    }
}
